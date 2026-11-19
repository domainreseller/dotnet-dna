namespace DomainNameApi.Ssl;

/// <summary>The result of buying, configuring, reissuing or revoking a certificate.</summary>
public sealed record SslOperationResult
{
    /// <summary>Whether the operation succeeded.</summary>
    public bool Success { get; init; }

    /// <summary>The API's message.</summary>
    public string? Message { get; init; }

    /// <summary>The certificate's ID.</summary>
    public Guid CertificateId { get; init; }

    /// <summary>The certificate's state after the operation.</summary>
    public string? Status { get; init; }

    /// <summary>What to publish for domain control validation (DNS record or file), when it applies.</summary>
    public string? DcvDetails { get; init; }

    /// <summary>The CSR, when the API generated one.</summary>
    public string? Csr { get; init; }

    /// <summary>The private key, when the API generated one.</summary>
    public string? PrivateKey { get; init; }
}
