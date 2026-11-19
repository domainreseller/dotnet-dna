namespace DomainNameApi.Dns;

/// <summary><c>GET domains/zones</c>: the DNS record sets of a domain.</summary>
public sealed class ListDnsRecordsRequest : BaseRequest<IReadOnlyList<DnsRecordSet>>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    public ListDnsRecordsRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "domains/zones";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) => query.Add("domainName", DomainName);

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
