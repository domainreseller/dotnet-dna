namespace DomainNameApi.Domains;

/// <summary>
/// <c>PUT domains/dns/name-server</c>: replaces the nameservers of a domain. On a DNSSEC-signed domain
/// the change is refused unless <see cref="DnssecMode"/> says what to do with the DS records.
/// </summary>
public sealed class SetNameServersRequest : BaseRequest
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="nameServers">The complete list of nameservers.</param>
    public SetNameServersRequest(string domainName, IEnumerable<string> nameServers)
    {
        DomainName = domainName;
        NameServers = nameServers?.ToList() ?? [];
    }

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <summary>The complete list of nameservers.</summary>
    public List<string> NameServers { get; set; }

    /// <summary>What to do with DNSSEC on a signed domain. Null uses the API default, <see cref="Domains.DnssecMode.Block"/>.</summary>
    public DnssecMode? DnssecMode { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Put;

    /// <inheritdoc />
    public override string Path => "domains/dns/name-server";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    public override bool IsIdempotent => true;

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        Require(NameServers, nameof(NameServers));
    }
}
