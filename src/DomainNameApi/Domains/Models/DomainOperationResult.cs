namespace DomainNameApi.Domains;

/// <summary>The result of a registration, renewal or transfer.</summary>
public sealed record DomainOperationResult
{
    /// <summary>Whether the registry accepted the operation.</summary>
    public bool Success { get; init; }

    /// <summary>The API's message.</summary>
    public string? Message { get; init; }

    /// <summary>The domain's ID.</summary>
    public Guid DomainId { get; init; }

    /// <summary>The domain's state after the operation.</summary>
    public string? Status { get; init; }

    /// <summary>The expiration date after the operation.</summary>
    public DateTime ExpirationDate { get; init; }
}
