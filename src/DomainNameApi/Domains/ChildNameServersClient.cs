namespace DomainNameApi.Domains;

/// <summary>
/// Child nameservers (glue records) such as ns1.example.com for example.com.
/// Reach it through <see cref="DomainsClient.ChildNameServers"/>. The current ones are listed in
/// <see cref="DomainDetails.Hosts"/>.
/// </summary>
public sealed class ChildNameServersClient
{
    private readonly DomainNameApiClient _api;

    internal ChildNameServersClient(DomainNameApiClient api) => _api = api;

    /// <summary>Creates a child nameserver. The IP version of each address is detected.</summary>
    /// <param name="domainName">The parent domain, for example <c>example.com</c>.</param>
    /// <param name="hostName">The nameserver, for example <c>ns1.example.com</c>.</param>
    /// <param name="ipAddresses">Its IPv4 and IPv6 addresses.</param>
    /// <exception cref="ArgumentException">An address is not a valid IP address.</exception>
    public Task AddAsync(string domainName, string hostName, params string[] ipAddresses) =>
        AddAsync(domainName, hostName, ipAddresses, CancellationToken.None);

    /// <summary>Creates a child nameserver. The IP version of each address is detected.</summary>
    /// <param name="domainName">The parent domain, for example <c>example.com</c>.</param>
    /// <param name="hostName">The nameserver, for example <c>ns1.example.com</c>.</param>
    /// <param name="ipAddresses">Its IPv4 and IPv6 addresses.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <exception cref="ArgumentException">An address is not a valid IP address.</exception>
    public Task AddAsync(string domainName, string hostName, IEnumerable<string> ipAddresses, CancellationToken cancellationToken) =>
        _api.SendAsync(new AddChildNameServerRequest(domainName, hostName, Parse(ipAddresses)), cancellationToken);

    /// <summary>Replaces the IP addresses of a child nameserver, and optionally renames it.</summary>
    /// <param name="domainName">The parent domain.</param>
    /// <param name="hostName">The current nameserver host name.</param>
    /// <param name="ipAddresses">The new IPv4 and IPv6 addresses.</param>
    /// <param name="newHostName">A new host name, or null to keep it.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task UpdateAsync(
        string domainName,
        string hostName,
        IEnumerable<string> ipAddresses,
        string? newHostName = null,
        CancellationToken cancellationToken = default) =>
        _api.SendAsync(
            new UpdateChildNameServerRequest(domainName, hostName, Parse(ipAddresses)) { NewHostName = newHostName },
            cancellationToken);

    /// <summary>Deletes a child nameserver.</summary>
    /// <param name="domainName">The parent domain.</param>
    /// <param name="hostName">The nameserver host name.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task DeleteAsync(string domainName, string hostName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new DeleteChildNameServerRequest(domainName, hostName), cancellationToken);

    private static List<HostIpAddress> Parse(IEnumerable<string> ipAddresses) =>
        (ipAddresses ?? []).Select(HostIpAddress.Parse).ToList();
}
