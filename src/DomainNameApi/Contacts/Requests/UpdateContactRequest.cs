using System.Text.Json.Serialization;

namespace DomainNameApi.Contacts;

/// <summary>
/// <c>PUT contacts/{handleCode}</c>: updates a contact handle. The domains that use it are updated too;
/// the result says what happened to each of them.
/// </summary>
public sealed class UpdateContactRequest : BaseRequest<HandleUpdateResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="handle">The contact handle.</param>
    /// <param name="contact">The new details.</param>
    public UpdateContactRequest(string handle, ContactInfo contact)
    {
        Handle = handle;
        Contact = contact;
    }

    /// <summary>The contact handle.</summary>
    [JsonIgnore]
    public string Handle { get; set; }

    /// <summary>The new details.</summary>
    public ContactInfo Contact { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Put;

    /// <inheritdoc />
    public override string Path => "contacts/" + Uri.EscapeDataString(Handle ?? "");

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(Handle, nameof(Handle));
        Require(Contact, nameof(Contact));
        Contact.EnsureComplete(nameof(Contact));
    }
}
