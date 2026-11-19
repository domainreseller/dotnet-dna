namespace DomainNameApi.Domains;

/// <summary>URL forwarding of domains. Reach it through <see cref="DomainsClient.Forwarding"/>.</summary>
public sealed class DomainForwardingClient
{
    private readonly DomainNameApiClient _api;

    internal DomainForwardingClient(DomainNameApiClient api) => _api = api;

    /// <summary>Returns the URL forwarding of a domain.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <exception cref="DomainNameApiException">With <see cref="ApiErrorKind.NotFound"/> when no forwarding is set up.</exception>
    public Task<DomainForwarding> GetAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new GetForwardingRequest(domainName), cancellationToken);

    /// <summary>Forwards a domain to a URL.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="redirectAddress">The target URL, for example <c>https://www.example.net</c>.</param>
    /// <param name="forwardType">Standard (HTTP 301, the default) or Frame (masked).</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task SetAsync(string domainName, string redirectAddress, ForwardType? forwardType = null, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new SetForwardingRequest(domainName, redirectAddress, forwardType), cancellationToken);

    /// <summary>Removes the URL forwarding of a domain.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task DeleteAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new DeleteForwardingRequest(domainName), cancellationToken);
}
