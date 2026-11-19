namespace DomainNameApi.Contacts;

/// <summary>A contact handle assigned to a role, used to register a domain or to change its contacts.</summary>
public sealed record ContactHandle
{
    /// <summary>Creates an empty assignment.</summary>
    public ContactHandle()
    {
    }

    /// <summary>Assigns a handle to a role.</summary>
    /// <param name="contactType">The role.</param>
    /// <param name="handle">The contact handle, for example <c>DNA-12345</c>.</param>
    public ContactHandle(ContactType contactType, string handle)
    {
        ContactType = contactType;
        Handle = handle;
    }

    /// <summary>The role.</summary>
    public ContactType ContactType { get; init; }

    /// <summary>The contact handle.</summary>
    public string? Handle { get; init; }

    /// <summary>Uses the same handle for all four roles.</summary>
    /// <param name="handle">The contact handle.</param>
    public static List<ContactHandle> ForAllRoles(string handle) =>
        ContactType.All.Select(role => new ContactHandle(role, handle)).ToList();
}
