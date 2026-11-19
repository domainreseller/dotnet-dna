namespace DomainNameApi.Hosting;

/// <summary><c>GET hostings/check-status</c>: the provisioning state of a hosting, and the panel access once ready.</summary>
public sealed class GetHostingStatusRequest : BaseRequest<HostingStatus>
{
    /// <summary>Looks the status up by domain name.</summary>
    /// <param name="domainName">The domain.</param>
    public static GetHostingStatusRequest ByDomain(string domainName) => new() { DomainName = domainName };

    /// <summary>Looks the status up by service ID.</summary>
    /// <param name="serviceId">The service ID.</param>
    public static GetHostingStatusRequest ByServiceId(string serviceId) => new() { ServiceId = serviceId };

    /// <summary>The domain.</summary>
    public string? DomainName { get; set; }

    /// <summary>The service ID.</summary>
    public string? ServiceId { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "hostings/check-status";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) =>
        query.Add("ServiceId", ServiceId).Add("DomainName", DomainName);

    /// <inheritdoc />
    protected internal override void Validate()
    {
        if (string.IsNullOrWhiteSpace(DomainName) && string.IsNullOrWhiteSpace(ServiceId))
        {
            throw new ArgumentException("Give a DomainName or a ServiceId.");
        }
    }
}
