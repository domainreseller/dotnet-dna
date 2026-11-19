namespace DomainNameApi.Dns;

/// <summary>
/// <c>POST domains/zones/apply</c>: switches the domain to the platform nameservers so its DNS zone is served.
/// Until then, records you add are stored but not visible on the internet.
/// </summary>
public sealed class ApplyDnsZoneRequest : BaseRequest<ZoneApplyResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    public ApplyDnsZoneRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/zones/apply";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) => query.Add("domainName", DomainName);

    /// <summary>The domain goes in the query string; the request has no body.</summary>
    protected internal override object? GetBody() => null;

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
