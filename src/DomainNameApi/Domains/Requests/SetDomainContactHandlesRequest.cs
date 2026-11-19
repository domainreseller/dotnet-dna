using System.Text.Json.Serialization;
using DomainNameApi.Contacts;

namespace DomainNameApi.Domains;

/// <summary><c>PUT domains/contacts/update-with-handle</c>: points the contact roles of a domain at existing contact handles.</summary>
public sealed class SetDomainContactHandlesRequest : BaseRequest
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="handles">The handles, one per role you want to change.</param>
    public SetDomainContactHandlesRequest(string domainName, IEnumerable<ContactHandle> handles)
    {
        DomainName = domainName;
        Handles = handles?.ToList() ?? [];
    }

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <summary>The handles, one per role.</summary>
    [JsonPropertyName("handleItems")]
    public List<ContactHandle> Handles { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Put;

    /// <inheritdoc />
    public override string Path => "domains/contacts/update-with-handle";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        Require(Handles, nameof(Handles));
    }
}
