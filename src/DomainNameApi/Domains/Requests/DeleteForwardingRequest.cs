namespace DomainNameApi.Domains;

/// <summary><c>DELETE domains/forwards</c>: removes the URL forwarding of a domain.</summary>
public sealed class DeleteForwardingRequest : BaseRequest
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    public DeleteForwardingRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Delete;

    /// <inheritdoc />
    public override string Path => "domains/forwards";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Destructive;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) => query.Add("DomainName", DomainName);

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
