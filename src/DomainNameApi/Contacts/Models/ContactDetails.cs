using System.Text.Json.Serialization;

namespace DomainNameApi.Contacts;

/// <summary>A contact handle and the state of its e-mail and phone verification.</summary>
public sealed record ContactDetails
{
    /// <summary>The contact.</summary>
    public Contact? Contact { get; init; }

    /// <summary>The verification records of the contact's e-mail address and phone number.</summary>
    [JsonPropertyName("verificationObjects")]
    public IReadOnlyList<ContactVerification> Verifications { get; init; } = [];
}
