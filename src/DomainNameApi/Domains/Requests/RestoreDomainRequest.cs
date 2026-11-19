namespace DomainNameApi.Domains;

/// <summary><c>POST domains/restore</c>: restores a deleted domain from redemption. <b>Charges your balance.</b></summary>
public sealed class RestoreDomainRequest : BaseRequest
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    public RestoreDomainRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/restore";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Billable;

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
