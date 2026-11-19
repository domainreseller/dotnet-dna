using System.Text.Json.Serialization;

namespace DomainNameApi.Contacts;

/// <summary><c>POST contacts/verification/request</c>: sends the contact verification notice again.</summary>
public sealed class ResendContactVerificationRequest : BaseRequest
{
    /// <summary>Creates the request.</summary>
    /// <param name="handle">The contact handle.</param>
    public ResendContactVerificationRequest(string handle) => Handle = handle;

    /// <summary>The contact handle.</summary>
    [JsonPropertyName("handleCode")]
    public string Handle { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "contacts/verification/request";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    protected internal override void Validate() => Require(Handle, nameof(Handle));
}
