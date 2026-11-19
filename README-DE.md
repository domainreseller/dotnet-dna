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

Das offizielle .NET SDK für die Reseller-Plattform von [Domain Name API](https://www.domainnameapi.com). Es deckt alle
64 Operationen der REST-API mit typisierten Clients und Modellen ab: Domains, Kontakte, DNS, DNSSEC, SSL-Zertifikate,
Hosting, Bestellungen, Produkte und Ihr Konto.

```csharp
using var client = new DomainNameApiClient(resellerId, apiKey, ApiEnvironment.Ote);

var search = await client.Domains.SearchAsync("example.com");
if (search.Info?.IsAvailable == true)
{
    await client.Domains.RegisterAsync("example.com", contact, ["tr.apiname.com", "eu.apiname.com"]);
}
```

- **Alles typisiert.** Jede Operation ist eine Request-Klasse und jede Antwort ein Modell mit IntelliSense-Dokumentation. Keine Dictionaries, kein JSON-Parsing.
- **Ihre eigenen Requests.** Jeder Request erbt von `BaseRequest`. Leiten Sie davon ab, wenn ein Endpunkt vom SDK noch nicht abgedeckt wird.
- **Sicher im Umgang mit Geld.** Jeder Request gibt an, ob er liest, schreibt, Ihr Guthaben belastet oder etwas löscht. Kostenpflichtige Requests werden nie wiederholt, und ein Nur-Lese-Modus blockiert jede Änderung.
- **Eingebaute Ausfallsicherheit.** Rate-Limits werden automatisch eingehalten, 429-Antworten werden wiederholt, und Netzwerkfehler werden bei Lesezugriffen wiederholt.
- **Klare Fehler.** Ein Exception-Typ mit stabiler Art, dem Fehlercode der API, Validierungsfehlern auf Feldebene, der Correlation-ID und einem Hinweis.
- **Modernes .NET.** Async mit Abbruch, automatische Paginierung mit `IAsyncEnumerable`, Unterstützung für `IHttpClientFactory`, Nullable-Annotationen.
- **Läuft überall.** .NET 10 und jede Runtime, die .NET Standard 2.0 unterstützt (.NET Framework 4.6.2+, .NET 8, Mono, Unity).

Es ist das .NET-Pendant zu [`dna`, der Domain Name API CLI](https://github.com/domainreseller/cli): dasselbe API-Dokument,
dieselben Sicherheitsklassen, Wiederholungsregeln und Fehlercodes.

## Inhalt

- [Installation](#install)
- [Zugangsdaten](#credentials)
- [Schnellstart](#quick-start)
- [Konfiguration](#configuration)
- [Verwendung](#usage): [Konto](#account) · [Domains](#domains) · [Kontakte](#contacts) · [DNS](#dns) · [DNSSEC](#dnssec) · [SSL](#ssl) · [Hosting](#hosting) · [Bestellungen und Produkte](#orders-and-products)
- [Requests und `BaseRequest`](#requests-and-baserequest)
- [Fehler](#errors)
- [Sicherheit, Wiederholungen und Rate-Limits](#safety-retries-and-rate-limits)
- [Dependency Injection](#dependency-injection)
- [Ihren Code testen](#testing-your-code)
- [Projektstruktur](#project-layout)
- [Entwicklung](#development)
- [Umstieg von 1.x](#upgrading-from-1x)

<a id="install"></a>

## Installation

> [!IMPORTANT]
> **Verwenden Sie immer das [neueste Release](https://github.com/domainreseller/dotnet-dna/releases/latest).** Release-Pakete sind versioniert und getestet.
> Der grüne Button **Code → Download ZIP** lädt stattdessen den Entwicklungszweig herunter.

```bash
dotnet add package DomainNameApi
```

Um ein heruntergeladenes `.nupkg` zu installieren, legen Sie es in einen Ordner und verwenden Sie diesen Ordner als Paketquelle:

```bash
dotnet add package DomainNameApi --source ./packages
```

Oder referenzieren Sie das Projekt aus dem Quellcode eines Releases:

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

**Voraussetzungen:** .NET 10 oder neuer, oder jede Runtime, die .NET Standard 2.0 unterstützt. Es gibt keine
Abhängigkeiten von Drittanbietern. Unter .NET Standard 2.0 bindet das Paket Microsofts `System.Text.Json` und
`Microsoft.Bcl.AsyncInterfaces` ein, die in .NET enthalten sind.

<a id="credentials"></a>

## Zugangsdaten

Die API authentifiziert jeden Request mit zwei Werten, die das SDK für Sie sendet:

| Wert | Header | Fundort |
|---|---|---|
| Reseller-ID (eine UUID) | `__reseller` | Reseller-Panel → **API-Einstellungen** |
| API-Key | `X-API-KEY` | Reseller-Panel → **API-Einstellungen** |

> [!NOTE]
> Dies sind **API-Zugangsdaten**: Ihre Panel-Anmeldung funktioniert hier nicht. **Produktion und OTE (die Testumgebung) verwenden
> unterschiedliche API-Keys.** Wird der eine mit der jeweils anderen Umgebung verwendet, schlägt der Aufruf mit `INVALID_API_KEY` oder `INVALID_RESELLER` fehl.

<a id="quick-start"></a>

## Schnellstart

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

Das [Beispielprojekt](examples/DomainNameApi.Examples) führt Szenarien wie diese gegen OTE aus:

```bash
dotnet run --project examples/DomainNameApi.Examples -- search example.com
```

<a id="configuration"></a>

## Konfiguration

Es gibt vier Wege, einen Client zu erstellen:

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

| Option | Standard | Bedeutung |
|---|---|---|
| `ResellerId`, `ApiKey` | (erforderlich) | Die Zugangsdaten. |
| `Environment` | `Production` | `Production` (`https://api.domainresellerapi.com`) oder `Ote` (`https://ote.domainresellerapi.com`). |
| `BaseAddress` | keine | Überschreibt die Adresse der Umgebung, zum Beispiel für einen Proxy. |
| `Timeout` | 60 s | Zeitlimit eines einzelnen Versuchs. |
| `MaxRetries` | 2 | Wie oft ein fehlgeschlagener Request wiederholt wird (siehe [Wiederholungen](#safety-retries-and-rate-limits)). 0 schaltet Wiederholungen aus. |
| `MaxRetryDelay` | 30 s | Die längste Wartezeit vor einer Wiederholung. |
| `PaceRequests` | `true` | Verteilt Requests zeitlich, um unter den Rate-Limits des Gateways zu bleiben. |
| `ReadOnly` | `false` | Blockiert vor dem Senden jeden Request, der kein Lesezugriff ist. |
| `UserAgent` | `DomainNameAPI-DotNet/<version>` | Der `User-Agent`-Header. |

`FromEnvironment()` liest:

| Variable | Bedeutung |
|---|---|
| `DNA_RESELLER_ID`, `DNA_API_KEY` | Die Zugangsdaten. |
| `DNA_ENV` | `production` oder `ote` (auch `prod`, `live`, `test`, `sandbox`). |
| `DNA_TEST_MODE` | `true`, `1`, `on` oder `yes` wählt OTE. Wird verwendet, wenn `DNA_ENV` nicht gesetzt ist. |
| `DNA_BASE_URL` | Optionale Überschreibung der Basisadresse. |
| `DNA_READ_ONLY` | `true` schaltet den Nur-Lese-Modus ein. |

Der Client ist threadsicher. Erstellen Sie pro Satz Zugangsdaten einen Client und verwenden Sie ihn für die Lebensdauer der Anwendung weiter.

<a id="usage"></a>

## Verwendung

Jeder Bereich der API hat einen eigenen Client. Die Methoden nehmen einfache Argumente entgegen; für weitere Optionen hat jede Methode auch
eine Überladung (oder eine Schwestermethode), die das Request-Objekt annimmt.

<a id="account"></a>

### Konto

```csharp
var account = await client.Account.GetAsync();             // ResellerId, ResellerName, TryBalance, UsdBalance

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

### Domains

**Suche**

```csharp
var one = await client.Domains.SearchAsync("example.com", currency: "USD");
bool available = one.Info?.IsAvailable == true;

var many = await client.Domains.BulkSearchAsync(["example.com", "example.net", "example.com.tr"]);
foreach (var domain in many.Available)
{
    Console.WriteLine($"{domain.DomainName}: {domain.Price} {domain.Currency}");
}
```

**Liste und Details**

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

**Registrieren, verlängern, transferieren, wiederherstellen** (diese belasten Ihr Guthaben)

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

Requests werden vor dem Senden geprüft. Eine Registrierung ohne alle vier Kontaktrollen, ein Kontakt ohne E-Mail-Adresse
oder eine Laufzeit von 0 Jahren löst eine `ArgumentException` aus, und nichts erreicht die API.

**Einstellungen**

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

**URL-Weiterleitung, Child-Nameserver und Dokumente**

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

### Kontakte

Kontakt-Handles sind wiederverwendbare Kontakte. Legen Sie einen einmal an und verwenden Sie ihn für viele Domains.

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

Zoneneinträge von Domains auf den Nameservern der Plattform. Die Einträge sind öffentlich auflösbar, sobald die Domain diese Nameserver verwendet,
worum sich `ApplyAsync` kümmert.

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

Das SDK prüft, was die API ablehnen würde: einen CNAME am Apex, einen CAA-Wert ohne seine drei Bestandteile, eine TTL außerhalb von
1–86400. Zonenaufrufe werden im Abstand von 1 Sekunde gesendet, dem Limit des Gateways.

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

```csharp
// 1. Buy (charges the balance). Product codes come from client.Products.ListSslProductsAsync().
var order = await client.Ssl.PurchaseAsync("example.com", "PKG-CODE");

// 2. A CSR and private key, if you do not have one
var csr = await client.Ssl.GenerateCsrAsync(new GenerateCsrRequest("example.com")
{
    Country = "TR", State = "Istanbul", Locality = "Istanbul", Organization = "Example Ltd", OrganizationUnit = "IT",
});

// 3. Configure: CSR, validation method and contact
var approvers = await client.Ssl.GetApproverEmailsAsync("example.com");
await client.Ssl.ConfigureAsync(new ConfigureCertificateRequest("example.com", csr.Csr!, certificateContact)
{
    ValidationMethod = ValidationMethod.Email,
    ApproverEmail = approvers[0],
    WebServerType = WebServerType.Apache,
});

// 4. Wait for issuance, then download
var status = await client.Ssl.RefreshStatusAsync("example.com");
if (status.IsIssued)
{
    var content = await client.Ssl.GetContentAsync("example.com");
    File.WriteAllText("fullchain.pem", content.FullChain);
}

await client.Ssl.ReissueAsync(new ReissueCertificateRequest("example.com") { Csr = newCsr, PrivateKey = newKey });
await client.Ssl.RevokeAsync("example.com");              // cannot be undone
```

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

### Bestellungen und Produkte

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

## Requests und `BaseRequest`

Jede Operation ist eine Request-Klasse, zum Beispiel `RenewDomainRequest`, `ListDomainsRequest` oder `AddDnsRecordRequest`.
Die oben gezeigten Client-Methoden erstellen sie für Sie, und Sie können sie auch selbst erstellen und senden:

```csharp
var request = new RenewDomainRequest("example.com", period: 2);
Console.WriteLine($"{request} is {request.Safety}");   // "POST domains/renew is Billable"

DomainOperationResult result = await client.SendAsync(request);

// The same, with the HTTP details: correlation ID, soft-quota state, attempts, duration
var response = await client.SendWithResponseAsync(request);
Console.WriteLine($"{response.CorrelationId} quota {response.Quota?.Used}/{response.Quota?.Limit}");
```

Alle Requests erben von einer von drei Basisklassen:

| Basisklasse | Wofür |
|---|---|
| `BaseRequest` | Operationen, die nichts zurückgeben (sperren, entsperren, einen DNS-Eintrag hinzufügen, ...). |
| `BaseRequest<TResponse>` | Operationen, die Daten zurückgeben. |
| `PagedRequest<TItem>` | Listenoperationen. Ergänzt `SkipCount`, `MaxResultCount` und `Sorting` und funktioniert mit `ListAllAsync`. |

Ein Request beschreibt sich selbst über einige Overrides. Bei POST und PUT ist das Request-Objekt selbst der JSON-Body (camelCase,
Nullwerte weggelassen). Query-Parameter werden in `AddQueryParameters` hinzugefügt, und Prüfungen vor dem Senden gehören in `Validate`.
Um einen Endpunkt aufzurufen, den das SDK noch nicht abdeckt, schreiben Sie einen eigenen Request:

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

Ihr Request erhält alles, was die eingebauten erhalten: Zugangsdaten, Wiederholungen nach Sicherheitsklasse, Pacing, Nur-Lese-Modus und
Fehlerbehandlung. Überschreiben Sie `GetBody()`, wenn die API eine andere Body-Form erwartet, und `IsIdempotent`, wenn ein Schreibzugriff
sicher wiederholt werden kann.

<a id="errors"></a>

## Fehler

Jeder fehlgeschlagene Request löst eine `DomainNameApiException` aus. Ungültige Argumente lösen eine `ArgumentException` aus, bevor etwas
gesendet wird.

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

| Eigenschaft | Bedeutung |
|---|---|
| `Kind` | Eine stabile Klassifizierung (Tabelle unten). Verzweigen Sie danach, nicht nach der Meldung. |
| `Code` | Der Fehlercode der API (etwa `Dna.DomainService:Domain:10007`) oder einer der unten genannten Codes des SDK. |
| `Message` | Was passiert ist. Standardmeldungen werden durch die Details der API ersetzt. |
| `StatusCode` | Der HTTP-Status, oder null, wenn keine Antwort eingetroffen ist. |
| `Details`, `ValidationErrors` | Zusätzliche Details und Probleme auf Feldebene von der API. |
| `CorrelationId` | Die `X-Correlation-Id` des Gateways. Nennen Sie sie, wenn Sie den Support kontaktieren. |
| `Hint` | Ein kurzer Vorschlag, was als Nächstes zu tun ist. |
| `IsRetryable` | Ob die spätere Wiederholung desselben Requests erfolgreich sein kann. |
| `OutcomeUnknown` | Ein Request, der etwas ändert, ist im Netzwerk fehlgeschlagen und wurde möglicherweise ausgeführt. |
| `Operation`, `Safety` | Der fehlgeschlagene Request (`POST domains/renew`) und seine Sicherheitsklasse. |
| `RetryAfter`, `ResponseBody` | Das `Retry-After` des Gateways und die rohe Antwort. |

| `Kind` | Wann |
|---|---|
| `Authentication` | Falsche oder fehlende Zugangsdaten oder ein unbekannter oder gesperrter Reseller in dieser Umgebung (401, Weiterleitung). |
| `Forbidden` | Authentifiziert, aber nicht berechtigt (403). |
| `NotFound` | Die Domain, der Kontakt, die Bestellung oder eine andere Ressource existiert nicht (404). |
| `Validation` | Die API hat die Eingabe abgelehnt oder eine Geschäftsregel ist fehlgeschlagen: ungültiges Feld, Domain gesperrt, unzureichendes Guthaben (400, 422). |
| `Conflict` | Der Zustand der Ressource lässt die Operation nicht zu (409). |
| `RateLimited` | Auch nach den automatischen Wiederholungen weiterhin durch das Rate-Limit begrenzt (429). |
| `Server` | Ein serverseitiger Fehler (5xx). Oft vorübergehend. |
| `Network`, `Timeout` | Keine Antwort: DNS, TLS, Proxy, zurückgesetzte Verbindung oder keine Antwort innerhalb von `Timeout`. |
| `ReadOnly` | Vom Nur-Lese-Modus blockiert; nichts wurde gesendet. |
| `Unknown` | Alles andere, einschließlich einer Antwort, die das SDK nicht lesen konnte. |

Codes, die das SDK selbst setzt:

| Code | Bedeutung |
|---|---|
| `INVALID_RESELLER` | Das Gateway hat auf seine Sperrseite weitergeleitet: Die Reseller-ID ist **in dieser Umgebung** unbekannt, inaktiv oder gesperrt. |
| `INVALID_API_KEY` | Der Key passt nicht zum Reseller. Keys für Produktion und OTE unterscheiden sich. |
| `MISSING_API_KEY`, `MISSING_RESELLER_ID`, `NO_CREDENTIALS` | Ein Zugangswert ist leer. |
| `SUBRESELLER_NOT_ALLOWED` | Sub-Reseller-Konten können die API nicht verwenden. Verwenden Sie die Zugangsdaten des Haupt-Resellers. |
| `UNAUTHORIZED` | Jedes andere 401. |
| `RATE_LIMITED` | 429 nach den Wiederholungen. |
| `NETWORK`, `TIMEOUT` | Es ist keine Antwort eingetroffen. |
| `READ_ONLY` | Vom Nur-Lese-Modus blockiert. |
| `EMPTY_RESPONSE`, `INVALID_RESPONSE` | Eine erfolgreiche Antwort war leer oder konnte nicht gelesen werden. |
| `HTTP_<status>` | Die API hat keinen Fehlercode gesendet. |

<a id="safety-retries-and-rate-limits"></a>

## Sicherheit, Wiederholungen und Rate-Limits

Jeder Request hat eine Sicherheitsklasse, dieselbe, die die `dna` CLI verwendet:

| `OperationSafety` | Bedeutet | Beispiele |
|---|---|---|
| `Read` | Keine Seiteneffekte, auch wenn die API POST oder PUT verwendet | Suche, Details, Listen, Verlängerungs- und Transferprüfungen, CSR-Erzeugung |
| `Write` | Eine kostenlose Änderung, die sich rückgängig machen lässt | DNS-Einträge, Sperre, Datenschutz, Nameserver, Kontakte |
| `Billable` | Belastet Ihr Guthaben | Registrierung, Verlängerung, Transfer, Wiederherstellung, SSL-Kauf, Hosting-Bestellung |
| `Destructive` | Löscht etwas oder lässt sich kaum rückgängig machen | Löschen von DNS-Einträgen, Entfernen von DNSSEC, Widerruf von Zertifikaten |

- **Nur-Lese-Modus** (`ReadOnly = true` oder `DNA_READ_ONLY=true`) blockiert vor dem Senden alles, was kein `Read` ist.
  Verwenden Sie ihn beim Erkunden, in Berichten und in Dashboards.
- **Wiederholungen.** 429-Antworten werden immer wiederholt, wobei `Retry-After` beachtet wird. Das ist auch bei kostenpflichtigen Requests sicher, weil
  das Gateway sie ablehnt, bevor etwas ausgeführt wird. Netzwerkfehler, Timeouts und 502/503/504-Antworten werden nur bei
  Lesezugriffen wiederholt (und bei Schreibzugriffen, die sich sicher wiederholen lassen, etwa dem Ersetzen von Nameservern). Eine Registrierung oder Verlängerung wird nie zweimal gesendet.
- **Pacing.** Das Gateway erlaubt 5 Requests pro Sekunde und Key, 2 pro Sekunde für Domainsuchen und 20 pro Sekunde für
  DNS-Zonenaufrufe. Der Client verteilt Requests zeitlich, um unter diesen Limits zu bleiben (Zonenaufrufe im Abstand von 1 Sekunde). Domain-Infos und die
  TLD-Liste unterliegen keinem Rate-Limit.
- **Soft-Quota.** Suchen und Info-Abfragen kosten 1 Punkt, alles andere 10. Ab 200 Punkten verlangsamt das Gateway jeden
  Request um bis zu 5 Sekunden; die Punkte verfallen um 20 % pro Minute. `SendWithResponseAsync` zeigt den Zustand in `Quota`.

<a id="dependency-injection"></a>

## Dependency Injection

`DomainNameApiClient` funktioniert als typisierter `HttpClient`:

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

Das SDK folgt auf seinem eigenen `HttpClient` keinen Weiterleitungen. Ein von einer Factory erzeugter Client folgt ihnen möglicherweise; das SDK erkennt die
Sperrseite des Gateways trotzdem und meldet `INVALID_RESELLER`.

<a id="testing-your-code"></a>

## Ihren Code testen

Übergeben Sie einen `HttpClient` mit einem Stub-Handler, um Code, der das SDK verwendet, zu testen, ohne die API aufzurufen:

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

Für Tests gegen die echte API verwenden Sie OTE mit dem zugehörigen eigenen Key.

<a id="project-layout"></a>

## Projektstruktur

```
src/DomainNameApi/
├── DomainNameApiClient.cs        The entry point: sub-clients, SendAsync, retries, pacing
├── DomainNameApiOptions.cs       Settings and FromEnvironment()
├── Core/                         BaseRequest, PagedRequest, QueryParameters, errors, responses, shared enums
├── Internal/                     JSON settings, error parsing, the rate-limit pacer
├── Accounts/   AccountClient     Balances and transactions
├── Contacts/   ContactsClient    Contact handles
├── Domains/    DomainsClient     Domains, plus Forwarding, ChildNameServers and Documents
├── Dns/        DnsClient         Zone records
├── Dnssec/     DnssecClient      DS records
├── Ssl/        SslClient         Certificates
├── Hosting/    HostingClient     Hosting packages and services
├── Orders/     OrdersClient      Orders
└── Products/   ProductsClient    TLDs, SSL products and prices
```

Jeder Bereichsordner hat denselben Aufbau: `XxxClient.cs`, einen Ordner `Requests/` mit einer Klasse pro Operation und einen Ordner `Models/`
mit den Antworten und Enums. Alle Typen eines Bereichs teilen sich einen Namespace, etwa `DomainNameApi.Domains`.

<a id="development"></a>

## Entwicklung

```bash
dotnet build
dotnet test
```

Die Tests rufen nie die echte API auf: Sie verwenden einen gefälschten `HttpMessageHandler`. Unter Windows laufen sie auf .NET 10 und auf
.NET Framework 4.8; Letzteres prüft den Build für .NET Standard 2.0.

`tests/DomainNameApi.Tests/testdata/openapi.json` ist das API-Dokument, das die `dna` CLI einbettet. `SpecCoverageTests` prüft,
dass jede darin enthaltene Operation genau eine Request-Klasse mit der Sicherheitsklasse der CLI hat. Wenn die API eine Operation erhält,
aktualisieren Sie das Dokument, und der Test schlägt so lange fehl, bis der Request existiert.

<a id="upgrading-from-1x"></a>

## Umstieg von 1.x

2.0 ist ein neues SDK. 1.x war eine Portierung der PHP-Bibliothek und gab untypisierte `JsonNode`-Umschläge zurück; 2.0 hat typisierte Requests
und Modelle, asynchrone Methoden und Exceptions.

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

1.x hat Fehlerberichte an Sentry gesendet. 2.0 sendet nirgendwohin etwas, außer an die API.

## Links

- [`dna` CLI und MCP-Server](https://github.com/domainreseller/cli): dieselbe API vom Terminal und von KI-Agenten aus.
- [API-Dokumentation](https://www.domainnameapi.com/docs/)
- [Changelog](CHANGELOG.md)

## Lizenz

Apache License 2.0. Siehe [LICENSE](LICENSE).
