namespace DomainNameApi.Ssl;

/// <summary><c>GET ssls/certificate</c>: the details of a certificate (subscription, configuration, contacts).</summary>
public sealed class GetCertificateRequest : BaseRequest<CertificateDetails>
{
    /// <summary>Creates the request.</summary>
    /// <param name="certificateId">The certificate.</param>
    public GetCertificateRequest(Guid certificateId) => CertificateId = certificateId;

    /// <summary>The certificate.</summary>
    public Guid CertificateId { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "ssls/certificate";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) => query.Add("CertificateId", CertificateId);

    /// <inheritdoc />
    protected internal override void Validate() => SslValidation.RequireCertificateId(CertificateId);
}
