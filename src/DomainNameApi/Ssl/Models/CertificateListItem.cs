namespace DomainNameApi.Ssl;

/// <summary>One certificate in the list of your certificates. A domain can have several.</summary>
public sealed record CertificateListItem
{
    /// <summary>The certificate's ID. Every operation on the certificate takes it.</summary>
    public Guid CertificateId { get; init; }

    /// <summary>The domain.</summary>
    public string? DomainName { get; init; }

    /// <summary>The product code.</summary>
    public string? PackageCode { get; init; }

    /// <summary>The product name.</summary>
    public string? PackageName { get; init; }

    /// <summary>The state, for example <c>Active</c>, <c>WaitingForInstall</c>, <c>Cancelled</c> or <c>Renewed</c>.</summary>
    public string? Status { get; init; }

    /// <summary>
    /// Why the certificate is in this state, for example <c>None</c>, <c>ConfigurationWindowExpired</c> or <c>ResellerRevoked</c>.
    /// </summary>
    public string? StatusReason { get; init; }

    /// <summary>Whether the certificate has been configured (CSR, validation, contact).</summary>
    public bool IsConfigured { get; init; }

    /// <summary>Whether the certificate has been issued.</summary>
    public bool IsIssued { get; init; }

    /// <summary>When it was bought.</summary>
    public DateTime PurchasedAt { get; init; }

    /// <summary>When the paid subscription ends. Renewal is allowed from 30 days before this date.</summary>
    public DateTime SubscriptionExpiresAt { get; init; }

    /// <summary>When the issued certificate stops being valid; null until it is issued.</summary>
    public DateTime? CertificateExpiresAt { get; init; }

    /// <summary>On a renewal: the ID of the certificate it renews. Null otherwise.</summary>
    public Guid? RenewedFromCertificateId { get; init; }
}
