namespace DomainNameApi.Ssl;

/// <summary>The current state of a certificate.</summary>
public sealed record CertificateStatus
{
    /// <summary>The domain.</summary>
    public string? DomainName { get; init; }

    /// <summary>The state.</summary>
    public string? Status { get; init; }

    /// <summary>A description of the state.</summary>
    public string? StatusMessage { get; init; }

    /// <summary>Whether the certificate has been configured (CSR, validation, contact).</summary>
    public bool IsConfigured { get; init; }

    /// <summary>Whether the certificate has been issued.</summary>
    public bool IsIssued { get; init; }

    /// <summary>When the issued certificate expires.</summary>
    public DateTime? CertificateExpiresAt { get; init; }
}
