namespace DomainNameApi.Contacts;

/// <summary><c>POST contacts</c>: creates a reusable contact handle.</summary>
public sealed class CreateContactRequest : BaseRequest<CreateContactResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="contact">The contact details.</param>
    public CreateContactRequest(ContactInfo contact) => Contact = contact;

    /// <summary>The contact details.</summary>
    public ContactInfo Contact { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "contacts";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(Contact, nameof(Contact));
        Contact.EnsureComplete(nameof(Contact));
    }
}
