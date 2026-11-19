namespace DomainNameApi.Domains;

/// <summary><c>POST domains/renew</c>: renews a domain. <b>Charges your balance.</b></summary>
public sealed class RenewDomainRequest : BaseRequest<DomainOperationResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain to renew.</param>
    /// <param name="period">The renewal period in years.</param>
    public RenewDomainRequest(string domainName, int period = 1)
    {
        DomainName = domainName;
        Period = period;
    }

    /// <summary>The domain to renew.</summary>
    public string DomainName { get; set; }

    /// <summary>The renewal period in years.</summary>
    public int Period { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/renew";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Billable;

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        if (Period < 1)
        {
            throw new ArgumentException("Period must be at least 1 year.", nameof(Period));
        }
    }
}
