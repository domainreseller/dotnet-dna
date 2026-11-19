namespace DomainNameApi.Domains;

/// <summary><c>POST domains/renew/check</c>: whether a domain can be renewed, and its premium price if any. Charges nothing.</summary>
public sealed class CheckRenewalRequest : BaseRequest<RenewalCheck>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="period">The renewal period in years to check.</param>
    public CheckRenewalRequest(string domainName, int period = 1)
    {
        DomainName = domainName;
        Period = period;
    }

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <summary>The renewal period in years.</summary>
    public int Period { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/renew/check";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
