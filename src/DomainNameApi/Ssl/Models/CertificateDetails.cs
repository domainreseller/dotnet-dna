namespace DomainNameApi.Ssl;

/// <summary>Everything about a certificate: subscription, configuration, issued certificate and contacts.</summary>
public sealed record CertificateDetails
{
    /// <summary>The domain.</summary>
    public string? DomainName { get; init; }

    /// <summary>The state.</summary>
    public string? Status { get; init; }

    /// <summary>Whether the certificate has been configured.</summary>
    public bool IsConfigured { get; init; }

    /// <summary>Whether the certificate has been issued.</summary>
    public bool IsIssued { get; init; }

    /// <summary>The product name.</summary>
    public string? PackageName { get; init; }

    /// <summary>The product code.</summary>
    public string? PackageCode { get; init; }

    /// <summary>The purchase and expiry of the subscription.</summary>
    public CertificateSubscription? Subscription { get; init; }

    /// <summary>The configuration that was submitted.</summary>
    public CertificateConfiguration? Configuration { get; init; }

    /// <summary>The issued certificate's validity, once issued.</summary>
    public IssuedCertificate? Certificate { get; init; }

    /// <summary>The certificate contacts.</summary>
    public IReadOnlyList<CertificateContact> Contacts { get; init; } = [];
}

/// <summary>The purchase and expiry of a certificate subscription.</summary>
public sealed record CertificateSubscription
{
    /// <summary>When it was bought.</summary>
    public DateTime PurchasedAt { get; init; }

    /// <summary>When it expires.</summary>
    public DateTime ExpiresAt { get; init; }
}

/// <summary>The configuration submitted for a certificate.</summary>
public sealed record CertificateConfiguration
{
    /// <summary>When it was submitted.</summary>
    public DateTime ConfiguredAt { get; init; }

    /// <summary>A link to the certificate authority's configuration page.</summary>
    public string? ConfigurationLink { get; init; }

    /// <summary>The configuration PIN.</summary>
    public string? Pin { get; init; }

    /// <summary>The web server type.</summary>
    public string? WebServer { get; init; }

    /// <summary>The validation method.</summary>
    public string? AuthenticationType { get; init; }

    /// <summary>Additional domain names.</summary>
    public string? SanList { get; init; }

    /// <summary>Wildcard names.</summary>
    public string? WildcardList { get; init; }

    /// <summary>The approver e-mail address.</summary>
    public string? ApproverMail { get; init; }

    /// <summary>The CSR.</summary>
    public string? Csr { get; init; }

    /// <summary>What to publish for domain control validation.</summary>
    public string? DcvDetails { get; init; }
}

/// <summary>The validity of an issued certificate.</summary>
public sealed record IssuedCertificate
{
    /// <summary>When it was issued.</summary>
    public DateTime IssuedAt { get; init; }

    /// <summary>When it stops being valid.</summary>
    public DateTime ValidUntil { get; init; }
}
