namespace DomainNameApi.Contacts;

/// <summary>A stored contact: its details, its role and its handle.</summary>
public sealed class Contact : ContactInfo
{
    /// <summary>The role, when the contact is read from a domain.</summary>
    public ContactType? ContactType { get; set; }

    /// <summary>The contact handle, for example <c>DNA-12345</c>.</summary>
    public string? Handle { get; set; }
}
