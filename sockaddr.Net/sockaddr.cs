using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
#if !NO_INTERNAL_DEPS
using StirlingLabs.Utilities;
#endif

namespace StirlingLabs;

using static SockaddrExtensions;

[PublicAPI]
[DebuggerDisplay("")]
[SuppressMessage("Design", "CA1066", Justification = "Opaque")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "InvokeAsExtensionMethod")]
public readonly unsafe struct sockaddr
{
    private const string LibName = "sa";

#if NET5_0_OR_GREATER
    //[SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern bool sa_is_unspec(sockaddr* sa);

#if NET5_0_OR_GREATER
    //[SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern bool sa_is_ipv4(sockaddr* sa);

#if NET5_0_OR_GREATER
    //[SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern bool sa_is_ipv6(sockaddr* sa);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern sbyte* sa_address_to_str(sockaddr* sa);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern sockaddr* sa_unspec(ushort port);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern sockaddr* sa_ipv4(sbyte* str, ushort port);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern sockaddr* sa_ipv4_bin(byte* str, ushort port);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern sockaddr* sa_ipv6(sbyte* str, ushort port);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern sockaddr* sa_ipv6_bin(byte* str, ushort port);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern int sa_get_port(sockaddr* sa);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern bool sa_set_port(sockaddr* sa, ushort port);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern sbyte* sa_get_scope(sockaddr* sa);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern bool sa_set_scope(sockaddr* sa, sbyte* scope);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern int sa_get_scope_index(sockaddr* sa);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern bool sa_set_scope_index(sockaddr* sa, ushort scope);


#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern int sa_get_address_byte(sockaddr* sa, int index);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern bool sa_set_address_byte(sockaddr* sa, int index, byte value);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern sbyte* sa_scope_get_name(ushort scope);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern ushort sa_scope_get_index(sbyte* scope);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern byte* sa_address_bytes(sockaddr* sa, nuint* size);

    internal static byte* sa_address_bytes(sockaddr* sa, out nuint size)
    {
        fixed (nuint* pSize = &size)
            return sa_address_bytes(sa, pSize);
    }

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern void sa_free(void* p);

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern int sa_get_family_unspec();

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern int sa_get_family_ipv4();

#if NET5_0_OR_GREATER
    [SuppressGCTransition]
#endif
    [DllImport(LibName, ExactSpelling = true)]
    internal static extern int sa_get_family_ipv6();

    public static readonly int AF_UNSPEC = sa_get_family_unspec();
    public static readonly int AF_INET = sa_get_family_ipv4();
    public static readonly int AF_INET6 = sa_get_family_ipv6();

    public static sockaddr* Create(IPAddress address, ushort port)
    {
        if (address is null) throw new ArgumentNullException(nameof(address));

        bool isV4;
        switch (address.AddressFamily)
        {
            case AddressFamily.Unspecified:
                return CreateUnspec(port);
            case AddressFamily.InterNetwork:
                isV4 = true;
                break;
            case AddressFamily.InterNetworkV6:
                isV4 = false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(address),
                    "Address Family must be InterNetwork or InterNetworkV6.");
        }

        Span<byte> addressBytes = stackalloc byte[isV4 ? 4 : 16];

#if NETSTANDARD2_0
        address.GetAddressBytes().CopyTo(addressBytes);
#else
        if (!address.TryWriteBytes(addressBytes, out _))
            throw new InvalidOperationException("Unable to extract address bytes.");
#endif

        fixed (byte* p = addressBytes)
        {
            var result = isV4
                ? sa_ipv4_bin(p, port)
                : sa_ipv6_bin(p, port);

            if (result == null) throw new ArgumentException("Invalid arguments.");

            return result;
        }
    }

    public static sockaddr* CreateUnspec(ushort port)
    {
        var result = sa_unspec(port);
        if (result == null) throw new ArgumentException("Invalid arguments.");
        return result;
    }

    public static sockaddr* CreateIPv4(Utf8String address, ushort port)
    {
        var result = sa_ipv4(address.Pointer, port);
        if (result == null) throw new ArgumentException("Invalid arguments.");
        return result;
    }

    public static sockaddr* CreateIPv4(string address, ushort port)
    {
        if (address is null) throw new ArgumentNullException(nameof(address));
        return CreateIPv4(IPAddress.Parse(address), port);
    }

#if !NETSTANDARD
    public static sockaddr* Create(string endPoint)
    {
        if (endPoint is null) throw new ArgumentNullException(nameof(endPoint));
        var ep = IPEndPoint.Parse(endPoint);
        return Create(ep);
    }
#endif

    public static sockaddr* Create(string address, ushort port)
    {
        if (address is null) throw new ArgumentNullException(nameof(address));
        return Create(IPAddress.Parse(address), port);
    }

    public static sockaddr* Create(IPEndPoint endPoint)
    {
        if (endPoint is null) throw new ArgumentNullException(nameof(endPoint));
        return Create(endPoint.Address, checked((ushort)endPoint.Port));
    }

    public static sockaddr* Create(IPEndPoint endPoint, ushort scope)
    {
        if (endPoint is null) throw new ArgumentNullException(nameof(endPoint));
        var sa = Create(endPoint.Address, checked((ushort)endPoint.Port));
        sa->SetScope(scope);
        return sa;
    }

    public static sockaddr* Create(IPEndPoint endPoint, Utf8String scope)
    {
        if (endPoint is null) throw new ArgumentNullException(nameof(endPoint));
        var sa = Create(endPoint.Address, checked((ushort)endPoint.Port));
        sa->SetScopeByName(scope);
        return sa;
    }

    public static sockaddr* CreateIPv4(IPAddress address, ushort port)
    {
        if (address is null)
            throw new ArgumentNullException(nameof(address));
        if (address.AddressFamily != AddressFamily.InterNetwork)
            throw new ArgumentOutOfRangeException(nameof(address));

        Span<byte> addressBytes = stackalloc byte[4];

#if NETSTANDARD2_0
        address.GetAddressBytes().CopyTo(addressBytes);
#else
        if (!address.TryWriteBytes(addressBytes, out _))
            throw new InvalidOperationException("Unable to extract address bytes.");
#endif

        fixed (byte* p = addressBytes)
        {
            var sa = sa_ipv4_bin(p, port);

            if (sa == null) throw new ArgumentException("Invalid arguments.");

            return sa;
        }
    }

    public static sockaddr* CreateIPv6(Utf8String address, ushort port)
    {
        var result = sa_ipv6(address.Pointer, port);
        if (result == null) throw new ArgumentException("Invalid arguments.");
        return result;
    }

    public static sockaddr* CreateIPv6(string address, ushort port)
    {
        if (address is null) throw new ArgumentNullException(nameof(address));
        return CreateIPv6(IPAddress.Parse(address), port);
    }

    public static sockaddr* CreateIPv6(Utf8String address, ushort port, ushort scope)
    {
        var sa = CreateIPv6(address, port);
        if (scope != 0) sa->SetScope(scope);
        return sa;
    }

    public static sockaddr* CreateIPv6(string address, ushort port, ushort scope)
    {
        if (address is null) throw new ArgumentNullException(nameof(address));
        return CreateIPv6(IPAddress.Parse(address), port, scope);
    }

    public static sockaddr* CreateIPv6(Utf8String address, ushort port, Utf8String scope)
    {
        var sa = CreateIPv6(address, port);
        if (scope != default) sa->SetScopeByName(scope);
        return sa;
    }

    public static sockaddr* CreateIPv6(string address, ushort port, Utf8String scope)
    {
        if (address is null) throw new ArgumentNullException(nameof(address));
        return CreateIPv6(IPAddress.Parse(address), port, scope);
    }

    public static sockaddr* CreateIPv6(IPAddress address, ushort port)
    {
        if (address is null)
            throw new ArgumentNullException(nameof(address));
        if (address.AddressFamily != AddressFamily.InterNetworkV6)
            throw new ArgumentOutOfRangeException(nameof(address));

        Span<byte> addressBytes = stackalloc byte[16];

#if NETSTANDARD2_0
        address.GetAddressBytes().CopyTo(addressBytes);
#else
        if (!address.TryWriteBytes(addressBytes, out _))
            throw new InvalidOperationException("Unable to extract address bytes.");
#endif

        fixed (byte* p = addressBytes)
        {
            var sa = sa_ipv6_bin(p, port);

            if (sa == null) throw new ArgumentException("Invalid arguments.");

            return sa;
        }
    }

    public static sockaddr* CreateIPv6(IPAddress address, ushort port, Utf8String scope)
    {
        var sa = CreateIPv6(address, port);

        if (scope != default) sa->SetScopeByName(scope);

        return sa;
    }

    public static sockaddr* CreateIPv6(IPAddress address, ushort port, ushort scope)
    {
        var sa = CreateIPv6(address, port);

        if (scope != default) sa->SetScope(scope);

        return sa;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj)
        => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in sockaddr left, in sockaddr right)
        => Unsafe.AreSame(ref Unsafe.AsRef(left), ref Unsafe.AsRef(right));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in sockaddr left, in sockaddr right)
        => !(left == right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(in sockaddr other)
        => this == other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in sockaddr left, sockaddr* right)
        => Unsafe.AreSame(ref Unsafe.AsRef(left), ref Unsafe.AsRef<sockaddr>(right));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in sockaddr left, sockaddr* right)
        => !(left == right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(sockaddr* other)
        => this == other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8String GetNameOfScope(ushort scope)
        => new(sa_scope_get_name(scope));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort GetIndexOfScope(Utf8String scope)
        => sa_scope_get_index(scope.Pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string? ToString()
        => SockaddrExtensions.ToString(ref Unsafe.AsRef(this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
        => SockaddrExtensions.GetHashCode(ref Unsafe.AsRef(this));

    public bool IsUnspec
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => SockaddrExtensions.IsUnspec(ref Unsafe.AsRef(this));
    }

    public bool IsIPv4
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => SockaddrExtensions.IsIPv4(ref Unsafe.AsRef(this));
    }

    public bool IsIPv6
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => SockaddrExtensions.IsIPv6(ref Unsafe.AsRef(this));
    }

    public Span<byte> AddressBytes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => SockaddrExtensions.GetAddressByteSpan(ref Unsafe.AsRef(this));
    }

    public ushort Port
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => SockaddrExtensions.GetPort(ref Unsafe.AsRef(this));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => SockaddrExtensions.SetPort(ref Unsafe.AsRef(this), value);
    }

    public Utf8String ScopeName
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => SockaddrExtensions.GetScopeName(ref Unsafe.AsRef(this));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => SockaddrExtensions.SetScopeByName(ref Unsafe.AsRef(this), value);
    }

    public ushort ScopeIndex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => SockaddrExtensions.GetScope(ref Unsafe.AsRef(this));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => SockaddrExtensions.SetScope(ref Unsafe.AsRef(this), value);
    }

    public IPAddress IPAddress
    {
        get => SockaddrExtensions.GetIPAddress(ref Unsafe.AsRef(this));
        set => SockaddrExtensions.SetIPAddress(ref Unsafe.AsRef(this), value);
    }

    public IPEndPoint EndPoint
    {
        get => SockaddrExtensions.ToEndPoint(ref Unsafe.AsRef(this));
        set => SockaddrExtensions.CopyFromEndPoint(ref Unsafe.AsRef(this), value);
    }

    [SuppressMessage("Usage", "CA2225", Justification = "See EndPoint Property")]
    public static implicit operator IPEndPoint(in sockaddr sa)
        => Unsafe.AsRef(sa).AsPointer()->EndPoint;

    [SuppressMessage("Usage", "CA2225", Justification = "See IPAddress Property")]
    public static implicit operator IPAddress(in sockaddr sa)
        => Unsafe.AsRef(sa).AsPointer()->IPAddress;
}
