using System.Text.Json.Serialization;

namespace DomainNameApi.Domains;

/// <summary>The result of searching several domains at once.</summary>
public sealed record BulkSearchResult
{
    /// <summary>Whether the registry answered the search.</summary>
    public bool Success { get; init; }

    /// <summary>The registry's result code.</summary>
    public string? Code { get; init; }

    /// <summary>The registry's message.</summary>
    public string? OperationMessage { get; init; }

    /// <summary>Why the search failed, if it did.</summary>
    public string? Reason { get; init; }

    /// <summary>One entry per domain.</summary>
    [JsonPropertyName("infos")]
    public IReadOnlyList<DomainAvailability> Results { get; init; } = [];

    /// <summary>The domains that can be registered.</summary>
    public IEnumerable<DomainAvailability> Available => Results.Where(r => r.IsAvailable);
}
