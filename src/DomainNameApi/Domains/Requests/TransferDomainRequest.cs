using DomainNameApi.Contacts;

namespace DomainNameApi.Domains;

/// <summary>
/// <c>POST domains/transfer</c>: transfers a domain in from another registrar. <b>Charges your balance.</b>
/// Check it first with <see cref="CheckTransferRequest"/>.
/// </summary>
public sealed class TransferDomainRequest : BaseRequest<DomainOperationResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain to transfer.</param>
    /// <param name="authCode">The transfer (EPP) code from the current registrar.</param>
    /// <param name="period">Years added by the transfer.</param>
    public TransferDomainRequest(string domainName, string? authCode, int period = 1)
    {
        DomainName = domainName;
        AuthCode = authCode;
        Period = period;
    }

    /// <summary>The domain to transfer.</summary>
    public string DomainName { get; set; }

    /// <summary>The transfer (EPP) code. Some registries do not use one.</summary>
    public string? AuthCode { get; set; }

    /// <summary>Years added by the transfer.</summary>
    public int Period { get; set; }

    /// <summary>Contacts to set after the transfer. Usually left empty: the registrar takes the current ones over.</summary>
    public List<ContactInfo>? Contacts { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/transfer";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Billable;

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
