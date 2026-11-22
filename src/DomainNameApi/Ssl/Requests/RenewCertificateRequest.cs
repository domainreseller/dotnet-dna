namespace DomainNameApi.Ssl;

/// <summary>
/// <c>POST ssls/renew</c>: renews a certificate for another year. <b>Charges your balance</b> (the renewal price).
/// The answer carries the ID of a <b>new</b> certificate, which you then configure with
/// <see cref="ConfigureCertificateRequest"/> within 15 days. Allowed from 30 days before the subscription ends.
/// </summary>
/// <remarks>
/// Do not renew by purchasing again: <see cref="PurchaseCertificateRequest"/> is an unrelated new purchase at the
/// purchase price, and the old certificate would never be marked <c>Renewed</c>.
/// </remarks>
public sealed class RenewCertificateRequest : BaseRequest<SslOperationResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="certificateId">The certificate to renew.</param>
    public RenewCertificateRequest(Guid certificateId) => CertificateId = certificateId;

    /// <summary>The certificate to renew.</summary>
    public Guid CertificateId { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "ssls/renew";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Billable;

    /// <inheritdoc />
    protected internal override void Validate() => SslValidation.RequireCertificateId(CertificateId);
}
