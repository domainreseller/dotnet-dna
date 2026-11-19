namespace DomainNameApi.Dns;

/// <summary>All values of one name and type in a DNS zone, for example every A record of <c>www</c>.</summary>
public sealed record DnsRecordSet
{
    /// <summary>The record name: <c>@</c> for the apex, a relative name such as <c>www</c>, or a full name.</summary>
    public string? Name { get; init; }

    /// <summary>The record type.</summary>
    public RecordType Type { get; init; }

    /// <summary>Time to live in seconds.</summary>
    public int Ttl { get; init; }

    /// <summary>A pending change, if the zone has not been published yet.</summary>
    public string? ChangeType { get; init; }

    /// <summary>The values.</summary>
    public IReadOnlyList<DnsRecord> Records { get; init; } = [];

    /// <summary>The contents of the enabled records.</summary>
    public IEnumerable<string> Values => Records.Where(r => !r.IsDisabled && r.Content is not null).Select(r => r.Content!);
}
