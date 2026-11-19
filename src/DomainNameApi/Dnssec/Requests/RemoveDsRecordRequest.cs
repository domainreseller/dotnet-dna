namespace DomainNameApi.Dnssec;

/// <summary><c>DELETE domains/dns/dnssec</c>: removes one DS record.</summary>
public sealed class RemoveDsRecordRequest : BaseRequest<DnssecOperationResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="record">The DS record to remove.</param>
    public RemoveDsRecordRequest(string domainName, DsRecordData record)
    {
        DomainName = domainName;
        Record = record;
    }

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <summary>The DS record to remove.</summary>
    public DsRecordData Record { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Delete;

    /// <inheritdoc />
    public override string Path => "domains/dns/dnssec";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Destructive;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) =>
        query.Add("DomainName", DomainName)
             .Add("KeyTag", Record?.KeyTag)
             .Add("Algorithm", (int?)Record?.Algorithm)
             .Add("DigestType", (int?)Record?.DigestType)
             .Add("Digest", Record?.Digest);

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        Require(Record, nameof(Record));
        Require(Record.Digest, nameof(Record) + ".Digest");
    }
}
