namespace DomainNameApi.Domains;

/// <summary>The result of searching one domain.</summary>
public sealed record DomainSearchResult
{
    /// <summary>Whether the registry answered the search.</summary>
    public bool Success { get; init; }

    /// <summary>The registry's result code.</summary>
    public string? Code { get; init; }

    /// <summary>The registry's message.</summary>
    public string? OperationMessage { get; init; }

    /// <summary>Why the search failed, if it did.</summary>
    public string? Reason { get; init; }

    /// <summary>Availability and price.</summary>
    public DomainAvailability? Info { get; init; }
}
