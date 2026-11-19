namespace DomainNameApi.Ssl;

/// <summary><c>GET ssls/certificate</c>: the details of a certificate (subscription, configuration, contacts).</summary>
public sealed class GetCertificateRequest : BaseRequest<CertificateDetails>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    public GetCertificateRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "ssls/certificate";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) => query.Add("DomainName", DomainName);

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
