namespace DomainNameApi.Dnssec;

/// <summary><c>POST domains/dns/dnssec</c>: publishes DS records at the registry, which turns DNSSEC on.</summary>
public sealed class AddDsRecordsRequest : BaseRequest<ListResult<DnssecOperationResult>>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="records">The DS records from your DNS provider.</param>
    public AddDsRecordsRequest(string domainName, IEnumerable<DsRecordData> records)
    {
        DomainName = domainName;
        Records = records?.ToList() ?? [];
    }

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <summary>The DS records.</summary>
    public List<DsRecordData> Records { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/dns/dnssec";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        Require(Records, nameof(Records));
        if (Records.Any(r => r is null || string.IsNullOrWhiteSpace(r.Digest)))
        {
            throw new ArgumentException("Every DS record needs a digest.", nameof(Records));
        }
    }
}
