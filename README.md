<div align="center">
  <a href="README-TR.md"   >   TR <img style="padding-top: 8px" src="https://raw.githubusercontent.com/yammadev/flag-icons/master/png/TR.png" alt="TR" height="20" /></a>
  <a href="README.md"> | EN <img style="padding-top: 8px" src="https://raw.githubusercontent.com/yammadev/flag-icons/master/png/US.png" alt="EN" height="20" /></a>
   <a href="README-DE.md"> | DE <img style="padding-top: 8px" src="https://raw.githubusercontent.com/yammadev/flag-icons/master/png/DE.png" alt="DE" height="20" /></a>
  <a href="README-SA.md"> | SA <img style="padding-top: 8px" src="https://raw.githubusercontent.com/yammadev/flag-icons/master/png/SA.png" alt="AR" height="20" /></a>
  <a href="README-NL.md"> | NL <img style="padding-top: 8px" src="https://raw.githubusercontent.com/yammadev/flag-icons/master/png/NL.png" alt="NL" height="20" /></a>
  <a href="README-AZ.md"> | AZ <img style="padding-top: 8px" src="https://raw.githubusercontent.com/yammadev/flag-icons/master/png/AZ.png" alt="AZ" height="20" /></a>
  <a href="README-CN.md"> | CN <img style="padding-top: 8px" src="https://raw.githubusercontent.com/yammadev/flag-icons/master/png/CN.png" alt="CN" height="20" /></a>
  <a href="README-FR.md"> | FR <img style="padding-top: 8px" src="https://raw.githubusercontent.com/yammadev/flag-icons/master/png/FR.png" alt="FR" height="20" /></a>
  <a href="README-IT.md"> | IT <img style="padding-top: 8px" src="https://raw.githubusercontent.com/yammadev/flag-icons/master/png/IT.png" alt="IT" height="20" /></a>
</div>

# DomainNameAPI .NET SDK (dotnet-dna)

<p>
  <a href="https://github.com/domainreseller/dotnet-dna/actions/workflows/ci.yml"><img src="https://img.shields.io/github/actions/workflow/status/domainreseller/dotnet-dna/ci.yml?branch=main&label=CI&style=flat-square" alt="CI"></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/license-Apache--2.0-0B8DB5?style=flat-square" alt="Apache-2.0"></a>
</p>

The official .NET SDK for the [Domain Name API](https://www.domainnameapi.com) reseller platform. It covers all
67 operations of the REST API with typed clients and models: domains, contacts, DNS, DNSSEC, SSL certificates,
hosting, orders, products and your account.

```csharp
using var client = new DomainNameApiClient(resellerId, apiKey, ApiEnvironment.Ote);

var search = await client.Domains.SearchAsync("example.com");
if (search.Info?.IsAvailable == true)
{
    await client.Domains.RegisterAsync("example.com", contact, ["tr.apiname.com", "eu.apiname.com"]);
}
```

- **Typed everything.** Every operation is a request class and every answer a model with IntelliSense documentation. No dictionaries, no JSON parsing.
- **Your own requests.** Every request derives from `BaseRequest`. Subclass it for an endpoint the SDK does not cover yet.
- **Safe with money.** Every request declares whether it reads, writes, charges your balance or deletes something. Billable requests are never retried, and a read-only mode blocks every change.
- **Built-in resilience.** Rate limits are respected automatically, 429 answers are retried, and network errors are retried for reads.
- **Clear errors.** One exception type with a stable kind, the API's error code, field-level validation errors, the correlation ID and a hint.
- **Modern .NET.** Async with cancellation, automatic paging with `IAsyncEnumerable`, `IHttpClientFactory` support, nullable annotations.
- **Runs everywhere.** .NET 10, and any runtime that supports .NET Standard 2.0 (.NET Framework 4.6.2+, .NET 8, Mono, Unity).

It is the .NET counterpart of [`dna`, the Domain Name API CLI](https://github.com/domainreseller/cli): the same API
document, the same safety classes, retry rules and error codes.

## Contents

- [Install](#install)
- [Credentials](#credentials)
- [Quick start](#quick-start)
- [Configuration](#configuration)
- [Usage](#usage): [account](#account) · [domains](#domains) · [contacts](#contacts) · [DNS](#dns) · [DNSSEC](#dnssec) · [SSL](#ssl) · [hosting](#hosting) · [orders and products](#orders-and-products)
- [Requests and `BaseRequest`](#requests-and-baserequest)
- [Errors](#errors)
- [Safety, retries and rate limits](#safety-retries-and-rate-limits)
- [Dependency injection](#dependency-injection)
- [Testing your code](#testing-your-code)
- [Project layout](#project-layout)
- [Development](#development)
- [Upgrading from 1.x](#upgrading-from-1x)

## Install

> [!IMPORTANT]
> **Always use the [latest release](https://github.com/domainreseller/dotnet-dna/releases/latest).** Release packages are versioned and tested.
> The green **Code → Download ZIP** button downloads the development branch instead.

```bash
dotnet add package DomainNameApi
```

To install a downloaded `.nupkg`, put it in a folder and use that folder as a package source:

```bash
dotnet add package DomainNameApi --source ./packages
```

Or reference the project from a release's source:

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

**Requirements:** .NET 10 or later, or any runtime that supports .NET Standard 2.0. There are no third-party
dependencies. On .NET Standard 2.0 the package pulls in Microsoft's `System.Text.Json` and
`Microsoft.Bcl.AsyncInterfaces`, which are built into .NET.

## Credentials

The API authenticates every request with two values, which the SDK sends for you:

| Value | Header | Where to find it |
|---|---|---|
| Reseller ID (a UUID) | `__reseller` | Reseller panel → **API settings** |
| API key | `X-API-KEY` | Reseller panel → **API settings** |

> [!NOTE]
> These are **API credentials**: your panel login does not work here. **Production and OTE (the test environment) use
> different API keys.** Using one with the other fails with `INVALID_API_KEY` or `INVALID_RESELLER`.

## Quick start

```csharp
using DomainNameApi;
using DomainNameApi.Contacts;
using DomainNameApi.Domains;

// OTE is the test environment: real API, its own data, no real money.
using var client = new DomainNameApiClient(resellerId, apiKey, ApiEnvironment.Ote);

// 1. Check the credentials and the balance.
var account = await client.Account.GetAsync();
Console.WriteLine($"{account.ResellerName}: {account.UsdBalance} USD");

// 2. Is the domain available, and at what price?
var search = await client.Domains.SearchAsync("my-new-domain.com");
Console.WriteLine($"{search.Info?.Status} {search.Info?.Price} {search.Info?.Currency}");

// 3. Register it with one contact for all four roles. This charges the balance.
var contact = new ContactInfo
{
    FirstName = "Ada", LastName = "Lovelace", Email = "ada@example.com",
    Address = "1 Example Street", City = "Istanbul", State = "Istanbul", PostalCode = "34000",
    Country = "TR", PhoneCountryCode = "90", Phone = "5551234567",
};
var result = await client.Domains.RegisterAsync("my-new-domain.com", contact, ["tr.apiname.com", "eu.apiname.com"]);
Console.WriteLine($"{result.Status}, expires {result.ExpirationDate:d}");

// 4. List every domain, page by page.
await foreach (var domain in client.Domains.ListAllAsync())
{
    Console.WriteLine($"{domain.DomainName} expires in {domain.RemainingDays} days");
}
```

The [examples project](examples/DomainNameApi.Examples) runs scenarios like these against OTE:

```bash
dotnet run --project examples/DomainNameApi.Examples -- search example.com
```

## Configuration

There are four ways to create a client:

```csharp
// Credentials and environment
var client = new DomainNameApiClient(resellerId, apiKey, ApiEnvironment.Production);

// All settings
var client = new DomainNameApiClient(new DomainNameApiOptions
{
    ResellerId = resellerId,
    ApiKey = apiKey,
    Environment = ApiEnvironment.Ote,
    Timeout = TimeSpan.FromSeconds(30),
    ReadOnly = true,
});

// The same environment variables the dna CLI reads
var client = DomainNameApiClient.FromEnvironment();

// On top of your own HttpClient (for example from IHttpClientFactory)
var client = new DomainNameApiClient(httpClient, options);
```

| Option | Default | Meaning |
|---|---|---|
| `ResellerId`, `ApiKey` | (required) | The credentials. |
| `Environment` | `Production` | `Production` (`https://api.domainresellerapi.com`) or `Ote` (`https://ote.domainresellerapi.com`). |
| `BaseAddress` | none | Overrides the environment's address, for example for a proxy. |
| `Timeout` | 60 s | Time limit of one attempt. |
| `MaxRetries` | 2 | How often a failed request is repeated (see [retries](#safety-retries-and-rate-limits)). 0 turns retries off. |
| `MaxRetryDelay` | 30 s | The longest wait before a retry. |
| `PaceRequests` | `true` | Spaces requests to stay under the gateway's rate limits. |
| `ReadOnly` | `false` | Blocks every request that is not a read, before it is sent. |
| `UserAgent` | `DomainNameAPI-DotNet/<version>` | The `User-Agent` header. |

`FromEnvironment()` reads:

| Variable | Meaning |
|---|---|
| `DNA_RESELLER_ID`, `DNA_API_KEY` | The credentials. |
| `DNA_ENV` | `production` or `ote` (also `prod`, `live`, `test`, `sandbox`). |
| `DNA_TEST_MODE` | `true`, `1`, `on` or `yes` selects OTE. Used when `DNA_ENV` is not set. |
| `DNA_BASE_URL` | Optional base address override. |
| `DNA_READ_ONLY` | `true` turns on read-only mode. |

The client is thread-safe. Create one per set of credentials and reuse it for the lifetime of the application.

## Usage

Each area of the API has its own client. Methods take simple arguments; for more options, every method also has an
overload (or a sibling) that takes the request object.

### Account

```csharp
var account = await client.Account.GetAsync();             // ResellerId, ResellerName, TryBalance, UsdBalance
var profile = await client.Account.GetProfileAsync();      // Name, CompanyName, AccountCurrency, Status, ...

// This month's charges, across all pages
var request = new ListTransactionsRequest
{
    ChargeType = ChargeType.Subtract,
    StartDate = new DateTimeOffset(2026, 11, 1, 0, 0, 0, TimeSpan.Zero),
    TransactionTypes = { TransactionType.Register, TransactionType.Renew },
};
await foreach (var transaction in client.Account.ListAllTransactionsAsync(request))
{
    Console.WriteLine($"{transaction.CreationTime:d} {transaction.Type} {transaction.Amount} {transaction.AmountCurrency}");
}
```

### Domains

**Search**

```csharp
var one = await client.Domains.SearchAsync("example.com", currency: "USD");
bool available = one.Info?.IsAvailable == true;

var many = await client.Domains.BulkSearchAsync(["example.com", "example.net", "example.com.tr"]);
foreach (var domain in many.Available)
{
    Console.WriteLine($"{domain.DomainName}: {domain.Price} {domain.Currency}");
}
```

**List and details**

```csharp
var page = await client.Domains.ListAsync(new ListDomainsRequest { MaxResultCount = 50, Statuses = { DomainStatus.Active } });

await foreach (var domain in client.Domains.ListAllAsync(new ListDomainsRequest { DomainName = "example" }))
{
    Console.WriteLine($"{domain.DomainName} {domain.Status} {domain.ExpirationDate:d}");
}

var details = await client.Domains.GetAsync("example.com");
Console.WriteLine(details.AuthCode);
Console.WriteLine(details.GetContact(ContactType.Registrant)?.Email);
```

**Register, renew, transfer, restore** (these charge your balance)

```csharp
// One contact for all four roles
await client.Domains.RegisterAsync("example.com", contact, ["tr.apiname.com", "eu.apiname.com"], years: 2);

// Full control: different contacts per role, registry attributes, trustee service
await client.Domains.RegisterAsync(new RegisterDomainRequest("example.com.tr")
{
    Period = 1,
    NameServers = { "tr.apiname.com", "eu.apiname.com" },
    Contacts = DomainContact.ForAllRoles(contact),
    TldAttributes = new() { ["<key>"] = "<value>" },   // the keys: (await client.Products.GetTldAsync("com.tr"))!.Attributes
});

// With existing contact handles
await client.Domains.RegisterWithHandleAsync("example.com", "DNA-12345", ["tr.apiname.com", "eu.apiname.com"]);

var renewal = await client.Domains.CheckRenewalAsync("example.com");   // free
if (renewal.IsRenewable)
{
    await client.Domains.RenewAsync("example.com", years: 1);
}

var check = await client.Domains.CheckTransferAsync("example.com", "EPP-CODE");  // free
if (check.IsTransferable)
{
    await client.Domains.TransferAsync("example.com", "EPP-CODE");
}

await client.Domains.RestoreAsync("example.com");
```

Requests are checked before they are sent. A registration without all four contact roles, a contact without an e-mail
address or a period of 0 years throws an `ArgumentException`, and nothing reaches the API.

**Settings**

```csharp
await client.Domains.LockAsync("example.com");
await client.Domains.UnlockAsync("example.com");
await client.Domains.SetPrivacyProtectionAsync("example.com", enabled: true);
await client.Domains.SetNameServersAsync("example.com", ["ns1.example.net", "ns2.example.net"]);
await client.Domains.SetNameServersAsync("example.com", ["ns1.example.net"], DnssecMode.DisableDnssec);  // a signed domain
var authCode = await client.Domains.UpdateAuthCodeAsync("example.com");

await client.Domains.UpdateContactsAsync("example.com", DomainContact.ForAllRoles(contact));
await client.Domains.SetContactHandlesAsync("example.com", [new ContactHandle(ContactType.Tech, "DNA-12345")]);
```

**URL forwarding, child nameservers and documents**

```csharp
await client.Domains.Forwarding.SetAsync("example.com", "https://www.example.net", ForwardType.Standard);
var forwarding = await client.Domains.Forwarding.GetAsync("example.com");
await client.Domains.Forwarding.DeleteAsync("example.com");

// The IP version of each address is detected
await client.Domains.ChildNameServers.AddAsync("example.com", "ns1.example.com", "192.0.2.10", "2001:db8::10");
await client.Domains.ChildNameServers.UpdateAsync("example.com", "ns1.example.com", ["192.0.2.20"]);
await client.Domains.ChildNameServers.DeleteAsync("example.com", "ns1.example.com");

// Registry documents (mostly .tr)
await client.Domains.Documents.UploadAsync("example.com.tr", [DocumentUpload.FromFile(DocumentType.IdentityCard, "id.pdf")]);
await foreach (var document in client.Domains.Documents.ListAllAsync("example.com.tr"))
{
    Console.WriteLine($"{document.Name}: {document.Status}");
}
```

### Contacts

Contact handles are reusable contacts. Create one once and use it for many domains.

```csharp
var created = await client.Contacts.CreateAsync(contact);
string handle = created.Handle!;                          // for example DNA-12345

var details = await client.Contacts.GetAsync(handle);     // the contact and its verification state
var result = await client.Contacts.UpdateAsync(handle, contact);
Console.WriteLine($"{result.Stats?.SuccessCount} of {result.Stats?.TotalDomainCount} domains updated");

var verifications = await client.Contacts.GetVerificationAsync(handle);
await client.Contacts.ResendVerificationAsync(handle);
```

### DNS

Zone records of domains on the platform nameservers. Records resolve publicly once the domain uses those nameservers,
which `ApplyAsync` takes care of.

```csharp
var sets = await client.Dns.ListAsync("example.com");
foreach (var set in sets)
{
    Console.WriteLine($"{set.Name} {set.Type} {set.Ttl} {string.Join(", ", set.Values)}");
}

await client.Dns.AddAsync("example.com", "www", RecordType.A, "192.0.2.10");
await client.Dns.AddAsync("example.com", "@", "txt", "v=spf1 include:_spf.example.net ~all");   // "txt" becomes TXT
await client.Dns.AddAsync("example.com", "@", RecordType.MX, ["10 mx1.example.net", "20 mx2.example.net"], ttl: 3600);
await client.Dns.ReplaceAsync("example.com", "www", RecordType.A, ["192.0.2.20"], ttl: 300);
await client.Dns.DeleteAsync("example.com", "www", RecordType.A, "192.0.2.20");

var applied = await client.Dns.ApplyAsync("example.com");
```

The SDK checks what the API would refuse: a CNAME at the apex, a CAA value without its three parts, a TTL outside
1–86400. Zone calls are kept 1 second apart, the gateway's limit.

### DNSSEC

```csharp
var info = await client.Dnssec.GetAsync("example.com");   // Status, DsRecords, AllowedAlgorithms, ...

var record = DsRecordData.Parse("12345 13 2 49FD46E6C4B45C55D4AC69CBD3CD34AC1AFE51DE");
await client.Dnssec.AddAsync("example.com", record);
await client.Dnssec.RemoveAsync("example.com", record);
await client.Dnssec.DisableAsync("example.com");          // removes every DS record
```

### SSL

A domain can have several certificates, so an existing certificate is addressed by its **certificate ID** (a `Guid`),
never by its domain name. `PurchaseAsync` and `RenewAsync` return the ID: store it. `ListAsync` finds the IDs of the
certificates you already have.

```csharp
// 1. Buy (charges the balance). Product codes come from client.Products.ListSslProductsAsync().
var approvers = await client.Ssl.GetApproverEmailsAsync("example.com", "PKG-CODE");   // before the purchase
var order = await client.Ssl.PurchaseAsync("example.com", "PKG-CODE");
Guid certificateId = order.CertificateId;                  // store it: every later call takes it

// 2. A CSR and private key, if you do not have one
var csr = await client.Ssl.GenerateCsrAsync(new GenerateCsrRequest("example.com")
{
    Country = "TR", State = "Istanbul", Locality = "Istanbul", Organization = "Example Ltd", OrganizationUnit = "IT",
});

// 3. Configure within 15 days: CSR, validation method and contact
await client.Ssl.ConfigureAsync(new ConfigureCertificateRequest(certificateId, csr.Csr!, certificateContact)
{
    ValidationMethod = ValidationMethod.Email,
    ApproverEmail = approvers[0],
    WebServerType = WebServerType.Apache,
});

// 4. Wait for issuance, then download
var status = await client.Ssl.RefreshStatusAsync(certificateId);
if (status.IsIssued)
{
    var content = await client.Ssl.GetContentAsync(certificateId);
    File.WriteAllText("fullchain.pem", content.FullChain);
}

var details = await client.Ssl.GetAsync(certificateId);    // subscription, configuration, contacts
await client.Ssl.ReissueAsync(new ReissueCertificateRequest(certificateId) { Csr = newCsr, PrivateKey = newKey });
await client.Ssl.RevokeAsync(certificateId);               // cannot be undone
```

**List certificates and find an ID**

```csharp
var page = await client.Ssl.ListAsync(new ListCertificatesRequest { DomainName = "example.com" });   // newest first

await foreach (var certificate in client.Ssl.ListAllAsync())
{
    Console.WriteLine($"{certificate.CertificateId} {certificate.DomainName} {certificate.Status} {certificate.SubscriptionExpiresAt:d}");
}

// The approver addresses of a certificate you already have
var approverEmails = await client.Ssl.GetApproverEmailsAsync(certificateId);
```

**Renew** (this charges your balance)

```csharp
// Allowed from 30 days before details.Subscription.ExpiresAt. The renewal is a new certificate with its own ID.
var renewal = await client.Ssl.RenewAsync(certificateId);
await client.Ssl.ConfigureAsync(new ConfigureCertificateRequest(renewal.CertificateId, newCsr, certificateContact)
{
    ValidationMethod = ValidationMethod.Email,
    ApproverEmail = approvers[0],
    WebServerType = WebServerType.Apache,
});
```

A purchased or renewed certificate must be configured within 15 days; after that it is cancelled. To renew, call
`RenewAsync` and then `ConfigureAsync` with the new ID. Do not buy again with `PurchaseAsync`: that is a separate
purchase at the purchase price, and the old certificate is never marked as renewed. An empty certificate ID
(`Guid.Empty`) throws an `ArgumentException` before anything is sent.

### Hosting

```csharp
var packages = await client.Hosting.ListPackagesAsync();
var created = await client.Hosting.CreateAsync("example.com", "starter-linux");   // charges the balance
var status = await client.Hosting.GetStatusAsync("example.com");                   // panel access once ready
var hosting = await client.Hosting.GetAsync("example.com");
var plans = await client.Hosting.ListPlansAsync();
```

### Orders and products

```csharp
var order = await client.Orders.GetAsync("ORDER-CODE");

var com = await client.Products.GetTldAsync("com");
Console.WriteLine($"{com?.GetRegistrationPrice(1)?.Price} {com?.GetRegistrationPrice(1)?.Currency}, DNSSEC: {com?.SupportsDnssec}");

await foreach (var tld in client.Products.ListAllTldsAsync(new ListTldsRequest { Currency = "USD" }))
{
    Console.WriteLine(tld.Name);
}

var price = await client.Products.GetPriceAsync("com", OrderType.Renew, years: 1);
var ssl = await client.Products.ListSslProductsAsync(new ListSslProductsRequest { IsWildcard = true });
```

## Requests and `BaseRequest`

Every operation is a request class, for example `RenewDomainRequest`, `ListDomainsRequest` or `AddDnsRecordRequest`.
The client methods above build them for you, and you can also create and send them yourself:

```csharp
var request = new RenewDomainRequest("example.com", period: 2);
Console.WriteLine($"{request} is {request.Safety}");   // "POST domains/renew is Billable"

DomainOperationResult result = await client.SendAsync(request);

// The same, with the HTTP details: correlation ID, soft-quota state, attempts, duration
var response = await client.SendWithResponseAsync(request);
Console.WriteLine($"{response.CorrelationId} quota {response.Quota?.Used}/{response.Quota?.Limit}");
```

All requests derive from one of three base classes:

| Base class | For |
|---|---|
| `BaseRequest` | Operations that return nothing (lock, unlock, add a DNS record, ...). |
| `BaseRequest<TResponse>` | Operations that return data. |
| `PagedRequest<TItem>` | List operations. Adds `SkipCount`, `MaxResultCount` and `Sorting`, and works with `ListAllAsync`. |

A request says what it is with a few overrides. For POST and PUT the request object itself is the JSON body (camelCase,
nulls left out). Query parameters are added in `AddQueryParameters`, and checks before sending go in `Validate`.
To call an endpoint the SDK does not cover yet, write your own:

```csharp
public sealed class GetDomainNotesRequest : BaseRequest<DomainNotes>
{
    public GetDomainNotesRequest(string domainName) => DomainName = domainName;

    public string DomainName { get; }

    public override HttpMethod Method => HttpMethod.Get;
    public override string Path => "domains/notes";              // below /api/v1/
    public override OperationSafety Safety => OperationSafety.Read;

    protected override void AddQueryParameters(QueryParameters query) => query.Add("DomainName", DomainName);
    protected override void Validate() => Require(DomainName, nameof(DomainName));
}

var notes = await client.SendAsync(new GetDomainNotesRequest("example.com"));
```

Your request gets everything the built-in ones get: credentials, retries by safety class, pacing, read-only mode and
error handling. Override `GetBody()` when the API expects a different body shape, and `IsIdempotent` when a write is
safe to repeat.

## Errors

Every failed request throws a `DomainNameApiException`. Invalid arguments throw `ArgumentException` before anything is
sent.

```csharp
try
{
    await client.Domains.RenewAsync("example.com");
}
catch (DomainNameApiException e) when (e.Kind == ApiErrorKind.Validation)
{
    Console.WriteLine($"{e.Code}: {e.Message}");          // Dna.DomainService:Domain:10007: ...
    foreach (var field in e.ValidationErrors)
    {
        Console.WriteLine($"  {field}");
    }
}
catch (DomainNameApiException e) when (e.OutcomeUnknown)
{
    // A billable or changing request failed on the network: it may have reached the API.
    // Check the state before sending it again.
    var details = await client.Domains.GetAsync("example.com");
}
```

| Property | Meaning |
|---|---|
| `Kind` | A stable classification (table below). Branch on it, not on the message. |
| `Code` | The API's error code (such as `Dna.DomainService:Domain:10007`), or one of the SDK's codes below. |
| `Message` | What happened. Boilerplate messages are replaced with the API's details. |
| `StatusCode` | The HTTP status, or null when no answer arrived. |
| `Details`, `ValidationErrors` | Additional detail and field-level problems from the API. |
| `CorrelationId` | The gateway's `X-Correlation-Id`. Quote it when contacting support. |
| `Hint` | A short suggestion on what to do next. |
| `IsRetryable` | Whether repeating the same request later can succeed. |
| `OutcomeUnknown` | A request that changes something failed on the network, so it may have run. |
| `Operation`, `Safety` | The failed request (`POST domains/renew`) and its safety class. |
| `RetryAfter`, `ResponseBody` | The gateway's `Retry-After` and the raw answer. |

| `Kind` | When |
|---|---|
| `Authentication` | Wrong or missing credentials, or an unknown or suspended reseller in this environment (401, redirect). |
| `Forbidden` | Authenticated but not allowed (403). |
| `NotFound` | The domain, contact, order or other resource does not exist (404). |
| `Validation` | The API refused the input or a business rule failed: bad field, domain locked, insufficient balance (400, 422). |
| `Conflict` | The resource's state does not allow the operation (409). |
| `RateLimited` | Still rate limited after the automatic retries (429). |
| `Server` | A server-side failure (5xx). Often temporary. |
| `Network`, `Timeout` | No answer: DNS, TLS, proxy, connection reset, or no answer within `Timeout`. |
| `ReadOnly` | Blocked by read-only mode; nothing was sent. |
| `Unknown` | Anything else, including an answer the SDK could not read. |

Codes the SDK sets itself:

| Code | Meaning |
|---|---|
| `INVALID_RESELLER` | The gateway redirected to its suspend page: the reseller ID is unknown, inactive or suspended **in this environment**. |
| `INVALID_API_KEY` | The key does not match the reseller. Production and OTE keys differ. |
| `MISSING_API_KEY`, `MISSING_RESELLER_ID`, `NO_CREDENTIALS` | A credential is empty. |
| `SUBRESELLER_NOT_ALLOWED` | Sub-reseller accounts cannot use the API. Use the main reseller's credentials. |
| `UNAUTHORIZED` | Any other 401. |
| `RATE_LIMITED` | 429 after the retries. |
| `NETWORK`, `TIMEOUT` | No answer arrived. |
| `READ_ONLY` | Blocked by read-only mode. |
| `EMPTY_RESPONSE`, `INVALID_RESPONSE` | A successful answer was empty or could not be read. |
| `HTTP_<status>` | The API sent no error code. |

## Safety, retries and rate limits

Every request has a safety class, the same one the `dna` CLI uses:

| `OperationSafety` | Means | Examples |
|---|---|---|
| `Read` | No side effects, even when the API uses POST or PUT | search, details, lists, renewal and transfer checks, CSR generation |
| `Write` | A free change that can be undone | DNS records, lock, privacy, nameservers, contacts |
| `Billable` | Charges your balance | register, renew, transfer, restore, SSL purchase and renewal, hosting order |
| `Destructive` | Deletes something or is hard to undo | DNS record deletion, DNSSEC removal, certificate revocation |

- **Read-only mode** (`ReadOnly = true` or `DNA_READ_ONLY=true`) blocks everything that is not a `Read` before it is sent.
  Use it while exploring, in reports and in dashboards.
- **Retries.** 429 answers are always retried, honouring `Retry-After`. This is safe even for billable requests, because
  the gateway refuses them before anything runs. Network errors, timeouts and 502/503/504 answers are retried only for
  reads (and for writes that are safe to repeat, such as replacing nameservers). A registration or renewal is never sent twice.
- **Pacing.** The gateway allows 5 requests per second per key, 2 per second for domain searches and 20 per second for
  DNS zone calls. The client spaces requests to stay under these limits (zone calls 1 second apart). Domain info and the
  TLD list are not rate limited.
- **Soft quota.** Searches and info lookups cost 1 point, everything else 10. Past 200 points the gateway slows each
  request down by up to 5 seconds; points decay by 20% per minute. `SendWithResponseAsync` shows the state in `Quota`.

## Dependency injection

`DomainNameApiClient` works as a typed `HttpClient`:

```csharp
builder.Services.AddSingleton(new DomainNameApiOptions
{
    ResellerId = builder.Configuration["DomainNameApi:ResellerId"]!,
    ApiKey = builder.Configuration["DomainNameApi:ApiKey"]!,
    Environment = ApiEnvironment.Ote,
});
builder.Services.AddHttpClient<DomainNameApiClient>();

// Then inject it
public sealed class RenewalService(DomainNameApiClient dna)
{
    public Task<DomainOperationResult> RenewAsync(string domain) => dna.Domains.RenewAsync(domain);
}
```

The SDK follows no redirects on its own `HttpClient`. A factory-made client may follow them; the SDK still recognises
the gateway's suspend page and reports `INVALID_RESELLER`.

## Testing your code

Pass an `HttpClient` with a stub handler to test code that uses the SDK without calling the API:

```csharp
sealed class StubHandler(string json) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });
}

var client = new DomainNameApiClient(
    new HttpClient(new StubHandler("""{"resellerName":"Test","usdBalance":10}""")),
    new DomainNameApiOptions { ResellerId = "test", ApiKey = "test", PaceRequests = false });

var account = await client.Account.GetAsync();   // UsdBalance == 10
```

For tests against the real API, use OTE with its own key.

## Project layout

```
src/DomainNameApi/
├── DomainNameApiClient.cs        The entry point: sub-clients, SendAsync, retries, pacing
├── DomainNameApiOptions.cs       Settings and FromEnvironment()
├── Core/                         BaseRequest, PagedRequest, QueryParameters, errors, responses, shared enums
├── Internal/                     JSON settings, error parsing, the rate-limit pacer
├── Accounts/   AccountClient     Balances, profile and transactions
├── Contacts/   ContactsClient    Contact handles
├── Domains/    DomainsClient     Domains, plus Forwarding, ChildNameServers and Documents
├── Dns/        DnsClient         Zone records
├── Dnssec/     DnssecClient      DS records
├── Ssl/        SslClient         Certificates
├── Hosting/    HostingClient     Hosting packages and services
├── Orders/     OrdersClient      Orders
└── Products/   ProductsClient    TLDs, SSL products and prices
```

Every area folder has the same shape: `XxxClient.cs`, a `Requests/` folder with one class per operation, and a `Models/`
folder with the answers and enums. All types of an area share one namespace, such as `DomainNameApi.Domains`.

## Development

```bash
dotnet build
dotnet test
```

The tests never call the real API: they use a fake `HttpMessageHandler`. On Windows they run on .NET 10 and on
.NET Framework 4.8; the latter exercises the .NET Standard 2.0 build.

`tests/DomainNameApi.Tests/testdata/openapi.json` is the API document the `dna` CLI embeds. `SpecCoverageTests` checks
that every operation in it has exactly one request class with the CLI's safety class. When the API gains an operation,
update the document and the test fails until the request exists.

## Upgrading from 1.x

2.0 is a new SDK. 1.x was a port of the PHP library and returned untyped `JsonNode` envelopes; 2.0 has typed requests
and models, async methods and exceptions.

| 1.x | 2.0 |
|---|---|
| `new DomainNameApiClient(id, key, testMode: true)` | `new DomainNameApiClient(id, key, ApiEnvironment.Ote)` |
| `dna.CheckAvailability(...)` | `client.Domains.SearchAsync(...)` / `BulkSearchAsync(...)` |
| `dna.GetDetails(domain)` | `client.Domains.GetAsync(domain)` |
| `dna.GetList()` | `client.Domains.ListAsync()` / `ListAllAsync()` |
| `dna.RegisterWithContactInfo(...)` | `client.Domains.RegisterAsync(...)` |
| `dna.Renew(domain, period)` | `client.Domains.RenewAsync(domain, years)` |
| `dna.ModifyNameServer(...)` | `client.Domains.SetNameServersAsync(...)` |
| `dna.AddChildNameServer(...)` | `client.Domains.ChildNameServers.AddAsync(...)` |
| `dna.GetResourceRecords(domain)` | `client.Dns.ListAsync(domain)` |
| `dna.AddResourceRecord(...)` | `client.Dns.AddAsync(...)` |
| `dna.SetForwarding(...)` | `client.Domains.Forwarding.SetAsync(...)` |
| `dna.GetCurrentBalance()` | `client.Account.GetAsync()` |
| `result["result"] == "ERROR"` | `catch (DomainNameApiException e)` |

1.x sent error reports to Sentry. 2.0 sends nothing anywhere except to the API.

## Links

- [`dna` CLI and MCP server](https://github.com/domainreseller/cli): the same API from the terminal and from AI agents.
- [API documentation](https://www.domainnameapi.com/docs/)
- [Changelog](CHANGELOG.md)

## License

Apache License 2.0. See [LICENSE](LICENSE).
