using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace StirlingLabs;

[PublicAPI]
public readonly unsafe struct SockaddrAddressBytes : IEnumerable<byte>, IComparable<SockaddrAddressBytes>
{
    /// <summary>
    /// Accesses or mutates an IP address bytes in network byte order. 
    /// </summary>
    /// <param name="i">The index parameter.</param>
    /// <exception cref="IndexOutOfRangeException">The indexer parameter was out of range.</exception>
    [SuppressMessage("Design", "CA1065")]
    public byte this[int i]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            var value = GetByte(i);
            if (value == -1) throw new IndexOutOfRangeException();
            return (byte)value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set {
            if (!TrySetByte(i, value))
                throw new IndexOutOfRangeException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetByte(int i)
        => sockaddr.sa_get_address_byte((sockaddr*)Unsafe.AsPointer(ref Unsafe.AsRef(this)), i);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TrySetByte(int i, byte value)
        => sockaddr.sa_set_address_byte((sockaddr*)Unsafe.AsPointer(ref Unsafe.AsRef(this)), i, value);

    public int Length
    {
        get {
            var p = (sockaddr*)Unsafe.AsPointer(ref Unsafe.AsRef(this));
            return p->IsIPv6 ? 16 : p->IsIPv4 ? 4 : 0;
        }
    }

    public IEnumerator<byte> GetEnumerator()
        => new SockaddrAddressBytesEnumerator((sockaddr*)Unsafe.AsPointer(ref Unsafe.AsRef(this)));

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj)
        => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in SockaddrAddressBytes left, in SockaddrAddressBytes right)
        => Unsafe.AreSame(ref Unsafe.AsRef(left), ref Unsafe.AsRef(right))
            || left.CompareTo(right) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in SockaddrAddressBytes left, in SockaddrAddressBytes right)
        => !(left == right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(in SockaddrAddressBytes other)
        => this == other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in SockaddrAddressBytes left, SockaddrAddressBytes* right)
        => Unsafe.AreSame(ref Unsafe.AsRef(left), ref Unsafe.AsRef<SockaddrAddressBytes>(right));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in SockaddrAddressBytes left, SockaddrAddressBytes* right)
        => !(left == right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(SockaddrAddressBytes* other)
        => this == other;

    public static implicit operator Span<byte>(in SockaddrAddressBytes sa)
        => ((sockaddr*)Unsafe.AsPointer(ref Unsafe.AsRef(sa)))->AddressBytes;

    public static implicit operator ReadOnlySpan<byte>(in SockaddrAddressBytes sa)
        => (Span<byte>)sa;

    public int CompareTo(SockaddrAddressBytes other)
        => ((Span<byte>)this).SequenceCompareTo(other);

    public static bool operator <(SockaddrAddressBytes left, SockaddrAddressBytes right)
        => left.CompareTo(right) < 0;

    public static bool operator >(SockaddrAddressBytes left, SockaddrAddressBytes right)
        => left.CompareTo(right) > 0;

    public static bool operator <=(SockaddrAddressBytes left, SockaddrAddressBytes right)
        => left.CompareTo(right) <= 0;

    public static bool operator >=(SockaddrAddressBytes left, SockaddrAddressBytes right)
        => left.CompareTo(right) >= 0;

    public Span<byte> ToSpan() => this;

    public ReadOnlySpan<byte> ToReadOnlySpan() => this;
}
