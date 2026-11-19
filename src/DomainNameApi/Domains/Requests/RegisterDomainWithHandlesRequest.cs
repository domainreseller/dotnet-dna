using DomainNameApi.Contacts;

namespace DomainNameApi.Domains;

/// <summary>
/// <c>POST domains/register</c>: registers a domain using existing contact handles.
/// <b>Charges your balance.</b> Create handles with <see cref="ContactsClient.CreateAsync"/>.
/// </summary>
public sealed class RegisterDomainWithHandlesRequest : BaseRequest<DomainOperationResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain to register.</param>
    public RegisterDomainWithHandlesRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain to register.</summary>
    public string DomainName { get; set; }

    /// <summary>The registration period in years. 1 by default.</summary>
    public int Period { get; set; } = 1;

    /// <summary>The nameservers. Most registries need at least two.</summary>
    public List<string> NameServers { get; set; } = [];

    /// <summary>The contact handles, one per role. <see cref="ContactHandle.ForAllRoles"/> uses one handle for all four.</summary>
    public List<ContactHandle> Handles { get; set; } = [];

    /// <summary>Registry-specific attributes, such as the ones .tr domains need.</summary>
    public Dictionary<string, string>? TldAttributes { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/register";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Billable;

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        if (Period < 1)
        {
            throw new ArgumentException("Period must be at least 1 year.", nameof(Period));
        }
        Require(NameServers, nameof(NameServers));
        Require(Handles, nameof(Handles));
        if (Handles.Any(h => h is null || string.IsNullOrWhiteSpace(h.Handle)))
        {
            throw new ArgumentException("Every entry in Handles needs a handle.", nameof(Handles));
        }
    }
}
