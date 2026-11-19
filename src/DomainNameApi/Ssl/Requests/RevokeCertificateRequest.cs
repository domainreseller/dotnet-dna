namespace DomainNameApi.Ssl;

/// <summary><c>DELETE ssls/revoke</c>: revokes a certificate. <b>This cannot be undone.</b></summary>
public sealed class RevokeCertificateRequest : BaseRequest<SslOperationResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    public RevokeCertificateRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Delete;

    /// <inheritdoc />
    public override string Path => "ssls/revoke";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Destructive;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) => query.Add("DomainName", DomainName);

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
