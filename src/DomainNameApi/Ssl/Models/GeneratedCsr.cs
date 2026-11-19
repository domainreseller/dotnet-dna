namespace DomainNameApi.Ssl;

/// <summary>A certificate signing request and its private key. Keep the private key safe; the API does not store it.</summary>
public sealed record GeneratedCsr
{
    /// <summary>The CSR in PEM format.</summary>
    public string? Csr { get; init; }

    /// <summary>The private key in PEM format.</summary>
    public string? PrivateKey { get; init; }
}
