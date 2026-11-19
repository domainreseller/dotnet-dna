using System.Net;
using System.Net.Sockets;

namespace DomainNameApi.Domains;

/// <summary>An IP address of a child nameserver.</summary>
public sealed record HostIpAddress
{
    /// <summary>Creates an empty entry.</summary>
    public HostIpAddress()
    {
    }

    /// <summary>Creates an entry.</summary>
    /// <param name="ipAddress">The address.</param>
    /// <param name="ipVersion">The IP version.</param>
    public HostIpAddress(string ipAddress, IpVersion ipVersion)
    {
        IpAddress = ipAddress;
        IpVersion = ipVersion;
    }

    /// <summary>The address, for example <c>192.0.2.10</c> or <c>2001:db8::10</c>.</summary>
    public string? IpAddress { get; init; }

    /// <summary>The IP version.</summary>
    public IpVersion IpVersion { get; init; }

    /// <summary>Reads an address and detects its IP version.</summary>
    /// <param name="ipAddress">An IPv4 or IPv6 address.</param>
    /// <exception cref="ArgumentException">The text is not an IP address.</exception>
    public static HostIpAddress Parse(string ipAddress)
    {
        if (!IPAddress.TryParse((ipAddress ?? "").Trim(), out var parsed))
        {
            throw new ArgumentException("'" + ipAddress + "' is not an IP address.", nameof(ipAddress));
        }
        var version = parsed.AddressFamily == AddressFamily.InterNetworkV6 ? IpVersion.V6 : IpVersion.V4;
        return new HostIpAddress(parsed.ToString(), version);
    }
}
