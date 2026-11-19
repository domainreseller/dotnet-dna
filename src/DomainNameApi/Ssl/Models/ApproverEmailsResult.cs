using System.Text.Json.Serialization;

namespace DomainNameApi.Ssl;

/// <summary>The answer of <see cref="GetApproverEmailsRequest"/>.</summary>
public sealed record ApproverEmailsResult
{
    /// <summary>The e-mail addresses allowed for e-mail validation.</summary>
    [JsonPropertyName("mails")]
    public IReadOnlyList<string> Emails { get; init; } = [];
}
