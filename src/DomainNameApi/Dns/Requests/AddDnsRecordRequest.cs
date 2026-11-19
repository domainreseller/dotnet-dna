using System.Text.Json.Serialization;

namespace DomainNameApi.Dns;

/// <summary>
/// <c>POST domains/zones</c>: adds values to a record set and creates the set if needed. Existing values of the
/// set are kept. Records resolve publicly once the domain uses the platform nameservers
/// (see <see cref="ApplyDnsZoneRequest"/>).
/// </summary>
public sealed class AddDnsRecordRequest : BaseRequest
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="name">The record name: <c>@</c> for the apex, <c>www</c>, or a full name.</param>
    /// <param name="type">The record type.</param>
    /// <param name="contents">The values to add.</param>
    /// <param name="ttl">Time to live in seconds (1–86400).</param>
    public AddDnsRecordRequest(string domainName, string name, RecordType type, IEnumerable<string> contents, int ttl = 3600)
    {
        DomainName = domainName;
        Name = name;
        Type = type;
        Contents = contents?.ToList() ?? [];
        Ttl = ttl;
    }

    /// <summary>The domain.</summary>
    [JsonIgnore]
    public string DomainName { get; set; }

    /// <summary>The record name.</summary>
    public string Name { get; set; }

    /// <summary>The record type.</summary>
    public RecordType Type { get; set; }

    /// <summary>The values to add.</summary>
    public List<string> Contents { get; set; }

    /// <summary>Time to live in seconds.</summary>
    public int Ttl { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/zones";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) => query.Add("domainName", DomainName);

    /// <inheritdoc />
    protected internal override object GetBody() => new ZoneRecordBody(Name, Type, Ttl, Contents);

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        Require(Type.Value, nameof(Type));
        Require(Contents, nameof(Contents));
        DnsValidation.Check(Name, Type, Ttl, Contents);
    }
}
