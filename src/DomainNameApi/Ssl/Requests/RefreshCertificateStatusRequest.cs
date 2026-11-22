namespace DomainNameApi.Ssl;

/// <summary><c>PUT ssls/check-status</c>: asks the certificate authority for the current state and returns it.</summary>
public sealed class RefreshCertificateStatusRequest : BaseRequest<CertificateStatus>
{
    /// <summary>Creates the request.</summary>
    /// <param name="certificateId">The certificate.</param>
    public RefreshCertificateStatusRequest(Guid certificateId) => CertificateId = certificateId;

    /// <summary>The certificate.</summary>
    public Guid CertificateId { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Put;

    /// <inheritdoc />
    public override string Path => "ssls/check-status";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void Validate() => SslValidation.RequireCertificateId(CertificateId);
}
