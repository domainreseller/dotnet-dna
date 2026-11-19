namespace DomainNameApi.Domains;

/// <summary>
/// <c>GET domains/info</c>: the full details of a domain: dates, state, lock and privacy, nameservers,
/// child nameservers, contacts, auth code and registry attributes. Not rate limited.
/// </summary>
public sealed class GetDomainRequest : BaseRequest<DomainDetails>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain name.</param>
    public GetDomainRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain name.</summary>
    public string DomainName { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "domains/info";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) => query.Add("DomainName", DomainName);

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
