namespace DomainNameApi.Domains;

/// <summary><c>POST domains/dns/host</c>: creates a child nameserver (glue record) such as ns1.example.com.</summary>
public sealed class AddChildNameServerRequest : BaseRequest
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The parent domain, for example <c>example.com</c>.</param>
    /// <param name="hostName">The nameserver, for example <c>ns1.example.com</c>.</param>
    /// <param name="ipAddresses">Its IP addresses.</param>
    public AddChildNameServerRequest(string domainName, string hostName, IEnumerable<HostIpAddress> ipAddresses)
    {
        DomainName = domainName;
        HostName = hostName;
        IpAddresses = ipAddresses?.ToList() ?? [];
    }

    /// <summary>The parent domain.</summary>
    public string DomainName { get; set; }

    /// <summary>The nameserver host name.</summary>
    public string HostName { get; set; }

    /// <summary>Its IP addresses. <see cref="HostIpAddress.Parse"/> detects the IP version.</summary>
    public List<HostIpAddress> IpAddresses { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

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
