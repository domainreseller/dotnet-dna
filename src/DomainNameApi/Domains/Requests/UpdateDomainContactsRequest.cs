using DomainNameApi.Contacts;

namespace DomainNameApi.Domains;

/// <summary><c>PUT domains/contacts/update</c>: replaces the contacts of a domain with details given inline.</summary>
public sealed class UpdateDomainContactsRequest : BaseRequest
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="contacts">The contacts, one per role.</param>
    public UpdateDomainContactsRequest(string domainName, IEnumerable<DomainContact> contacts)
    {
        DomainName = domainName;
        Contacts = contacts?.ToList() ?? [];
    }

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <summary>The contacts, one per role.</summary>
    public List<DomainContact> Contacts { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Put;

    /// <inheritdoc />
    public override string Path => "domains/contacts/update";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        Require(Contacts, nameof(Contacts));
        foreach (var contact in Contacts)
        {
            Require(contact, nameof(Contacts));
            contact.EnsureComplete(nameof(Contacts) + "[" + contact.ContactType + "]");
        }
    }
}
