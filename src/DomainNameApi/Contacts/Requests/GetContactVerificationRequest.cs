namespace DomainNameApi.Contacts;

/// <summary><c>GET contacts/verification/check</c>: the e-mail and phone verification state of a contact.</summary>
public sealed class GetContactVerificationRequest : BaseRequest<ContactVerificationResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="handle">The contact handle.</param>
    public GetContactVerificationRequest(string handle) => Handle = handle;

    /// <summary>The contact handle.</summary>
    public string Handle { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "contacts/verification/check";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) => query.Add("HandleCode", Handle);

    /// <inheritdoc />
    protected internal override void Validate() => Require(Handle, nameof(Handle));
}
