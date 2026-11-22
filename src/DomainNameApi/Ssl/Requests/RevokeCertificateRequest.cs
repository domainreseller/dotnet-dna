namespace DomainNameApi.Ssl;

/// <summary><c>DELETE ssls/revoke</c>: revokes a certificate. <b>This cannot be undone.</b></summary>
public sealed class RevokeCertificateRequest : BaseRequest<SslOperationResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="certificateId">The certificate to revoke.</param>
    public RevokeCertificateRequest(Guid certificateId) => CertificateId = certificateId;

    /// <summary>The certificate to revoke.</summary>
    public Guid CertificateId { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Delete;

    /// <inheritdoc />
    public override string Path => "ssls/revoke";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Destructive;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) => query.Add("CertificateId", CertificateId);

    /// <inheritdoc />
    protected internal override void Validate() => SslValidation.RequireCertificateId(CertificateId);
}
