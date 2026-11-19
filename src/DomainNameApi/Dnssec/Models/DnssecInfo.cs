namespace DomainNameApi.Dnssec;

/// <summary>The DNSSEC state of a domain: its DS records and what the registry allows.</summary>
public sealed record DnssecInfo
{
    /// <summary>The domain name.</summary>
    public string? DomainName { get; init; }

    /// <summary>Whether the registry supports DNSSEC for this domain.</summary>
    public bool IsDnssecAvailable { get; init; }

    /// <summary>The DNSSEC state.</summary>
    public DnssecStatus Status { get; init; }

    /// <summary>Whether the delegation is signed at the registry.</summary>
    [System.Text.Json.Serialization.JsonPropertyName("delegationSigned")]
    public bool IsDelegationSigned { get; init; }

    /// <summary>The maximum signature lifetime, when the registry sets one.</summary>
    public int? MaxSigLife { get; init; }

    /// <summary>When the DNSSEC state last changed.</summary>
    public DateTime? LastChangedAt { get; init; }

    /// <summary>The DS records at the registry.</summary>
    public IReadOnlyList<DsRecord> DsRecords { get; init; } = [];

    /// <summary>The algorithms the registry accepts.</summary>
    public IReadOnlyList<DnssecAlgorithm> AllowedAlgorithms { get; init; } = [];

    /// <summary>The digest types the registry accepts.</summary>
    public IReadOnlyList<DsDigestType> AllowedDigestTypes { get; init; } = [];

    /// <summary>The most DS records the domain can have.</summary>
    public int MaxDsRecordCount { get; init; }
}
