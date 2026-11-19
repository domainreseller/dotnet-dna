namespace DomainNameApi.Domains;

/// <summary><c>GET domains</c>: pages through the domains in your account.</summary>
public sealed class ListDomainsRequest : PagedRequest<Domain>
{
    /// <summary>Only domains whose name contains this text.</summary>
    public string? DomainName { get; set; }

    /// <summary>Only domains in these states.</summary>
    public List<DomainStatus> Statuses { get; set; } = [];

    /// <inheritdoc />
    public override string Path => "domains";

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query)
    {
        query.Add("DomainName", DomainName);
        if (Statuses.Count > 0)
        {
            query.AddCommaSeparated("StatusIds", Statuses);
        }
        base.AddQueryParameters(query);
    }
}
