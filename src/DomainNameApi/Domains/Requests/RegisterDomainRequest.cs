using DomainNameApi.Contacts;

namespace DomainNameApi.Domains;

/// <summary>
/// <c>POST domains/register-with-contacts</c>: registers a domain with contact details given inline.
/// <b>Charges your balance.</b> Use <see cref="RegisterDomainWithHandlesRequest"/> to reuse existing contact handles.
/// </summary>
/// <example>
/// <code>
/// var request = new RegisterDomainRequest("example.com")
/// {
///     Period = 1,
///     NameServers = { "ns1.example.net", "ns2.example.net" },
///     Contacts = DomainContact.ForAllRoles(contact),
/// };
/// var result = await client.Domains.RegisterAsync(request);
/// </code>
/// </example>
public sealed class RegisterDomainRequest : BaseRequest<DomainOperationResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain to register.</param>
    public RegisterDomainRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain to register.</summary>
    public string DomainName { get; set; }

    /// <summary>The registration period in years. 1 by default.</summary>
    public int Period { get; set; } = 1;

    /// <summary>The nameservers. Most registries need at least two.</summary>
    public List<string> NameServers { get; set; } = [];

    /// <summary>
    /// The contacts, one per role. All four roles (Registrant, Admin, Tech, Billing) are needed unless
    /// <see cref="UseTrusteeContact"/> is set. <see cref="DomainContact.ForAllRoles"/> fills all four from one contact.
    /// </summary>
    public List<DomainContact> Contacts { get; set; } = [];

    /// <summary>Registry-specific attributes, such as the ones .tr domains need. See the TLD list for each TLD's attributes.</summary>
    public Dictionary<string, string>? TldAttributes { get; set; }

    /// <summary>Use the registry's trustee (local presence) service.</summary>
    public bool UseTrusteeContact { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/register-with-contacts";

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
        Require(Contacts, nameof(Contacts));
        foreach (var contact in Contacts)
        {
            Require(contact, nameof(Contacts));
            contact.EnsureComplete(nameof(Contacts) + "[" + contact.ContactType + "]");
        }
        if (!UseTrusteeContact)
        {
            var missing = ContactType.All.Where(role => Contacts.All(c => c.ContactType != role)).ToList();
            if (missing.Count > 0)
            {
                throw new ArgumentException(
                    "Contacts is missing the roles " + string.Join(", ", missing) +
                    ". Give all four roles (DomainContact.ForAllRoles helps) or set UseTrusteeContact.",
                    nameof(Contacts));
            }
        }
    }
}
