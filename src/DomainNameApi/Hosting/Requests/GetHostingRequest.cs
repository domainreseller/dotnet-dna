namespace DomainNameApi.Hosting;

/// <summary><c>GET hostings</c>: a hosting, looked up by domain name or service ID.</summary>
public sealed class GetHostingRequest : BaseRequest<HostingService>
{
    /// <summary>Looks a hosting up by domain name.</summary>
    /// <param name="domainName">The domain.</param>
    public static GetHostingRequest ByDomain(string domainName) => new() { DomainName = domainName };

    /// <summary>Looks a hosting up by service ID.</summary>
    /// <param name="serviceId">The service ID.</param>
    public static GetHostingRequest ByServiceId(string serviceId) => new() { ServiceId = serviceId };

    /// <summary>The domain.</summary>
    public string? DomainName { get; set; }

    /// <summary>The service ID.</summary>
    public string? ServiceId { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "hostings";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) =>
        query.Add("serviceId", ServiceId).Add("domainName", DomainName);

    /// <inheritdoc />
    protected internal override void Validate()
    {
        if (string.IsNullOrWhiteSpace(DomainName) && string.IsNullOrWhiteSpace(ServiceId))
        {
            throw new ArgumentException("Give a DomainName or a ServiceId.");
        }
    }
}
