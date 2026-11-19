namespace DomainNameApi.Domains;

/// <summary><c>PUT domains/dns/host</c>: changes the name or IP addresses of a child nameserver.</summary>
public sealed class UpdateChildNameServerRequest : BaseRequest
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The parent domain.</param>
    /// <param name="hostName">The current nameserver host name.</param>
    /// <param name="ipAddresses">The new IP addresses.</param>
    public UpdateChildNameServerRequest(string domainName, string hostName, IEnumerable<HostIpAddress> ipAddresses)
    {
        DomainName = domainName;
        HostName = hostName;
        IpAddresses = ipAddresses?.ToList() ?? [];
    }

    /// <summary>The parent domain.</summary>
    public string DomainName { get; set; }

    /// <summary>The current nameserver host name.</summary>
    public string HostName { get; set; }

    /// <summary>A new host name, or null to keep it.</summary>
    public string? NewHostName { get; set; }

    /// <summary>The new IP addresses.</summary>
    public List<HostIpAddress> IpAddresses { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Put;

    /// <inheritdoc />
    public override string Path => "domains/dns/host";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        Require(HostName, nameof(HostName));
        Require(IpAddresses, nameof(IpAddresses));
    }
}
