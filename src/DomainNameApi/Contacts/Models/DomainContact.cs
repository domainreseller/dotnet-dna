namespace DomainNameApi.Contacts;

/// <summary>A contact together with the role it plays for a domain.</summary>
public sealed class DomainContact : ContactInfo
{
    /// <summary>Creates an empty contact.</summary>
    public DomainContact()
    {
    }

    /// <summary>Uses the details of <paramref name="contact"/> for the given role.</summary>
    /// <param name="contact">The contact details.</param>
    /// <param name="contactType">The role.</param>
    public DomainContact(ContactInfo contact, ContactType contactType)
        : base(contact)
    {
        ContactType = contactType;
    }

    /// <summary>The role: Registrant, Admin, Tech or Billing.</summary>
    public ContactType ContactType { get; set; }

    /// <summary>Uses the same details for all four roles (Registrant, Admin, Tech and Billing).</summary>
    /// <param name="contact">The contact details.</param>
    public static List<DomainContact> ForAllRoles(ContactInfo contact) =>
        ContactType.All.Select(role => new DomainContact(contact, role)).ToList();
}
