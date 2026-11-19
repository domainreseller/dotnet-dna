namespace DomainNameApi.Dns;

/// <summary><c>DELETE domains/zones</c>: removes one value from a record set.</summary>
public sealed class DeleteDnsRecordRequest : BaseRequest
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="name">The record name.</param>
    /// <param name="type">The record type.</param>
    /// <param name="value">The value to remove.</param>
    public DeleteDnsRecordRequest(string domainName, string name, RecordType type, string value)
    {
        DomainName = domainName;
        Name = name;
        Type = type;
        Value = value;
    }

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <summary>The record name.</summary>
    public string Name { get; set; }

    /// <summary>The record type.</summary>
    public RecordType Type { get; set; }

    /// <summary>The value to remove.</summary>
    public string Value { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Delete;

    /// <inheritdoc />
    public override string Path => "domains/zones";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Destructive;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) =>
        query.Add("domainName", DomainName).Add("Name", Name).Add("Record", Value).Add("RecordType", Type.Value);

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        Require(Name, nameof(Name));
        Require(Type.Value, nameof(Type));
        Require(Value, nameof(Value));
    }
}
