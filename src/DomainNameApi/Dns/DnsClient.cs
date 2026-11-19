namespace DomainNameApi.Dns;

/// <summary>
/// DNS zone records of domains. Records resolve publicly once the domain uses the platform nameservers;
/// <see cref="ApplyAsync"/> switches it to them. Zone calls are kept 1 second apart (the gateway's limit).
/// Reach it through <see cref="DomainNameApiClient.Dns"/>.
/// </summary>
public sealed class DnsClient
{
    private readonly DomainNameApiClient _api;

    internal DnsClient(DomainNameApiClient api) => _api = api;

    /// <summary>Returns the record sets of a domain.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<IReadOnlyList<DnsRecordSet>> ListAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new ListDnsRecordsRequest(domainName), cancellationToken);

    /// <summary>Adds a value to a record set, creating the set if needed. Existing values are kept.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="name">The record name: <c>@</c> for the apex, <c>www</c>, or a full name.</param>
    /// <param name="type">The record type, for example <see cref="RecordType.A"/>.</param>
    /// <param name="value">The value, for example <c>192.0.2.10</c>.</param>
    /// <param name="ttl">Time to live in seconds (1–86400).</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task AddAsync(string domainName, string name, RecordType type, string value, int ttl = 3600, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new AddDnsRecordRequest(domainName, name, type, [value], ttl), cancellationToken);

    /// <summary>Adds values to a record set, creating the set if needed. Existing values are kept.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="name">The record name.</param>
    /// <param name="type">The record type.</param>
    /// <param name="values">The values to add.</param>
    /// <param name="ttl">Time to live in seconds (1–86400).</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task AddAsync(string domainName, string name, RecordType type, IEnumerable<string> values, int ttl = 3600, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new AddDnsRecordRequest(domainName, name, type, values, ttl), cancellationToken);

    /// <summary>Replaces every value of a record set, and optionally renames it.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="name">The name of the existing record set.</param>
    /// <param name="type">The record type.</param>
    /// <param name="values">The new values.</param>
    /// <param name="ttl">Time to live in seconds (1–86400).</param>
    /// <param name="newName">A new name for the set, or null to keep it.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task ReplaceAsync(
        string domainName,
        string name,
        RecordType type,
        IEnumerable<string> values,
        int ttl = 3600,
        string? newName = null,
        CancellationToken cancellationToken = default) =>
        _api.SendAsync(new ReplaceDnsRecordSetRequest(domainName, name, type, values, ttl) { NewName = newName }, cancellationToken);

    /// <summary>Removes one value from a record set.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="name">The record name.</param>
    /// <param name="type">The record type.</param>
    /// <param name="value">The value to remove.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task DeleteAsync(string domainName, string name, RecordType type, string value, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new DeleteDnsRecordRequest(domainName, name, type, value), cancellationToken);

    /// <summary>Switches the domain to the platform nameservers so its zone is served.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<ZoneApplyResult> ApplyAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new ApplyDnsZoneRequest(domainName), cancellationToken);
}
