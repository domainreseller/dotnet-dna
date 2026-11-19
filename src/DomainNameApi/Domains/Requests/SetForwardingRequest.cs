namespace DomainNameApi.Domains;

/// <summary><c>POST domains/forwards</c>: forwards a domain to a URL.</summary>
public sealed class SetForwardingRequest : BaseRequest
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="redirectAddress">The target URL, for example <c>https://www.example.net</c>.</param>
    /// <param name="forwardType">Standard (HTTP 301, the default) or Frame (masked).</param>
    public SetForwardingRequest(string domainName, string redirectAddress, ForwardType? forwardType = null)
    {
        DomainName = domainName;
        RedirectAddress = redirectAddress;
        ForwardType = forwardType ?? Domains.ForwardType.Standard;
    }

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <summary>The target URL.</summary>
    public string RedirectAddress { get; set; }

    /// <summary>Standard (HTTP 301) or Frame (masked).</summary>
    public ForwardType ForwardType { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/forwards";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        Require(RedirectAddress, nameof(RedirectAddress));
    }
}
