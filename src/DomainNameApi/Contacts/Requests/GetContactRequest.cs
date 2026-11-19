namespace DomainNameApi.Contacts;

/// <summary><c>GET contacts/{handleCode}</c>: a contact handle and the state of its verification.</summary>
public sealed class GetContactRequest : BaseRequest<ContactDetails>
{
    /// <summary>Creates the request.</summary>
    /// <param name="handle">The contact handle, for example <c>DNA-12345</c>.</param>
    public GetContactRequest(string handle) => Handle = handle;

    /// <summary>The contact handle.</summary>
    public string Handle { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "contacts/" + Uri.EscapeDataString(Handle ?? "");

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void Validate() => Require(Handle, nameof(Handle));
}
