using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using StirlingLabs.Utilities;
using StirlingLabs.Utilities.Assertions;

namespace StirlingLabs.Sockaddr.Tests;

public class Tests
{
    public static IEnumerable UnspecTestCases
    {
        get {
            yield return new object[] { (ushort)1000 };
            yield return new object[] { (ushort)1234 };
            yield return new object[] { (ushort)4321 };
            yield return new object[] { (ushort)32890 };
        }
    }

    public static IEnumerable IPv4TestCases
    {
        get {
            yield return new object[] { "127.0.0.1", (ushort)1000 };
            yield return new object[] { "1.2.3.4", (ushort)1234 };
            yield return new object[] { "4.3.2.1", (ushort)4321 };
            yield return new object[] { "123.88.62.201", (ushort)32890 };
        }
    }

    public static IEnumerable IPv6TestCases
    {
        get {
            yield return new object[] { "1011:2021:3031:4041:5051:6061:7071:8081", (ushort)1000, (ushort)0 };
            yield return new object[] { "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", (ushort)1000, (ushort)0 };
            yield return new object[] { "::1", (ushort)1000, (ushort)0 };
            yield return new object[] { "::1.2.3.4", (ushort)1234, (ushort)1 };
            yield return new object[] { "::4.3.2.1", (ushort)4321, (ushort)1 };
            yield return new object[] { "::1", (ushort)1000, (ushort)1 };
            yield return new object[] { "2001:db8::8a2e:370", (ushort)1, (ushort)3 };
            yield return new object[] { "::ffff:192.0.2.128", (ushort)2, (ushort)2 };
            yield return new object[] { "64:ff9b::c000:280", (ushort)3, (ushort)1 };
        }
    }

    [TestCaseSource(nameof(UnspecTestCases))]
    [SuppressMessage("Warning", "CS1718", Justification = "Yes")]
    public unsafe void ReferenceEqualityUnspecTests(ushort port)
    {
        var pSa1 = sockaddr.CreateUnspec(port);
        var pSa2 = sockaddr.CreateUnspec(port);
        ref var sa1 = ref pSa1->AsRef();
        ref var sa2 = ref pSa2->AsRef();

        Assert.False(sa1.Equals(sa2));
        Assert.True(sa1.Equals(sa1));
        Assert.True(sa2.Equals(sa2));
        Assert.False(sa1.Equals(null));
        Assert.False(sa2.Equals(null));

        Assert.False(sa1 == sa2);
        // ReSharper disable once EqualExpressionComparison
        Assert.True(sa1 == sa1);
        // ReSharper disable once EqualExpressionComparison
        Assert.True(sa2 == sa2);

        Assert.False(sa1 == null);
        Assert.False(sa2 == null);

        Assert.True(sa1 != sa2);
        // ReSharper disable once EqualExpressionComparison
        Assert.False(sa1 != sa1);
        // ReSharper disable once EqualExpressionComparison
        Assert.False(sa2 != sa2);

        Assert.True(sa1 != null);
        Assert.True(sa2 != null);

        Assert.AreEqual((nuint)Unsafe.AsPointer(ref sa1), (nuint)pSa1);
        Assert.AreEqual((nuint)Unsafe.AsPointer(ref sa2), (nuint)pSa2);

        Assert.True(sa1.Equals(pSa1));
        Assert.True(sa2.Equals(pSa2));
    }

    [TestCaseSource(nameof(IPv4TestCases))]
    [SuppressMessage("Warning", "CS1718", Justification = "Yes")]
    public unsafe void ReferenceEqualityIPv4Tests(string address, ushort port)
    {
        var pSa1 = sockaddr.CreateIPv4(address, port);
        var pSa2 = sockaddr.CreateIPv4(address, port);
        ref var sa1 = ref pSa1->AsRef();
        ref var sa2 = ref pSa2->AsRef();

        Assert.False(sa1.Equals(sa2));
        Assert.True(sa1.Equals(sa1));
        Assert.True(sa2.Equals(sa2));
        Assert.False(sa1.Equals(null));
        Assert.False(sa2.Equals(null));

        Assert.False(sa1 == sa2);
        // ReSharper disable once EqualExpressionComparison
        Assert.True(sa1 == sa1);
        // ReSharper disable once EqualExpressionComparison
        Assert.True(sa2 == sa2);

        Assert.False(sa1 == null);
        Assert.False(sa2 == null);

        Assert.True(sa1 != sa2);
        // ReSharper disable once EqualExpressionComparison
        Assert.False(sa1 != sa1);
        // ReSharper disable once EqualExpressionComparison
        Assert.False(sa2 != sa2);

        Assert.True(sa1 != null);
        Assert.True(sa2 != null);

        Assert.AreEqual((nuint)Unsafe.AsPointer(ref sa1), (nuint)pSa1);
        Assert.AreEqual((nuint)Unsafe.AsPointer(ref sa2), (nuint)pSa2);

        Assert.True(sa1.Equals(pSa1));
        Assert.True(sa2.Equals(pSa2));
    }

    [TestCaseSource(nameof(IPv6TestCases))]
    [SuppressMessage("Warning", "CS1718", Justification = "Yes")]
    public unsafe void ReferenceEqualityIPv6Tests(string address, ushort port, ushort scope)
    {
        var pSa1 = sockaddr.CreateIPv6(address, port, scope);
        var pSa2 = sockaddr.CreateIPv6(address, port, scope);
        ref var sa1 = ref pSa1->AsRef();
        ref var sa2 = ref pSa2->AsRef();

        Assert.False(sa1.Equals(sa2));
        Assert.True(sa1.Equals(sa1));
        Assert.True(sa2.Equals(sa2));
        Assert.False(sa1.Equals(null));
        Assert.False(sa2.Equals(null));

        Assert.False(sa1 == sa2);
        // ReSharper disable once EqualExpressionComparison
        Assert.True(sa1 == sa1);
        // ReSharper disable once EqualExpressionComparison
        Assert.True(sa2 == sa2);

        Assert.False(sa1 == null);
        Assert.False(sa2 == null);

        Assert.True(sa1 != sa2);
        // ReSharper disable once EqualExpressionComparison
        Assert.False(sa1 != sa1);
        // ReSharper disable once EqualExpressionComparison
        Assert.False(sa2 != sa2);

        Assert.True(sa1 != null);
        Assert.True(sa2 != null);

        Assert.AreEqual((nuint)Unsafe.AsPointer(ref sa1), (nuint)pSa1);
        Assert.AreEqual((nuint)Unsafe.AsPointer(ref sa2), (nuint)pSa2);

        Assert.True(sa1.Equals(pSa1));
        Assert.True(sa2.Equals(pSa2));
    }

    [TestCaseSource(nameof(UnspecTestCases))]
    public unsafe void UnspecTests(ushort port)
    {
        var pSa = sockaddr.CreateUnspec(port);

        Assert.False(pSa->IsIPv4());
        Assert.False(pSa->IsIPv6());
        Assert.True(pSa->IsUnspec());
        Assert.False(pSa->IsIPv4);
        Assert.False(pSa->IsIPv6);
        Assert.True(pSa->IsUnspec);

        Assert.AreEqual("*", pSa->GetAddressString().ToString());

        Assert.True(pSa->GetAddressByteSpan().IsEmpty);
        Assert.True(pSa->AddressBytes.IsEmpty);

        Assert.AreEqual(port, pSa->GetPort());

        Assert.AreEqual($"*:{port}", pSa->ToString());

        ref var sa = ref pSa->AsRef();

        Assert.False(sa.IsIPv4());
        Assert.False(sa.IsIPv6());
        Assert.True(sa.IsUnspec());
        Assert.False(sa.IsIPv4);
        Assert.False(sa.IsIPv6);
        Assert.True(sa.IsUnspec);

        Assert.AreEqual("*", sa.GetAddressString().ToString());

        Assert.True(sa.GetAddressByteSpan().IsEmpty);
        Assert.True(sa.AddressBytes.IsEmpty);

        Assert.AreEqual(port, sa.GetPort());

        Assert.AreEqual($"*:{port}", sa.ToString());
    }

    [TestCaseSource(nameof(IPv4TestCases))]
    public unsafe void IPv4Tests(string address, ushort port)
    {
        var ip = IPAddress.Parse(address);

        var addressBytes = ip.GetAddressBytes();

        var pSa = sockaddr.CreateIPv4(address, port);

        Assert.True(pSa->IsIPv4());
        Assert.False(pSa->IsIPv6());
        Assert.False(pSa->IsUnspec());
        Assert.True(pSa->IsIPv4);
        Assert.False(pSa->IsIPv6);
        Assert.False(pSa->IsUnspec);

        Assert.AreEqual(address, pSa->GetAddressString().ToString());

        BigSpanAssert.AreEqual((ReadOnlyBigSpan<byte>)addressBytes, pSa->GetAddressByteSpan());
        BigSpanAssert.AreEqual((ReadOnlyBigSpan<byte>)addressBytes, pSa->AddressBytes);

        Assert.AreEqual(port, pSa->GetPort());

        Assert.AreEqual($"{address}:{port}", pSa->ToString());

        ref var sa = ref pSa->AsRef();

        Assert.True(sa.IsIPv4());
        Assert.False(sa.IsIPv6());
        Assert.False(sa.IsUnspec());
        Assert.True(sa.IsIPv4);
        Assert.False(sa.IsIPv6);
        Assert.False(sa.IsUnspec);

        Assert.AreEqual(address, sa.GetAddressString().ToString());

        BigSpanAssert.AreEqual((ReadOnlyBigSpan<byte>)addressBytes, sa.GetAddressByteSpan());
        BigSpanAssert.AreEqual((ReadOnlyBigSpan<byte>)addressBytes, sa.AddressBytes);

        Assert.AreEqual(port, sa.GetPort());

        Assert.AreEqual($"{address}:{port}", sa.ToString());
    }

    [TestCaseSource(nameof(IPv6TestCases))]
    public unsafe void IPv6Tests(string address, ushort port, ushort scope)
    {
        var ip = IPAddress.Parse(address);

        var addressBytes = ip.GetAddressBytes();

        var pSa = sockaddr.CreateIPv6(address, port, scope);

        Assert.True(pSa->IsIPv6());
        Assert.False(pSa->IsIPv4());
        Assert.False(pSa->IsUnspec());
        Assert.True(pSa->IsIPv6);
        Assert.False(pSa->IsIPv4);
        Assert.False(pSa->IsUnspec);

        Assert.AreEqual(address, pSa->GetAddressString().ToString());

        BigSpanAssert.AreEqual((ReadOnlyBigSpan<byte>)addressBytes, pSa->GetAddressByteSpan());
        BigSpanAssert.AreEqual((ReadOnlyBigSpan<byte>)addressBytes, pSa->AddressBytes);

        Assert.AreEqual(port, pSa->GetPort());

        Assert.AreEqual(scope, pSa->GetScope());

        if (scope == 0)
            Assert.AreEqual($"{address}:{port}", pSa->ToString());
        else
        {
            var scopeName = sockaddr.GetNameOfScope(scope);
            Assert.AreEqual(scopeName == default
                    ? $"{address}:{port}%{scope}"
                    : $"{address}:{port}%{scopeName}",
                pSa->ToString());
        }

        ref var sa = ref pSa->AsRef();

        Assert.True(sa.IsIPv6());
        Assert.False(sa.IsIPv4());
        Assert.False(sa.IsUnspec());
        Assert.True(sa.IsIPv6);
        Assert.False(sa.IsIPv4);
        Assert.False(sa.IsUnspec);

        Assert.AreEqual(address, sa.GetAddressString().ToString());

        Assert.AreEqual(port, sa.GetPort());

        if (scope == 0)
            Assert.AreEqual($"{address}:{port}", pSa->ToString());
        else
        {
            var scopeName = sockaddr.GetNameOfScope(scope);
            Assert.AreEqual(scopeName == default
                    ? $"{address}:{port}%{scope}"
                    : $"{address}:{port}%{scopeName}",
                sa.ToString());
        }
    }
}
