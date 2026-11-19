namespace DomainNameApi.Domains;

/// <summary>The state of a pending transfer.</summary>
public sealed record TransferStatus
{
    /// <summary>The domain name.</summary>
    public string? DomainName { get; init; }

    /// <summary>The transfer state.</summary>
    public string? Status { get; init; }

    /// <summary>When the transfer was requested.</summary>
    public string? RequestedDate { get; init; }

    /// <summary>When the transfer completes automatically.</summary>
    public string? FinalDate { get; init; }

    /// <summary>The registrar that requested the transfer.</summary>
    public string? RequestedClientId { get; init; }

    /// <summary>The registrar that has to act on it.</summary>
    public string? ActionClientId { get; init; }
}
