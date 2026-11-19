using System.Text.Json.Serialization;

namespace DomainNameApi.Domains;

/// <summary>The URL forwarding of a domain.</summary>
public sealed record DomainForwarding
{
    /// <summary>The URL the domain forwards to.</summary>
    [JsonPropertyName("redirectAlias")]
    public string? RedirectAddress { get; init; }

    /// <summary>How the domain forwards: Standard (HTTP 301) or Frame (masked).</summary>
    [JsonPropertyName("redirectType")]
    public ForwardType? ForwardType { get; init; }
}
