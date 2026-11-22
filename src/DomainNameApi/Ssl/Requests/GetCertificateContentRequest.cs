namespace DomainNameApi.Ssl;

/// <summary><c>GET ssls/certificate-content</c>: the issued certificate and its chain in PEM format.</summary>
public sealed class GetCertificateContentRequest : BaseRequest<CertificateContent>
{
    /// <summary>Creates the request.</summary>
    /// <param name="certificateId">The certificate.</param>
    public GetCertificateContentRequest(Guid certificateId) => CertificateId = certificateId;

    /// <summary>The certificate.</summary>
    public Guid CertificateId { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "ssls/certificate-content";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) => query.Add("CertificateId", CertificateId);

    /// <inheritdoc />
    protected internal override void Validate() => SslValidation.RequireCertificateId(CertificateId);
}
