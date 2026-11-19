namespace DomainNameApi.Hosting;

/// <summary>Web hosting packages and services. Reach it through <see cref="DomainNameApiClient.Hosting"/>.</summary>
public sealed class HostingClient
{
    private readonly DomainNameApiClient _api;

    internal HostingClient(DomainNameApiClient api) => _api = api;

    /// <summary>Returns the hosting packages and their service plans.</summary>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<IReadOnlyList<HostingPackage>> ListPackagesAsync(CancellationToken cancellationToken = default) =>
        _api.SendAsync(new ListHostingPackagesRequest(), cancellationToken);

    /// <summary>Returns the service plans and their limits.</summary>
    /// <param name="hostingId">Only the plans available to this hosting; null for all.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<IReadOnlyList<ServicePlan>> ListPlansAsync(Guid? hostingId = null, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new ListHostingPlansRequest { HostingId = hostingId }, cancellationToken);

    /// <summary>Orders a hosting for a domain. <b>Charges your balance.</b> Poll <see cref="GetStatusAsync"/> until it is ready.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="servicePlan">The service plan, from <see cref="ListPackagesAsync"/>.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<HostingCreateResult> CreateAsync(string domainName, string servicePlan, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new CreateHostingRequest(domainName, servicePlan), cancellationToken);

    /// <summary>Returns the hosting of a domain.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<HostingService> GetAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(GetHostingRequest.ByDomain(domainName), cancellationToken);

    /// <summary>Returns a hosting by its service ID.</summary>
    /// <param name="serviceId">The service ID.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<HostingService> GetByServiceIdAsync(string serviceId, CancellationToken cancellationToken = default) =>
        _api.SendAsync(GetHostingRequest.ByServiceId(serviceId), cancellationToken);

    /// <summary>Returns a hosting by the token returned when it was ordered.</summary>
    /// <param name="token">The token from <see cref="HostingCreateResult.Token"/>.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<HostingService> GetByTokenAsync(string token, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new GetHostingByTokenRequest(token), cancellationToken);

    /// <summary>Returns the provisioning state of a domain's hosting, and the panel access once ready.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<HostingStatus> GetStatusAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(GetHostingStatusRequest.ByDomain(domainName), cancellationToken);

    /// <summary>Returns the provisioning state of a hosting by its service ID.</summary>
    /// <param name="serviceId">The service ID.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<HostingStatus> GetStatusByServiceIdAsync(string serviceId, CancellationToken cancellationToken = default) =>
        _api.SendAsync(GetHostingStatusRequest.ByServiceId(serviceId), cancellationToken);
}
