namespace DomainNameApi.Ssl;

/// <summary>
/// SSL certificates. The usual flow: <see cref="PurchaseAsync"/>, then <see cref="ConfigureAsync"/> with a CSR
/// (<see cref="GenerateCsrAsync"/> can make one), then <see cref="RefreshStatusAsync"/> until it is issued,
/// then <see cref="GetContentAsync"/>. Reach it through <see cref="DomainNameApiClient.Ssl"/>.
/// </summary>
/// <remarks>
/// <para>
/// A domain can have several certificates, so an existing certificate is addressed by its ID, never by its domain
/// name. <see cref="PurchaseAsync"/> and <see cref="RenewAsync"/> return the ID (<see cref="SslOperationResult.CertificateId"/>):
/// store it. <see cref="ListAsync"/> finds the IDs of the certificates you already have.
/// </para>
/// <para>
/// A purchased or renewed certificate must be configured within 15 days. To renew, call <see cref="RenewAsync"/> and
/// then <see cref="ConfigureAsync"/> with the new ID; a second <see cref="PurchaseAsync"/> is not a renewal.
/// </para>
/// </remarks>
public sealed class SslClient
{
    private readonly DomainNameApiClient _api;

    internal SslClient(DomainNameApiClient api) => _api = api;

    /// <summary>Returns one page of your certificates, newest first. Use it to find a certificate's ID.</summary>
    /// <param name="request">The domain filter and paging; null for the first page of all certificates.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<PagedResult<CertificateListItem>> ListAsync(ListCertificatesRequest? request = null, CancellationToken cancellationToken = default) =>
        _api.SendAsync(request ?? new ListCertificatesRequest(), cancellationToken);

    /// <summary>Yields every certificate that matches the filter, newest first, page by page.</summary>
    /// <param name="request">The domain filter; null for all certificates.</param>
    /// <param name="cancellationToken">Stops the enumeration.</param>
    public IAsyncEnumerable<CertificateListItem> ListAllAsync(ListCertificatesRequest? request = null, CancellationToken cancellationToken = default) =>
        _api.ListAllAsync(request ?? new ListCertificatesRequest(), cancellationToken);

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

    /// <summary>
    /// Buys an SSL certificate package for a domain. <b>Charges your balance.</b> Store the returned
    /// <see cref="SslOperationResult.CertificateId"/> and configure the certificate within 15 days.
    /// </summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="packageCode">The product code from <c>Products.ListSslProductsAsync</c>.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<SslOperationResult> PurchaseAsync(string domainName, string packageCode, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new PurchaseCertificateRequest(domainName, packageCode), cancellationToken);

    /// <summary>
    /// Renews a certificate for another year. <b>Charges your balance</b> (the renewal price). The returned
    /// <see cref="SslOperationResult.CertificateId"/> is the ID of a <b>new</b> certificate: configure that one with
    /// <see cref="ConfigureAsync"/> within 15 days. Allowed from 30 days before the subscription ends.
    /// </summary>
    /// <param name="certificateId">The certificate to renew.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<SslOperationResult> RenewAsync(Guid certificateId, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new RenewCertificateRequest(certificateId), cancellationToken);

    /// <summary>Submits the CSR, validation method and contact so the certificate can be issued.</summary>
    /// <param name="request">The configuration.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<SslOperationResult> ConfigureAsync(ConfigureCertificateRequest request, CancellationToken cancellationToken = default) =>
        _api.SendAsync(request, cancellationToken);

    /// <summary>Asks the certificate authority for the current state and returns it.</summary>
    /// <param name="certificateId">The certificate.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<CertificateStatus> RefreshStatusAsync(Guid certificateId, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new RefreshCertificateStatusRequest(certificateId), cancellationToken);

    /// <summary>Returns the details of a certificate.</summary>
    /// <param name="certificateId">The certificate.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<CertificateDetails> GetAsync(Guid certificateId, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new GetCertificateRequest(certificateId), cancellationToken);

    /// <summary>Downloads the issued certificate and its chain in PEM format.</summary>
    /// <param name="certificateId">The certificate.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<CertificateContent> GetContentAsync(Guid certificateId, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new GetCertificateContentRequest(certificateId), cancellationToken);

    /// <summary>Reissues a certificate with a new CSR and key, or a new approver.</summary>
    /// <param name="request">The reissue details.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<SslOperationResult> ReissueAsync(ReissueCertificateRequest request, CancellationToken cancellationToken = default) =>
        _api.SendAsync(request, cancellationToken);

    /// <summary>Returns the e-mail addresses allowed for e-mail validation of an existing certificate.</summary>
    /// <param name="certificateId">The certificate. Its domain and package are used.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public async Task<IReadOnlyList<string>> GetApproverEmailsAsync(Guid certificateId, CancellationToken cancellationToken = default) =>
        (await _api.SendAsync(new GetApproverEmailsRequest(certificateId), cancellationToken).ConfigureAwait(false)).Emails;

    /// <summary>
    /// Returns the e-mail addresses allowed for e-mail validation of a domain, before you buy a certificate for it.
    /// </summary>
    /// <param name="domainName">The domain you are about to buy a certificate for.</param>
    /// <param name="packageCode">The product code of the package you are about to buy.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public async Task<IReadOnlyList<string>> GetApproverEmailsAsync(string domainName, string packageCode, CancellationToken cancellationToken = default) =>
        (await _api.SendAsync(new GetApproverEmailsRequest(domainName, packageCode), cancellationToken).ConfigureAwait(false)).Emails;

    /// <summary>Revokes a certificate. <b>This cannot be undone.</b></summary>
    /// <param name="certificateId">The certificate to revoke.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<SslOperationResult> RevokeAsync(Guid certificateId, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new RevokeCertificateRequest(certificateId), cancellationToken);
}
