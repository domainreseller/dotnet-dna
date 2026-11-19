using DomainNameApi.Contacts;

namespace DomainNameApi.Domains;

/// <summary>
/// Search, register, renew, transfer and manage domains. Reach it through <see cref="DomainNameApiClient.Domains"/>.
/// URL forwarding, child nameservers and registry documents have their own clients:
/// <see cref="Forwarding"/>, <see cref="ChildNameServers"/> and <see cref="Documents"/>.
/// </summary>
public sealed class DomainsClient
{
    private readonly DomainNameApiClient _api;

    internal DomainsClient(DomainNameApiClient api)
    {
        _api = api;
        Forwarding = new DomainForwardingClient(api);
        ChildNameServers = new ChildNameServersClient(api);
        Documents = new DomainDocumentsClient(api);
    }

    /// <summary>URL forwarding of domains.</summary>
    public DomainForwardingClient Forwarding { get; }

    /// <summary>Child nameservers (glue records) such as ns1.example.com.</summary>
    public ChildNameServersClient ChildNameServers { get; }

    /// <summary>Registry documents (mostly for .tr domains).</summary>
    public DomainDocumentsClient Documents { get; }

    // ---------------------------------------------------------------- search

    /// <summary>Checks whether a domain can be registered and returns its price.</summary>
    /// <param name="domainName">The domain, for example <c>example.com</c>.</param>
    /// <param name="currency">The price currency, for example <c>USD</c> or <c>TRY</c>; null for the account default.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<DomainSearchResult> SearchAsync(string domainName, string? currency = null, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new SearchDomainRequest(domainName, currency), cancellationToken);

    /// <summary>Checks several domains in one call.</summary>
    /// <param name="domainNames">The domains, for example <c>example.com</c> and <c>example.net</c>.</param>
    /// <param name="currency">The price currency; null for the account default.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<BulkSearchResult> BulkSearchAsync(IEnumerable<string> domainNames, string? currency = null, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new BulkSearchDomainsRequest(domainNames, currency), cancellationToken);

    // ---------------------------------------------------------------- list and details

    /// <summary>Returns one page of the domains in your account.</summary>
    /// <param name="request">Filters and paging; null for the first page.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<PagedResult<Domain>> ListAsync(ListDomainsRequest? request = null, CancellationToken cancellationToken = default) =>
        _api.SendAsync(request ?? new ListDomainsRequest(), cancellationToken);

    /// <summary>Yields every domain in your account that matches the filters, page by page.</summary>
    /// <param name="request">Filters; null for all domains.</param>
    /// <param name="cancellationToken">Stops the enumeration.</param>
    public IAsyncEnumerable<Domain> ListAllAsync(ListDomainsRequest? request = null, CancellationToken cancellationToken = default) =>
        _api.ListAllAsync(request ?? new ListDomainsRequest(), cancellationToken);

    /// <summary>Returns the full details of a domain.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<DomainDetails> GetAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new GetDomainRequest(domainName), cancellationToken);

    // ---------------------------------------------------------------- register, renew, transfer, restore

    /// <summary>Registers a domain with contact details given inline. <b>Charges your balance.</b></summary>
    /// <param name="request">The registration.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<DomainOperationResult> RegisterAsync(RegisterDomainRequest request, CancellationToken cancellationToken = default) =>
        _api.SendAsync(request, cancellationToken);

    /// <summary>
    /// Registers a domain and uses <paramref name="contact"/> for all four contact roles. <b>Charges your balance.</b>
    /// </summary>
    /// <param name="domainName">The domain to register.</param>
    /// <param name="contact">The contact used as registrant, admin, tech and billing contact.</param>
    /// <param name="nameServers">The nameservers; most registries need at least two.</param>
    /// <param name="years">The registration period in years.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<DomainOperationResult> RegisterAsync(
        string domainName,
        ContactInfo contact,
        IEnumerable<string> nameServers,
        int years = 1,
        CancellationToken cancellationToken = default) =>
        _api.SendAsync(
            new RegisterDomainRequest(domainName)
            {
                Period = years,
                NameServers = nameServers?.ToList() ?? [],
                Contacts = contact is null ? [] : DomainContact.ForAllRoles(contact),
            },
            cancellationToken);

    /// <summary>Registers a domain using existing contact handles. <b>Charges your balance.</b></summary>
    /// <param name="request">The registration.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<DomainOperationResult> RegisterWithHandlesAsync(RegisterDomainWithHandlesRequest request, CancellationToken cancellationToken = default) =>
        _api.SendAsync(request, cancellationToken);

    /// <summary>Registers a domain and uses one contact handle for all four roles. <b>Charges your balance.</b></summary>
    /// <param name="domainName">The domain to register.</param>
    /// <param name="handle">The contact handle, for example <c>DNA-12345</c>.</param>
    /// <param name="nameServers">The nameservers; most registries need at least two.</param>
    /// <param name="years">The registration period in years.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<DomainOperationResult> RegisterWithHandleAsync(
        string domainName,
        string handle,
        IEnumerable<string> nameServers,
        int years = 1,
        CancellationToken cancellationToken = default) =>
        _api.SendAsync(
            new RegisterDomainWithHandlesRequest(domainName)
            {
                Period = years,
                NameServers = nameServers?.ToList() ?? [],
                Handles = ContactHandle.ForAllRoles(handle),
            },
            cancellationToken);

    /// <summary>Renews a domain. <b>Charges your balance.</b></summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="years">The renewal period in years.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<DomainOperationResult> RenewAsync(string domainName, int years = 1, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new RenewDomainRequest(domainName, years), cancellationToken);

    /// <summary>Checks whether a domain can be renewed, and its premium price if any. Charges nothing.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="years">The renewal period in years to check.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<RenewalCheck> CheckRenewalAsync(string domainName, int years = 1, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new CheckRenewalRequest(domainName, years), cancellationToken);

    /// <summary>Transfers a domain in from another registrar. <b>Charges your balance.</b></summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="authCode">The transfer (EPP) code from the current registrar.</param>
    /// <param name="years">Years added by the transfer.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<DomainOperationResult> TransferAsync(string domainName, string authCode, int years = 1, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new TransferDomainRequest(domainName, authCode, years), cancellationToken);

    /// <summary>Checks whether a domain can be transferred in with an auth code. Charges nothing.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="authCode">The transfer (EPP) code.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<TransferCheck> CheckTransferAsync(string domainName, string authCode, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new CheckTransferRequest(domainName, authCode), cancellationToken);

    /// <summary>Returns the state of a pending transfer.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<TransferStatus> GetTransferStatusAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new GetTransferStatusRequest(domainName), cancellationToken);

    /// <summary>Restores a deleted domain from redemption. <b>Charges your balance.</b></summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task RestoreAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new RestoreDomainRequest(domainName), cancellationToken);

    // ---------------------------------------------------------------- settings

    /// <summary>Turns the transfer lock on.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task LockAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new LockDomainRequest(domainName), cancellationToken);

    /// <summary>Turns the transfer lock off, which is needed before transferring out.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task UnlockAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new UnlockDomainRequest(domainName), cancellationToken);

    /// <summary>Turns WHOIS privacy protection on or off.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="enabled">True to hide the contacts in WHOIS.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task SetPrivacyProtectionAsync(string domainName, bool enabled, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new SetPrivacyProtectionRequest(domainName, enabled), cancellationToken);

    /// <summary>Sets a new transfer (EPP) code and returns it.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<DomainAuthCode> UpdateAuthCodeAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new UpdateAuthCodeRequest(domainName), cancellationToken);

    /// <summary>Replaces the nameservers of a domain.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="nameServers">The complete list of nameservers.</param>
    /// <param name="dnssecMode">What to do with DNSSEC on a signed domain; null refuses the change while DS records exist.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task SetNameServersAsync(
        string domainName,
        IEnumerable<string> nameServers,
        DnssecMode? dnssecMode = null,
        CancellationToken cancellationToken = default) =>
        _api.SendAsync(new SetNameServersRequest(domainName, nameServers) { DnssecMode = dnssecMode }, cancellationToken);

    // ---------------------------------------------------------------- contacts

    /// <summary>Replaces the contacts of a domain with details given inline.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="contacts">The contacts, one per role. <see cref="DomainContact.ForAllRoles"/> fills all four.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task UpdateContactsAsync(string domainName, IEnumerable<DomainContact> contacts, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new UpdateDomainContactsRequest(domainName, contacts), cancellationToken);

    /// <summary>Points the contact roles of a domain at existing contact handles.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="handles">The handles, one per role. <see cref="ContactHandle.ForAllRoles"/> uses one handle for all four.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task SetContactHandlesAsync(string domainName, IEnumerable<ContactHandle> handles, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new SetDomainContactHandlesRequest(domainName, handles), cancellationToken);
}
