namespace DomainNameApi.Dnssec;

/// <summary>DNSSEC (DS records) of domains. Reach it through <see cref="DomainNameApiClient.Dnssec"/>.</summary>
public sealed class DnssecClient
{
    private readonly DomainNameApiClient _api;

    internal DnssecClient(DomainNameApiClient api) => _api = api;

    /// <summary>Returns the DNSSEC state, DS records and the algorithms the registry allows.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<DnssecInfo> GetAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new GetDnssecRequest(domainName), cancellationToken);

    /// <summary>Publishes a DS record, which turns DNSSEC on.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="record">The DS record from your DNS provider. <see cref="DsRecordData.Parse"/> reads the text form.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<IReadOnlyList<DnssecOperationResult>> AddAsync(string domainName, DsRecordData record, CancellationToken cancellationToken = default) =>
        AddAsync(domainName, [record], cancellationToken);

    /// <summary>Publishes DS records, which turns DNSSEC on.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="records">The DS records from your DNS provider.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public async Task<IReadOnlyList<DnssecOperationResult>> AddAsync(string domainName, IEnumerable<DsRecordData> records, CancellationToken cancellationToken = default) =>
        (await _api.SendAsync(new AddDsRecordsRequest(domainName, records), cancellationToken).ConfigureAwait(false)).Items;

    /// <summary>Removes one DS record.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="record">The DS record to remove.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<DnssecOperationResult> RemoveAsync(string domainName, DsRecordData record, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new RemoveDsRecordRequest(domainName, record), cancellationToken);

    /// <summary>Removes every DS record, which turns DNSSEC off.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<DnssecOperationResult> DisableAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new DisableDnssecRequest(domainName), cancellationToken);
}
