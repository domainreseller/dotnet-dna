using System.Text.Json.Serialization;

namespace DomainNameApi.Dns;

/// <summary>One value of a DNS record set.</summary>
public sealed record DnsRecord
{
    /// <summary>The value, for example <c>192.0.2.10</c>.</summary>
    public string? Content { get; init; }

    /// <summary>Whether the value is switched off.</summary>
    [JsonPropertyName("disabled")]
    public bool IsDisabled { get; init; }
}
