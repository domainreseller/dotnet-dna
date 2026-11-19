namespace DomainNameApi.Domains;

/// <summary><c>GET domains/forwards</c>: the URL forwarding of a domain.</summary>
public sealed class GetForwardingRequest : BaseRequest<DomainForwarding>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    public GetForwardingRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "domains/forwards";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) => query.Add("domainName", DomainName);

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
