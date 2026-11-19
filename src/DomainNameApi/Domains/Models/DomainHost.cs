namespace DomainNameApi.Domains;

/// <summary>A child nameserver (glue record) of a domain, such as ns1.example.com.</summary>
public sealed record DomainHost
{
    /// <summary>The host name.</summary>
    public string? Name { get; init; }

    /// <summary>Its IP addresses.</summary>
    public IReadOnlyList<HostIpAddress> IpAddresses { get; init; } = [];
}
