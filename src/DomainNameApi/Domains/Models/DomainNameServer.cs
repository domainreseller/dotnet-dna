using System.Text.Json.Serialization;

namespace DomainNameApi.Domains;

/// <summary>A nameserver of a domain.</summary>
public sealed record DomainNameServer
{
    /// <summary>The host name.</summary>
    public string? Name { get; init; }

    /// <summary>Whether it is a child nameserver of the domain itself (such as ns1.example.com for example.com).</summary>
    [JsonPropertyName("isChildnameServer")]
    public bool IsChildNameServer { get; init; }

    /// <summary>Whether the registry holds a glue record for it.</summary>
    public bool IsGlueRecord { get; init; }

    /// <summary>Whether it is linked to a host object.</summary>
    public bool IsLinked { get; init; }
}
