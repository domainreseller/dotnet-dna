namespace DomainNameApi.Ssl;

/// <summary>
/// <c>POST ssls/initialize</c>: buys an SSL certificate package for a domain. <b>Charges your balance.</b>
/// Then submit the CSR and validation details with <see cref="ConfigureCertificateRequest"/>.
/// </summary>
public sealed class PurchaseCertificateRequest : BaseRequest<SslOperationResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain the certificate is for.</param>
    /// <param name="packageCode">The product code from the SSL product list.</param>
    public PurchaseCertificateRequest(string domainName, string packageCode)
    {
        DomainName = domainName;
        PackageCode = packageCode;
    }

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <summary>The product code.</summary>
    public string PackageCode { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "ssls/initialize";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Billable;

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        Require(PackageCode, nameof(PackageCode));
    }
}
