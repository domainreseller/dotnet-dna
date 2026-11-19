namespace DomainNameApi.Ssl;

/// <summary><c>GET ssls/certificate-content</c>: the issued certificate and its chain in PEM format.</summary>
public sealed class GetCertificateContentRequest : BaseRequest<CertificateContent>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    public GetCertificateContentRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "ssls/certificate-content";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) => query.Add("DomainName", DomainName);

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
