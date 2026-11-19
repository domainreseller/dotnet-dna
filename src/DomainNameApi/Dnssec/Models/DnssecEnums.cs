namespace DomainNameApi.Dnssec;

/// <summary>The DNSSEC state of a domain.</summary>
public enum DnssecStatus
{
    /// <summary>No DS record at the registry.</summary>
    Unsigned = 0,

    /// <summary>A DS record was sent; waiting for the registry.</summary>
    PendingEnable = 1,

    /// <summary>At least one DS record is active.</summary>
    Signed = 2,

    /// <summary>DS removal was sent; waiting for the registry.</summary>
    PendingDisable = 3,

    /// <summary>The last DNSSEC operation failed.</summary>
    Error = 4,
}

/// <summary>Kind of DNSSEC operation.</summary>
public enum DnssecOperationType
{
    /// <summary>A DS record was added.</summary>
    Add = 0,

    /// <summary>A DS record was removed.</summary>
    Remove = 1,

    /// <summary>All DS records were removed.</summary>
    RemoveAll = 2,

    /// <summary>The state was synchronised with the registry.</summary>
    Sync = 3,
}

/// <summary>The state of a DS record.</summary>
public enum DsRecordStatus
{
    /// <summary>Waiting for the registry.</summary>
    Pending = 0,

    /// <summary>Published.</summary>
    Active = 1,

    /// <summary>Being removed.</summary>
    Removing = 2,

    /// <summary>Removed.</summary>
    Removed = 3,

    /// <summary>The registry refused it.</summary>
    Failed = 4,
}

/// <summary>IANA DNSSEC algorithm numbers.</summary>
public enum DnssecAlgorithm
{
    /// <summary>RSA/MD5 (deprecated).</summary>
    RsaMd5 = 1,

    /// <summary>DSA/SHA-1.</summary>
    Dsa = 3,

    /// <summary>RSA/SHA-1.</summary>
    RsaSha1 = 5,

    /// <summary>DSA-NSEC3-SHA1.</summary>
    DsaNsec3Sha1 = 6,

    /// <summary>RSASHA1-NSEC3-SHA1.</summary>
    RsaSha1Nsec3Sha1 = 7,

    /// <summary>RSA/SHA-256.</summary>
    RsaSha256 = 8,

    /// <summary>RSA/SHA-512.</summary>
    RsaSha512 = 10,

    /// <summary>GOST R 34.10-2001.</summary>
    EccGost = 12,

    /// <summary>ECDSA P-256 with SHA-256 (the most common choice).</summary>
    EcdsaP256Sha256 = 13,

    /// <summary>ECDSA P-384 with SHA-384.</summary>
    EcdsaP384Sha384 = 14,

    /// <summary>Ed25519.</summary>
    Ed25519 = 15,

    /// <summary>Ed448.</summary>
    Ed448 = 16,
}

/// <summary>IANA DS digest types.</summary>
public enum DsDigestType
{
    /// <summary>SHA-1.</summary>
    Sha1 = 1,

    /// <summary>SHA-256 (the most common choice).</summary>
    Sha256 = 2,

    /// <summary>GOST R 34.11-94.</summary>
    Gost = 3,

    /// <summary>SHA-384.</summary>
    Sha384 = 4,
}
