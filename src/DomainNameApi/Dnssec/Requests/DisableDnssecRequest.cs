namespace DomainNameApi.Dnssec;

/// <summary><c>POST domains/dns/dnssec/disable</c>: removes every DS record, which turns DNSSEC off.</summary>
public sealed class DisableDnssecRequest : BaseRequest<DnssecOperationResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    public DisableDnssecRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/dns/dnssec/disable";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Destructive;

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
