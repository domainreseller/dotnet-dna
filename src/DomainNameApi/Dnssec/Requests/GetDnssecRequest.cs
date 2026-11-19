namespace DomainNameApi.Dnssec;

/// <summary><c>GET domains/dns/dnssec</c>: DNSSEC state, DS records and the algorithms the registry allows.</summary>
public sealed class GetDnssecRequest : BaseRequest<DnssecInfo>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    public GetDnssecRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "domains/dns/dnssec";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) => query.Add("DomainName", DomainName);

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
