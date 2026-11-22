namespace DomainNameApi.Ssl;

/// <summary>
/// <c>GET ssls/approvers</c>: the e-mail addresses allowed for e-mail validation. Ask either for an existing
/// certificate (<see cref="CertificateId"/>) or, before a purchase, for a domain and the package you are about to buy
/// (<see cref="DomainName"/> and <see cref="PackageCode"/>).
/// </summary>
public sealed class GetApproverEmailsRequest : BaseRequest<ApproverEmailsResult>
{
    /// <summary>Creates the request for an existing certificate. Its domain and package are used.</summary>
    /// <param name="certificateId">The certificate.</param>
    public GetApproverEmailsRequest(Guid certificateId) => CertificateId = certificateId;

    /// <summary>Creates the request for a domain you have not bought a certificate for yet.</summary>
    /// <param name="domainName">The domain you are about to buy a certificate for.</param>
    /// <param name="packageCode">The product code of the package you are about to buy.</param>
    public GetApproverEmailsRequest(string domainName, string packageCode)
    {
        DomainName = domainName;
        PackageCode = packageCode;
    }

    /// <summary>
    /// The existing certificate. When it is set, the API takes the domain and the package from the certificate and
    /// ignores <see cref="DomainName"/> and <see cref="PackageCode"/>.
    /// </summary>
    public Guid? CertificateId { get; set; }

    /// <summary>The domain, when there is no certificate yet. Needs a <see cref="PackageCode"/>.</summary>
    public string? DomainName { get; set; }

    /// <summary>The product code, when there is no certificate yet.</summary>
    public string? PackageCode { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "ssls/approvers";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) =>
        query.Add("CertificateId", CertificateId).Add("DomainName", DomainName).Add("PackageCode", PackageCode);

    /// <inheritdoc />
    protected internal override void Validate()
    {
        if (CertificateId is not null)
        {
            SslValidation.RequireCertificateId(CertificateId.Value);
            return;
        }
        if (string.IsNullOrWhiteSpace(DomainName))
        {
            throw new ArgumentException(
                "Either CertificateId (an existing certificate) or DomainName and PackageCode (before a purchase) is required.",
                nameof(CertificateId));
        }
        if (string.IsNullOrWhiteSpace(PackageCode))
        {
            throw new ArgumentException(
                "PackageCode is required with DomainName. For a certificate you already bought, pass its CertificateId instead.",
                nameof(PackageCode));
        }
    }
}
