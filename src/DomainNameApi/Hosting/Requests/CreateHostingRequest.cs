namespace DomainNameApi.Hosting;

/// <summary><c>POST hostings/create</c>: orders a hosting for a domain. <b>Charges your balance.</b></summary>
public sealed class CreateHostingRequest : BaseRequest<HostingCreateResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="servicePlan">The service plan, from <see cref="HostingClient.ListPackagesAsync"/>.</param>
    public CreateHostingRequest(string domainName, string servicePlan)
    {
        DomainName = domainName;
        ServicePlan = servicePlan;
    }

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <summary>The service plan.</summary>
    public string ServicePlan { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "hostings/create";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Billable;

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        Require(ServicePlan, nameof(ServicePlan));
    }
}
