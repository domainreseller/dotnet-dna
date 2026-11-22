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

De officiële .NET SDK voor het resellerplatform van [Domain Name API](https://www.domainnameapi.com). Hij dekt alle
67 operaties van de REST API met getypeerde clients en modellen: domeinen, contacten, DNS, DNSSEC, SSL-certificaten,
hosting, bestellingen, producten en je account.

```csharp
using var client = new DomainNameApiClient(resellerId, apiKey, ApiEnvironment.Ote);

var search = await client.Domains.SearchAsync("example.com");
if (search.Info?.IsAvailable == true)
{
    await client.Domains.RegisterAsync("example.com", contact, ["tr.apiname.com", "eu.apiname.com"]);
}
```

- **Alles getypeerd.** Elke operatie is een requestklasse en elk antwoord een model met IntelliSense-documentatie. Geen dictionaries, geen JSON-parsing.
- **Je eigen requests.** Elk request erft van `BaseRequest`. Maak een subklasse voor een endpoint dat de SDK nog niet dekt.
- **Veilig met geld.** Elk request geeft aan of het leest, schrijft, je saldo belast of iets verwijdert. Billable requests worden nooit herhaald en een read-only modus blokkeert elke wijziging.
- **Ingebouwde veerkracht.** Rate limits worden automatisch gerespecteerd, 429-antwoorden worden herhaald en netwerkfouten worden herhaald bij leesacties.
- **Duidelijke fouten.** Eén uitzonderingstype met een vaste soort, de foutcode van de API, validatiefouten per veld, de correlation ID en een hint.
- **Modern .NET.** Async met annulering, automatische paginering met `IAsyncEnumerable`, ondersteuning voor `IHttpClientFactory`, nullable-annotaties.
- **Draait overal.** .NET 10 en elke runtime die .NET Standard 2.0 ondersteunt (.NET Framework 4.6.2+, .NET 8, Mono, Unity).

Het is de .NET-tegenhanger van [`dna`, de Domain Name API CLI](https://github.com/domainreseller/cli): hetzelfde API-document,
dezelfde veiligheidsklassen, retryregels en foutcodes.

## Inhoud

- [Installatie](#install)
- [Inloggegevens](#credentials)
- [Snel aan de slag](#quick-start)
- [Configuratie](#configuration)
- [Gebruik](#usage): [account](#account) · [domeinen](#domains) · [contacten](#contacts) · [DNS](#dns) · [DNSSEC](#dnssec) · [SSL](#ssl) · [hosting](#hosting) · [bestellingen en producten](#orders-and-products)
- [Requests en `BaseRequest`](#requests-and-baserequest)
- [Fouten](#errors)
- [Veiligheid, retries en rate limits](#safety-retries-and-rate-limits)
- [Dependency injection](#dependency-injection)
- [Je code testen](#testing-your-code)
- [Projectindeling](#project-layout)
- [Ontwikkeling](#development)
- [Upgraden van 1.x](#upgrading-from-1x)

<a id="install"></a>

## Installatie

> [!IMPORTANT]
> **Gebruik altijd de [nieuwste release](https://github.com/domainreseller/dotnet-dna/releases/latest).** Release-pakketten hebben een versienummer en zijn getest.
> De groene knop **Code → Download ZIP** downloadt in plaats daarvan de ontwikkelbranch.

```bash
dotnet add package DomainNameApi
```

Om een gedownload `.nupkg` te installeren, zet je het in een map en gebruik je die map als pakketbron:

```bash
dotnet add package DomainNameApi --source ./packages
```

Of verwijs naar het project uit de broncode van een release:

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

**Vereisten:** .NET 10 of nieuwer, of een runtime die .NET Standard 2.0 ondersteunt. Er zijn geen dependencies van
derden. Op .NET Standard 2.0 haalt het package `System.Text.Json` en `Microsoft.Bcl.AsyncInterfaces` van Microsoft
binnen, die in .NET ingebouwd zijn.

<a id="credentials"></a>

## Inloggegevens

De API authenticeert elk request met twee waarden, die de SDK voor je meestuurt:

| Waarde | Header | Waar je het vindt |
|---|---|---|
| Reseller ID (een UUID) | `__reseller` | Resellerpaneel → **API settings** |
| API-sleutel | `X-API-KEY` | Resellerpaneel → **API settings** |

> [!NOTE]
> Dit zijn **API-inloggegevens**: je paneelaccount werkt hier niet. **Productie en OTE (de testomgeving) gebruiken
> verschillende API-sleutels.** Als je de ene bij de andere gebruikt, krijg je `INVALID_API_KEY` of `INVALID_RESELLER`.

<a id="quick-start"></a>

## Snel aan de slag

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

Het [voorbeeldproject](examples/DomainNameApi.Examples) draait scenario's zoals deze tegen OTE:

```bash
dotnet run --project examples/DomainNameApi.Examples -- search example.com
```

<a id="configuration"></a>

## Configuratie

Er zijn vier manieren om een client te maken:

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

| Optie | Standaard | Betekenis |
|---|---|---|
| `ResellerId`, `ApiKey` | (verplicht) | De inloggegevens. |
| `Environment` | `Production` | `Production` (`https://api.domainresellerapi.com`) of `Ote` (`https://ote.domainresellerapi.com`). |
| `BaseAddress` | geen | Overschrijft het adres van de omgeving, bijvoorbeeld voor een proxy. |
| `Timeout` | 60 s | Tijdslimiet van één poging. |
| `MaxRetries` | 2 | Hoe vaak een mislukt request wordt herhaald (zie [retries](#safety-retries-and-rate-limits)). 0 zet retries uit. |
| `MaxRetryDelay` | 30 s | De langste wachttijd voor een retry. |
| `PaceRequests` | `true` | Spreidt requests zodat je onder de rate limits van de gateway blijft. |
| `ReadOnly` | `false` | Blokkeert elk request dat geen leesactie is, voordat het wordt verstuurd. |
| `UserAgent` | `DomainNameAPI-DotNet/<version>` | De `User-Agent`-header. |

`FromEnvironment()` leest:

| Variabele | Betekenis |
|---|---|
| `DNA_RESELLER_ID`, `DNA_API_KEY` | De inloggegevens. |
| `DNA_ENV` | `production` of `ote` (ook `prod`, `live`, `test`, `sandbox`). |
| `DNA_TEST_MODE` | `true`, `1`, `on` of `yes` kiest OTE. Wordt gebruikt als `DNA_ENV` niet is ingesteld. |
| `DNA_BASE_URL` | Optionele overschrijving van het basisadres. |
| `DNA_READ_ONLY` | `true` zet de read-only modus aan. |

De client is thread-safe. Maak er één per set inloggegevens en hergebruik hem zolang de applicatie draait.

<a id="usage"></a>

## Gebruik

Elk deel van de API heeft een eigen client. Methoden nemen eenvoudige argumenten; voor meer opties heeft elke methode ook een
overload (of een zusjermethode) die het requestobject neemt.

<a id="account"></a>

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

<a id="domains"></a>

### Domeinen

**Zoeken**

```csharp
var one = await client.Domains.SearchAsync("example.com", currency: "USD");
bool available = one.Info?.IsAvailable == true;

var many = await client.Domains.BulkSearchAsync(["example.com", "example.net", "example.com.tr"]);
foreach (var domain in many.Available)
{
    Console.WriteLine($"{domain.DomainName}: {domain.Price} {domain.Currency}");
}
```

**Lijst en details**

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

**Registreren, verlengen, verhuizen, herstellen** (deze belasten je saldo)

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

Requests worden gecontroleerd voordat ze worden verstuurd. Een registratie zonder alle vier de contactrollen, een contact zonder e-mailadres
of een periode van 0 jaar gooit een `ArgumentException`, en er bereikt niets de API.

**Instellingen**

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

**URL-doorsturing, child nameservers en documenten**

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

<a id="contacts"></a>

### Contacten

Contact handles zijn herbruikbare contacten. Maak er één aan en gebruik hem voor veel domeinen.

```csharp
var created = await client.Contacts.CreateAsync(contact);
string handle = created.Handle!;                          // for example DNA-12345

var details = await client.Contacts.GetAsync(handle);     // the contact and its verification state
var result = await client.Contacts.UpdateAsync(handle, contact);
Console.WriteLine($"{result.Stats?.SuccessCount} of {result.Stats?.TotalDomainCount} domains updated");

var verifications = await client.Contacts.GetVerificationAsync(handle);
await client.Contacts.ResendVerificationAsync(handle);
```

<a id="dns"></a>

### DNS

Zonerecords van domeinen op de nameservers van het platform. Records zijn publiek vindbaar zodra het domein die nameservers gebruikt,
waar `ApplyAsync` voor zorgt.

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

De SDK controleert wat de API zou weigeren: een CNAME op de apex, een CAA-waarde zonder de drie onderdelen, een TTL buiten
1–86400. Zoneaanroepen worden 1 seconde uit elkaar gehouden, de limiet van de gateway.

<a id="dnssec"></a>

### DNSSEC

```csharp
var info = await client.Dnssec.GetAsync("example.com");   // Status, DsRecords, AllowedAlgorithms, ...

var record = DsRecordData.Parse("12345 13 2 49FD46E6C4B45C55D4AC69CBD3CD34AC1AFE51DE");
await client.Dnssec.AddAsync("example.com", record);
await client.Dnssec.RemoveAsync("example.com", record);
await client.Dnssec.DisableAsync("example.com");          // removes every DS record
```

<a id="ssl"></a>

### SSL

Een domein kan meerdere certificaten hebben. Een bestaand certificaat wordt daarom aangeduid met zijn
**certificaat-ID** (een `Guid`), nooit met de domeinnaam. `PurchaseAsync` en `RenewAsync` geven het ID terug: bewaar het.
`ListAsync` vindt de ID's van de certificaten die je al hebt.

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

**Certificaten weergeven en een ID vinden**

```csharp
var page = await client.Ssl.ListAsync(new ListCertificatesRequest { DomainName = "example.com" });   // newest first

await foreach (var certificate in client.Ssl.ListAllAsync())
{
    Console.WriteLine($"{certificate.CertificateId} {certificate.DomainName} {certificate.Status} {certificate.SubscriptionExpiresAt:d}");
}

// The approver addresses of a certificate you already have
var approverEmails = await client.Ssl.GetApproverEmailsAsync(certificateId);
```

**Verlengen** (dit belast je saldo)

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

Een gekocht of verlengd certificaat moet binnen 15 dagen worden geconfigureerd; daarna wordt het geannuleerd. Om te
verlengen roep je `RenewAsync` aan en daarna `ConfigureAsync` met het nieuwe ID. Koop niet opnieuw met `PurchaseAsync`:
dat is een losse aankoop tegen de aankoopprijs, en het oude certificaat wordt nooit als verlengd gemarkeerd. Een leeg
certificaat-ID (`Guid.Empty`) gooit een `ArgumentException` voordat er iets wordt verstuurd.

<a id="hosting"></a>

### Hosting

```csharp
var packages = await client.Hosting.ListPackagesAsync();
var created = await client.Hosting.CreateAsync("example.com", "starter-linux");   // charges the balance
var status = await client.Hosting.GetStatusAsync("example.com");                   // panel access once ready
var hosting = await client.Hosting.GetAsync("example.com");
var plans = await client.Hosting.ListPlansAsync();
```

<a id="orders-and-products"></a>

### Bestellingen en producten

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

<a id="requests-and-baserequest"></a>

## Requests en `BaseRequest`

Elke operatie is een requestklasse, bijvoorbeeld `RenewDomainRequest`, `ListDomainsRequest` of `AddDnsRecordRequest`.
De clientmethoden hierboven bouwen ze voor je, en je kunt ze ook zelf maken en versturen:

```csharp
var request = new RenewDomainRequest("example.com", period: 2);
Console.WriteLine($"{request} is {request.Safety}");   // "POST domains/renew is Billable"

DomainOperationResult result = await client.SendAsync(request);

// The same, with the HTTP details: correlation ID, soft-quota state, attempts, duration
var response = await client.SendWithResponseAsync(request);
Console.WriteLine($"{response.CorrelationId} quota {response.Quota?.Used}/{response.Quota?.Limit}");
```

Alle requests erven van een van drie basisklassen:

| Basisklasse | Voor |
|---|---|
| `BaseRequest` | Operaties die niets teruggeven (vergrendelen, ontgrendelen, een DNS-record toevoegen, ...). |
| `BaseRequest<TResponse>` | Operaties die data teruggeven. |
| `PagedRequest<TItem>` | Lijstoperaties. Voegt `SkipCount`, `MaxResultCount` en `Sorting` toe en werkt met `ListAllAsync`. |

Een request geeft met een paar overrides aan wat het is. Bij POST en PUT is het requestobject zelf de JSON-body (camelCase,
null-waarden weggelaten). Queryparameters voeg je toe in `AddQueryParameters`, en controles vóór het versturen horen in `Validate`.
Om een endpoint aan te roepen dat de SDK nog niet dekt, schrijf je er zelf een:

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

Jouw request krijgt alles wat de ingebouwde requests krijgen: inloggegevens, retries per veiligheidsklasse, pacing, read-only modus en
foutafhandeling. Overschrijf `GetBody()` als de API een andere bodyvorm verwacht, en `IsIdempotent` als een schrijfactie
veilig te herhalen is.

<a id="errors"></a>

## Fouten

Elk mislukt request gooit een `DomainNameApiException`. Ongeldige argumenten gooien een `ArgumentException` voordat er iets
wordt verstuurd.

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

| Eigenschap | Betekenis |
|---|---|
| `Kind` | Een vaste classificatie (tabel hieronder). Vertak op deze waarde, niet op het bericht. |
| `Code` | De foutcode van de API (zoals `Dna.DomainService:Domain:10007`), of een van de SDK-codes hieronder. |
| `Message` | Wat er gebeurde. Standaardberichten worden vervangen door de details van de API. |
| `StatusCode` | De HTTP-status, of null als er geen antwoord kwam. |
| `Details`, `ValidationErrors` | Extra details en problemen per veld van de API. |
| `CorrelationId` | De `X-Correlation-Id` van de gateway. Noem hem als je contact opneemt met support. |
| `Hint` | Een korte suggestie voor wat je daarna kunt doen. |
| `IsRetryable` | Of het herhalen van hetzelfde request later kan slagen. |
| `OutcomeUnknown` | Een request dat iets wijzigt, mislukte op het netwerk, dus het kan zijn uitgevoerd. |
| `Operation`, `Safety` | Het mislukte request (`POST domains/renew`) en zijn veiligheidsklasse. |
| `RetryAfter`, `ResponseBody` | De `Retry-After` van de gateway en het ruwe antwoord. |

| `Kind` | Wanneer |
|---|---|
| `Authentication` | Onjuiste of ontbrekende inloggegevens, of een onbekende of geschorste reseller in deze omgeving (401, redirect). |
| `Forbidden` | Geauthenticeerd, maar niet toegestaan (403). |
| `NotFound` | Het domein, contact, de bestelling of een andere resource bestaat niet (404). |
| `Validation` | De API weigerde de invoer of een bedrijfsregel faalde: ongeldig veld, domein vergrendeld, onvoldoende saldo (400, 422). |
| `Conflict` | De status van de resource staat de operatie niet toe (409). |
| `RateLimited` | Na de automatische retries nog steeds rate limited (429). |
| `Server` | Een fout aan de serverzijde (5xx). Vaak tijdelijk. |
| `Network`, `Timeout` | Geen antwoord: DNS, TLS, proxy, verbroken verbinding, of geen antwoord binnen `Timeout`. |
| `ReadOnly` | Geblokkeerd door de read-only modus; er is niets verstuurd. |
| `Unknown` | Al het overige, inclusief een antwoord dat de SDK niet kon lezen. |

Codes die de SDK zelf instelt:

| Code | Betekenis |
|---|---|
| `INVALID_RESELLER` | De gateway verwees door naar de suspendpagina: de reseller-ID is onbekend, inactief of geschorst **in deze omgeving**. |
| `INVALID_API_KEY` | De sleutel hoort niet bij de reseller. Productie- en OTE-sleutels verschillen. |
| `MISSING_API_KEY`, `MISSING_RESELLER_ID`, `NO_CREDENTIALS` | Een inloggegeven is leeg. |
| `SUBRESELLER_NOT_ALLOWED` | Subreseller-accounts kunnen de API niet gebruiken. Gebruik de inloggegevens van de hoofdreseller. |
| `UNAUTHORIZED` | Elke andere 401. |
| `RATE_LIMITED` | 429 na de retries. |
| `NETWORK`, `TIMEOUT` | Er kwam geen antwoord. |
| `READ_ONLY` | Geblokkeerd door de read-only modus. |
| `EMPTY_RESPONSE`, `INVALID_RESPONSE` | Een geslaagd antwoord was leeg of kon niet worden gelezen. |
| `HTTP_<status>` | De API stuurde geen foutcode. |

<a id="safety-retries-and-rate-limits"></a>

## Veiligheid, retries en rate limits

Elk request heeft een veiligheidsklasse, dezelfde als die de `dna` CLI gebruikt:

| `OperationSafety` | Betekent | Voorbeelden |
|---|---|---|
| `Read` | Geen neveneffecten, ook niet als de API POST of PUT gebruikt | zoeken, details, lijsten, verlengings- en verhuiscontroles, CSR-generatie |
| `Write` | Een gratis wijziging die kan worden teruggedraaid | DNS-records, vergrendeling, privacy, nameservers, contacten |
| `Billable` | Belast je saldo | registreren, verlengen, verhuizen, herstellen, SSL-aankoop en -verlenging, hostingbestelling |
| `Destructive` | Verwijdert iets of is moeilijk terug te draaien | DNS-record verwijderen, DNSSEC verwijderen, certificaat intrekken |

- **Read-only modus** (`ReadOnly = true` of `DNA_READ_ONLY=true`) blokkeert alles wat geen `Read` is, voordat het wordt verstuurd.
  Gebruik hem tijdens het verkennen, in rapportages en in dashboards.
- **Retries.** 429-antwoorden worden altijd herhaald, met inachtneming van `Retry-After`. Dit is veilig, zelfs voor billable requests, omdat
  de gateway ze weigert voordat er iets wordt uitgevoerd. Netwerkfouten, timeouts en 502/503/504-antwoorden worden alleen herhaald voor
  leesacties (en voor schrijfacties die veilig te herhalen zijn, zoals het vervangen van nameservers). Een registratie of verlenging wordt nooit twee keer verstuurd.
- **Pacing.** De gateway staat 5 requests per seconde per sleutel toe, 2 per seconde voor domeinzoekopdrachten en 20 per seconde voor
  DNS-zoneaanroepen. De client spreidt requests om onder deze limieten te blijven (zoneaanroepen 1 seconde uit elkaar). Domeininfo en de
  TLD-lijst hebben geen rate limit.
- **Soft quota.** Zoekopdrachten en infoaanvragen kosten 1 punt, al het andere 10. Boven 200 punten vertraagt de gateway elk
  request met maximaal 5 seconden; punten nemen met 20% per minuut af. `SendWithResponseAsync` toont de stand in `Quota`.

<a id="dependency-injection"></a>

## Dependency injection

`DomainNameApiClient` werkt als een getypeerde `HttpClient`:

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

De SDK volgt op zijn eigen `HttpClient` geen redirects. Een client uit een factory kan ze wel volgen; de SDK herkent de
suspendpagina van de gateway dan nog steeds en meldt `INVALID_RESELLER`.

<a id="testing-your-code"></a>

## Je code testen

Geef een `HttpClient` met een stub-handler mee om code die de SDK gebruikt te testen zonder de API aan te roepen:

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

Gebruik voor tests tegen de echte API OTE met een eigen sleutel.

<a id="project-layout"></a>

## Projectindeling

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

Elke gebiedsmap heeft dezelfde opbouw: `XxxClient.cs`, een map `Requests/` met één klasse per operatie en een map `Models/`
met de antwoorden en enums. Alle typen van een gebied delen één namespace, zoals `DomainNameApi.Domains`.

<a id="development"></a>

## Ontwikkeling

```bash
dotnet build
dotnet test
```

De tests roepen nooit de echte API aan: ze gebruiken een nep-`HttpMessageHandler`. Op Windows draaien ze op .NET 10 en op
.NET Framework 4.8; die laatste test de .NET Standard 2.0-build.

`tests/DomainNameApi.Tests/testdata/openapi.json` is het API-document dat de `dna` CLI insluit. `SpecCoverageTests` controleert
of elke operatie erin precies één requestklasse heeft met de veiligheidsklasse van de CLI. Als de API een operatie erbij krijgt,
werk je het document bij en faalt de test totdat het request bestaat.

<a id="upgrading-from-1x"></a>

## Upgraden van 1.x

2.0 is een nieuwe SDK. 1.x was een port van de PHP-bibliotheek en gaf ongetypeerde `JsonNode`-envelopes terug; 2.0 heeft getypeerde requests
en modellen, async-methoden en uitzonderingen.

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

1.x stuurde foutrapporten naar Sentry. 2.0 stuurt nergens iets heen behalve naar de API.

## Links

- [`dna` CLI en MCP-server](https://github.com/domainreseller/cli): dezelfde API vanuit de terminal en vanuit AI-agents.
- [API-documentatie](https://www.domainnameapi.com/docs/)
- [Changelog](CHANGELOG.md)

## Licentie

Apache License 2.0. Zie [LICENSE](LICENSE).
