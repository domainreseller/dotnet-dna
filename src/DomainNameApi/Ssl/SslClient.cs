namespace DomainNameApi.Ssl;

/// <summary>
/// SSL certificates. The usual flow: <see cref="PurchaseAsync"/>, then <see cref="ConfigureAsync"/> with a CSR
/// (<see cref="GenerateCsrAsync"/> can make one), then <see cref="RefreshStatusAsync"/> until it is issued,
/// then <see cref="GetContentAsync"/>. Reach it through <see cref="DomainNameApiClient.Ssl"/>.
/// </summary>
public sealed class SslClient
{
    private readonly DomainNameApiClient _api;

    internal SslClient(DomainNameApiClient api) => _api = api;

    /// <summary>Generates a CSR and a private key. Charges nothing.</summary>
    /// <param name="request">The subject of the CSR.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<GeneratedCsr> GenerateCsrAsync(GenerateCsrRequest request, CancellationToken cancellationToken = default) =>
        _api.SendAsync(request, cancellationToken);

    /// <summary>Decodes a CSR and returns its subject.</summary>
    /// <param name="csr">The CSR in PEM format.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<CsrDetails> DecodeCsrAsync(string csr, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new DecodeCsrRequest(csr), cancellationToken);

    /// <summary>Buys an SSL certificate package for a domain. <b>Charges your balance.</b></summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="packageCode">The product code from <c>Products.ListSslProductsAsync</c>.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<SslOperationResult> PurchaseAsync(string domainName, string packageCode, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new PurchaseCertificateRequest(domainName, packageCode), cancellationToken);

    /// <summary>Submits the CSR, validation method and contact so the certificate can be issued.</summary>
    /// <param name="request">The configuration.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<SslOperationResult> ConfigureAsync(ConfigureCertificateRequest request, CancellationToken cancellationToken = default) =>
        _api.SendAsync(request, cancellationToken);

    /// <summary>Asks the certificate authority for the current state and returns it.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<CertificateStatus> RefreshStatusAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new RefreshCertificateStatusRequest(domainName), cancellationToken);

    /// <summary>Returns the details of a certificate.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<CertificateDetails> GetAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new GetCertificateRequest(domainName), cancellationToken);

    /// <summary>Downloads the issued certificate and its chain in PEM format.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<CertificateContent> GetContentAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new GetCertificateContentRequest(domainName), cancellationToken);

    /// <summary>Reissues a certificate with a new CSR and key, or a new approver.</summary>
    /// <param name="request">The reissue details.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<SslOperationResult> ReissueAsync(ReissueCertificateRequest request, CancellationToken cancellationToken = default) =>
        _api.SendAsync(request, cancellationToken);

    /// <summary>Returns the e-mail addresses allowed for e-mail validation of a domain.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="packageCode">The product code, when you have not bought the certificate yet.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public async Task<IReadOnlyList<string>> GetApproverEmailsAsync(string domainName, string? packageCode = null, CancellationToken cancellationToken = default) =>
        (await _api.SendAsync(new GetApproverEmailsRequest(domainName, packageCode), cancellationToken).ConfigureAwait(false)).Emails;

    /// <summary>Revokes a certificate. <b>This cannot be undone.</b></summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<SslOperationResult> RevokeAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new RevokeCertificateRequest(domainName), cancellationToken);
}
