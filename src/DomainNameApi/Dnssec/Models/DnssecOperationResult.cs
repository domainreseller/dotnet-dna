namespace DomainNameApi.Dnssec;

/// <summary>The result of a DNSSEC change.</summary>
public sealed record DnssecOperationResult
{
    /// <summary>The key tag of the DS record involved.</summary>
    public int? KeyTag { get; init; }

    /// <summary>What was done.</summary>
    public DnssecOperationType OperationType { get; init; }

    /// <summary>Whether the registry accepted the change.</summary>
    public bool IsSuccess { get; init; }

    /// <summary>Whether the registry still has to confirm the change.</summary>
    public bool IsPending { get; init; }

    /// <summary>The registry's message.</summary>
    public string? ResultMessage { get; init; }
}
