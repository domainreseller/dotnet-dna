using System.Text.Json.Serialization;

namespace DomainNameApi.Domains;

/// <summary><c>POST domains/privacy</c>: turns WHOIS privacy protection on or off.</summary>
public sealed class SetPrivacyProtectionRequest : BaseRequest
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="enabled">True to hide the contacts in WHOIS, false to show them.</param>
    public SetPrivacyProtectionRequest(string domainName, bool enabled)
    {
        DomainName = domainName;
        Enabled = enabled;
    }

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <summary>Whether privacy protection is on.</summary>
    [JsonPropertyName("privacyStatus")]
    public bool Enabled { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/privacy";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
