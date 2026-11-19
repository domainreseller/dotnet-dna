namespace DomainNameApi.Dnssec;

/// <summary>A DS record at the registry.</summary>
public sealed record DsRecord
{
    /// <summary>The key tag.</summary>
    public int KeyTag { get; init; }

    /// <summary>The algorithm.</summary>
    public DnssecAlgorithm Algorithm { get; init; }

    /// <summary>The digest type.</summary>
    public DsDigestType DigestType { get; init; }

    /// <summary>The digest, in hexadecimal.</summary>
    public string? Digest { get; init; }

    /// <summary>The state. Several records can be active during a key rollover.</summary>
    public DsRecordStatus Status { get; init; }

    /// <summary>Whether the algorithm is no longer recommended.</summary>
    public bool IsAlgorithmNotRecommended { get; init; }

    /// <summary>When the record was added.</summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>When the record became active.</summary>
    public DateTime? ActivatedAt { get; init; }

    /// <summary>When the record was removed.</summary>
    public DateTime? RemovedAt { get; init; }
}
