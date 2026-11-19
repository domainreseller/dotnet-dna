using System.Text.Json.Serialization;

namespace DomainNameApi.Dns;

/// <summary>
/// <c>PUT domains/zones</c>: replaces every value of a record set (and its TTL). Set <see cref="NewName"/> to rename it.
/// Use <see cref="AddDnsRecordRequest"/> to append a value instead.
/// </summary>
public sealed class ReplaceDnsRecordSetRequest : BaseRequest
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="recordName">The name of the existing record set.</param>
    /// <param name="type">The record type.</param>
    /// <param name="contents">The new values.</param>
    /// <param name="ttl">Time to live in seconds (1–86400).</param>
    public ReplaceDnsRecordSetRequest(string domainName, string recordName, RecordType type, IEnumerable<string> contents, int ttl = 3600)
    {
        DomainName = domainName;
        RecordName = recordName;
        Type = type;
        Contents = contents?.ToList() ?? [];
        Ttl = ttl;
    }

    /// <summary>The domain.</summary>
    [JsonIgnore]
    public string DomainName { get; set; }

    /// <summary>The name of the existing record set.</summary>
    [JsonIgnore]
    public string RecordName { get; set; }

    /// <summary>A new name for the record set, or null to keep <see cref="RecordName"/>.</summary>
    public string? NewName { get; set; }

    /// <summary>The record type.</summary>
    public RecordType Type { get; set; }

    /// <summary>The new values.</summary>
    public List<string> Contents { get; set; }

    /// <summary>Time to live in seconds.</summary>
    public int Ttl { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Put;

    /// <inheritdoc />
    public override string Path => "domains/zones";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    public override bool IsIdempotent => true;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) =>
        query.Add("domainName", DomainName).Add("recordName", RecordName);

    /// <inheritdoc />
    protected internal override object GetBody() => new ZoneRecordBody(NewName ?? RecordName, Type, Ttl, Contents);

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        Require(RecordName, nameof(RecordName));
        Require(Type.Value, nameof(Type));
        Require(Contents, nameof(Contents));
        DnsValidation.Check(NewName ?? RecordName, Type, Ttl, Contents);
    }
}
