namespace DomainNameApi.Domains;

/// <summary><c>DELETE domains/dns/host</c>: deletes a child nameserver.</summary>
public sealed class DeleteChildNameServerRequest : BaseRequest
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The parent domain.</param>
    /// <param name="hostName">The nameserver host name.</param>
    public DeleteChildNameServerRequest(string domainName, string hostName)
    {
        DomainName = domainName;
        HostName = hostName;
    }

    /// <summary>The parent domain.</summary>
    public string DomainName { get; set; }

    /// <summary>The nameserver host name.</summary>
    public string HostName { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Delete;

    /// <inheritdoc />
    public override string Path => "domains/dns/host";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Destructive;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) =>
        query.Add("DomainName", DomainName).Add("HostName", HostName);

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        Require(HostName, nameof(HostName));
    }
}
