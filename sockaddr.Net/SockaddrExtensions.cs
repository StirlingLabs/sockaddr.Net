using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using StirlingLabs.Utilities;
using static StirlingLabs.sockaddr;

namespace StirlingLabs;

[PublicAPI]
[SuppressMessage("Design", "CA1045", Justification = "Hacks")]
public static unsafe class SockaddrExtensions
{
    public static bool Free(ref this sockaddr self)
    {
        var p = (sockaddr*)Unsafe.AsPointer(ref Unsafe.AsRef(self));
        if (p == null)
            return false;
        sa_free(p);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static sockaddr* AsPointer(ref this sockaddr r)
        => (sockaddr*)Unsafe.AsPointer(ref r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref sockaddr AsRef(ref this sockaddr r) => ref r;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUnspec(ref this sockaddr self)
        => sa_is_unspec(self.AsPointer());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsIPv4(ref this sockaddr self)
        => sa_is_ipv4(self.AsPointer());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsIPv6(ref this sockaddr self)
        => sa_is_ipv6(self.AsPointer());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetPort(ref this sockaddr self, ushort value)
    {
        if (!sa_set_port(self.AsPointer(), value))
            throw new ArgumentOutOfRangeException(nameof(value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort GetPort(ref this sockaddr self)
    {
        var port = sa_get_port(self.AsPointer());
        return checked((ushort)port);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetScope(ref this sockaddr self, ushort value)
    {
        var pSa = self.AsPointer();
        if (!pSa->TrySetScopeIndex(value))
            throw new ArgumentOutOfRangeException(nameof(value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort GetScope(ref this sockaddr self)
    {
        var pSa = self.AsPointer();
        var port = pSa->GetScopeNoThrow();
        return checked((ushort)port);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetScopeNoThrow(ref this sockaddr self)
        => sa_get_scope_index(self.AsPointer());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TrySetScopeIndex(ref this sockaddr self, ushort value)
        => sa_set_scope_index(self.AsPointer(), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SockaddrAddressBytes* GetAddressBytes(ref this sockaddr self)
        => (SockaddrAddressBytes*)self.AsPointer();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> GetAddressByteSpan(ref this sockaddr self)
        => new(sa_address_bytes(self.AsPointer(), out var size), (int)size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetScopeByName(ref this sockaddr self, Utf8String value)
    {
        if (!sa_set_scope(self.AsPointer(), value.Pointer))
            throw new ArgumentException("Invalid Scope", nameof(value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8String GetScopeName(ref this sockaddr self)
        => new(sa_get_scope(self.AsPointer()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(ref this sockaddr self)
        => ((IntPtr)self.AsPointer()).GetHashCode();

    private static readonly ConditionalWeakTable<string, CleanUpAction> StringCache = new();

    [SuppressMessage("Reliability", "CA2000", Justification = "Intentional")]
    public static string? ToString(ref this sockaddr self)
    {
        void CleanUp(string addressStr, Utf8String address, Utf8String scope)
        {
            if (address.IsInterned)
                return;

            if (!scope.IsInterned)
                StringCache.Add(addressStr, new(() => {
                    sa_free(address.Pointer);
                    sa_free(scope.Pointer);
                }));
            else
                StringCache.Add(addressStr, new(() => {
                    sa_free(address.Pointer);
                }));
        }

        var pSa = self.AsPointer();
        if (pSa == null) return null;

        if (pSa->IsUnspec())
            return $"*:{pSa->GetPort()}";

        if (pSa->IsIPv4())
            return pSa->ToEndPoint().ToString();

        // IPv6

        var scope = pSa->GetScopeName();

        if (scope == default)
            return pSa->ToEndPoint().ToString();

        var address = pSa->GetAddressString();
        if (address == default)
            return null;

        var addressStr = address.ToString();
        if (addressStr == null)
        {
            address.Free();
            return null;
        }

        CleanUp(addressStr, address, scope);
        return $"[{addressStr}]:{pSa->GetPort()}%{scope}";

    }

    public static Utf8String GetAddressString(ref this sockaddr self)
    {
        var sa = self.AsPointer();

        return sa == null
            ? default
            : new Utf8String(sa_address_to_str(sa));

    }

    public static IPAddress GetIPAddress(ref this sockaddr self)
    {
        var sa = self.AsPointer();

        if (sa->IsUnspec())
            return IPAddress.IPv6Any;

        var bytes = sa->GetAddressByteSpan();

#if NETSTANDARD2_0
        if (sa->IsIPv6())
            return new(bytes.ToArray(), sa->GetScope());

        return new(bytes.ToArray());
#else
        if (sa->IsIPv6())
            return new(bytes, sa->GetScope());

        return new(bytes);
#endif
    }

    public static void SetIPAddress(ref this sockaddr self, IPAddress value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        var sa = self.AsPointer();

        var bytes = sa->GetAddressByteSpan();

        switch (value.AddressFamily)
        {
            case AddressFamily.InterNetwork: {
                if (bytes.Length != 4 || !sa->IsIPv4())
                    throw new InvalidOperationException("The address provided is not of the same address family.");
#if NETSTANDARD2_0
                value.GetAddressBytes().CopyTo(bytes);
#else
                if (!value.TryWriteBytes(bytes, out _))
                    throw new InvalidOperationException("Failed to update IP Address.");
#endif
                break;
            }
            case AddressFamily.InterNetworkV6: {
                if (bytes.Length != 16 || !sa->IsIPv6())
                    throw new InvalidOperationException("The address provided is not of the same address family.");
#if NETSTANDARD2_0
                value.GetAddressBytes().CopyTo(bytes);
#else
                if (!value.TryWriteBytes(bytes, out _))
                    throw new InvalidOperationException("Failed to update IP Address.");
#endif
                break;
            }
            case AddressFamily.Unspecified: {
                if (!bytes.IsEmpty || !sa->IsUnspec())
                    throw new InvalidOperationException("The address provided is not of the same address family.");
                // do nothing
                break;
            }
            default: {
                throw new InvalidOperationException("The address provided is not of a supported address family.");
            }
        }
    }

    public static IPEndPoint ToEndPoint(ref this sockaddr self)
    {
        var sa = self.AsPointer();

        return new(sa->GetIPAddress(), sa->GetPort());
    }

    public static void CopyFromEndPoint(ref this sockaddr self, IPEndPoint value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        var sa = self.AsPointer();

        sa->SetIPAddress(value.Address);

        sa->SetPort(checked((ushort)value.Port));
    }
}
