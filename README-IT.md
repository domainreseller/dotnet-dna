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

L'SDK .NET ufficiale per la piattaforma reseller di [Domain Name API](https://www.domainnameapi.com). Copre tutte le
64 operazioni dell'API REST con client e modelli tipizzati: domini, contatti, DNS, DNSSEC, certificati SSL,
hosting, ordini, prodotti e il tuo account.

```csharp
using var client = new DomainNameApiClient(resellerId, apiKey, ApiEnvironment.Ote);

var search = await client.Domains.SearchAsync("example.com");
if (search.Info?.IsAvailable == true)
{
    await client.Domains.RegisterAsync("example.com", contact, ["tr.apiname.com", "eu.apiname.com"]);
}
```

- **Tutto tipizzato.** Ogni operazione è una classe di richiesta e ogni risposta un modello con documentazione IntelliSense. Niente dizionari, niente parsing JSON.
- **Le tue richieste.** Ogni richiesta deriva da `BaseRequest`. Estendila per un endpoint che l'SDK non copre ancora.
- **Sicuro con il denaro.** Ogni richiesta dichiara se legge, scrive, addebita il tuo saldo o elimina qualcosa. Le richieste a pagamento non vengono mai ripetute e la modalità di sola lettura blocca qualsiasi modifica.
- **Resilienza integrata.** I limiti di richieste vengono rispettati automaticamente, le risposte 429 vengono ritentate e gli errori di rete vengono ritentati per le letture.
- **Errori chiari.** Un solo tipo di eccezione con un tipo stabile, il codice di errore dell'API, gli errori di validazione per campo, il correlation ID e un suggerimento.
- **.NET moderno.** Async con annullamento, paginazione automatica con `IAsyncEnumerable`, supporto a `IHttpClientFactory`, annotazioni nullable.
- **Gira ovunque.** .NET 10 e qualsiasi runtime che supporti .NET Standard 2.0 (.NET Framework 4.6.2+, .NET 8, Mono, Unity).

È la controparte .NET di [`dna`, la CLI di Domain Name API](https://github.com/domainreseller/cli): stesso documento
API, stesse classi di sicurezza, stesse regole di retry e stessi codici di errore.

## Indice

- [Installazione](#install)
- [Credenziali](#credentials)
- [Avvio rapido](#quick-start)
- [Configurazione](#configuration)
- [Utilizzo](#usage): [account](#account) · [domini](#domains) · [contatti](#contacts) · [DNS](#dns) · [DNSSEC](#dnssec) · [SSL](#ssl) · [hosting](#hosting) · [ordini e prodotti](#orders-and-products)
- [Richieste e `BaseRequest`](#requests-and-baserequest)
- [Errori](#errors)
- [Sicurezza, retry e limiti di richieste](#safety-retries-and-rate-limits)
- [Dependency injection](#dependency-injection)
- [Testare il tuo codice](#testing-your-code)
- [Struttura del progetto](#project-layout)
- [Sviluppo](#development)
- [Passare dalla 1.x](#upgrading-from-1x)

<a id="install"></a>

## Installazione

> [!IMPORTANT]
> **Usa sempre l'[ultima release](https://github.com/domainreseller/dotnet-dna/releases/latest).** I pacchetti delle release sono versionati e testati.
> Il pulsante verde **Code → Download ZIP** scarica invece il branch di sviluppo.

```bash
dotnet add package DomainNameApi
```

Per installare un `.nupkg` scaricato, mettilo in una cartella e usa quella cartella come sorgente dei pacchetti:

```bash
dotnet add package DomainNameApi --source ./packages
```

Oppure referenzia il progetto dai sorgenti di una release:

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

**Requisiti:** .NET 10 o successivo, oppure qualsiasi runtime che supporti .NET Standard 2.0. Non ci sono dipendenze
di terze parti. Su .NET Standard 2.0 il pacchetto include `System.Text.Json` e `Microsoft.Bcl.AsyncInterfaces` di
Microsoft, già integrati in .NET.

<a id="credentials"></a>

## Credenziali

L'API autentica ogni richiesta con due valori, che l'SDK invia per te:

| Valore | Header | Dove trovarlo |
|---|---|---|
| Reseller ID (un UUID) | `__reseller` | Pannello reseller → **API settings** |
| API key | `X-API-KEY` | Pannello reseller → **API settings** |

> [!NOTE]
> Queste sono **credenziali API**: il login del pannello qui non funziona. **Produzione e OTE (l'ambiente di test) usano
> chiavi API diverse.** Usare l'una con l'altro fallisce con `INVALID_API_KEY` o `INVALID_RESELLER`.

<a id="quick-start"></a>

## Avvio rapido

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

Il [progetto di esempi](examples/DomainNameApi.Examples) esegue scenari come questi su OTE:

```bash
dotnet run --project examples/DomainNameApi.Examples -- search example.com
```

<a id="configuration"></a>

## Configurazione

Ci sono quattro modi per creare un client:

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

| Opzione | Predefinito | Significato |
|---|---|---|
| `ResellerId`, `ApiKey` | (obbligatorie) | Le credenziali. |
| `Environment` | `Production` | `Production` (`https://api.domainresellerapi.com`) oppure `Ote` (`https://ote.domainresellerapi.com`). |
| `BaseAddress` | nessuno | Sostituisce l'indirizzo dell'ambiente, ad esempio per un proxy. |
| `Timeout` | 60 s | Tempo massimo di un singolo tentativo. |
| `MaxRetries` | 2 | Quante volte una richiesta fallita viene ripetuta (vedi [retry](#safety-retries-and-rate-limits)). 0 disattiva i retry. |
| `MaxRetryDelay` | 30 s | L'attesa massima prima di un retry. |
| `PaceRequests` | `true` | Distanzia le richieste per restare entro i limiti di richieste del gateway. |
| `ReadOnly` | `false` | Blocca, prima dell'invio, ogni richiesta che non sia una lettura. |
| `UserAgent` | `DomainNameAPI-DotNet/<version>` | L'header `User-Agent`. |

`FromEnvironment()` legge:

| Variabile | Significato |
|---|---|
| `DNA_RESELLER_ID`, `DNA_API_KEY` | Le credenziali. |
| `DNA_ENV` | `production` oppure `ote` (anche `prod`, `live`, `test`, `sandbox`). |
| `DNA_TEST_MODE` | `true`, `1`, `on` o `yes` seleziona OTE. Usata quando `DNA_ENV` non è impostata. |
| `DNA_BASE_URL` | Sostituzione facoltativa dell'indirizzo base. |
| `DNA_READ_ONLY` | `true` attiva la modalità di sola lettura. |

Il client è thread-safe. Creane uno per ogni set di credenziali e riusalo per tutta la vita dell'applicazione.

<a id="usage"></a>

## Utilizzo

Ogni area dell'API ha un proprio client. I metodi accettano argomenti semplici; per più opzioni, ogni metodo ha anche un
overload (o un metodo affine) che accetta l'oggetto richiesta.

<a id="account"></a>

### Account

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

### Domini

**Ricerca**

```csharp
var one = await client.Domains.SearchAsync("example.com", currency: "USD");
bool available = one.Info?.IsAvailable == true;

var many = await client.Domains.BulkSearchAsync(["example.com", "example.net", "example.com.tr"]);
foreach (var domain in many.Available)
{
    Console.WriteLine($"{domain.DomainName}: {domain.Price} {domain.Currency}");
}
```

**Elenco e dettagli**

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

**Registrazione, rinnovo, trasferimento, ripristino** (addebitano il tuo saldo)

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

Le richieste vengono controllate prima dell'invio. Una registrazione senza tutti e quattro i ruoli di contatto, un contatto senza indirizzo
e-mail o un periodo di 0 anni lanciano un `ArgumentException` e nulla raggiunge l'API.

**Impostazioni**

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

**Inoltro URL, nameserver figli e documenti**

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

### Contatti

Gli handle di contatto sono contatti riutilizzabili. Creane uno una volta e usalo per molti domini.

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

Record di zona dei domini sui nameserver della piattaforma. I record si risolvono pubblicamente quando il dominio usa quei nameserver,
cosa di cui si occupa `ApplyAsync`.

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

L'SDK controlla ciò che l'API rifiuterebbe: un CNAME all'apex, un valore CAA senza le sue tre parti, un TTL fuori dall'intervallo
1–86400. Le chiamate alla zona sono distanziate di 1 secondo, il limite del gateway.

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

### Ordini e prodotti

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

## Richieste e `BaseRequest`

Ogni operazione è una classe di richiesta, ad esempio `RenewDomainRequest`, `ListDomainsRequest` o `AddDnsRecordRequest`.
I metodi del client qui sopra le costruiscono per te, ma puoi anche crearle e inviarle tu stesso:

```csharp
var request = new RenewDomainRequest("example.com", period: 2);
Console.WriteLine($"{request} is {request.Safety}");   // "POST domains/renew is Billable"

DomainOperationResult result = await client.SendAsync(request);

// The same, with the HTTP details: correlation ID, soft-quota state, attempts, duration
var response = await client.SendWithResponseAsync(request);
Console.WriteLine($"{response.CorrelationId} quota {response.Quota?.Used}/{response.Quota?.Limit}");
```

Tutte le richieste derivano da una di tre classi base:

| Classe base | Per |
|---|---|
| `BaseRequest` | Operazioni che non restituiscono nulla (blocco, sblocco, aggiunta di un record DNS, ...). |
| `BaseRequest<TResponse>` | Operazioni che restituiscono dati. |
| `PagedRequest<TItem>` | Operazioni di elenco. Aggiunge `SkipCount`, `MaxResultCount` e `Sorting`, e funziona con `ListAllAsync`. |

Una richiesta dichiara cosa è con alcuni override. Per POST e PUT l'oggetto richiesta stesso è il corpo JSON (camelCase,
senza i valori null). I parametri di query si aggiungono in `AddQueryParameters` e i controlli prima dell'invio vanno in `Validate`.
Per chiamare un endpoint che l'SDK non copre ancora, scrivi la tua richiesta:

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

La tua richiesta ottiene tutto ciò che ottengono quelle integrate: credenziali, retry per classe di sicurezza, distanziamento, modalità di sola lettura e
gestione degli errori. Sovrascrivi `GetBody()` quando l'API si aspetta un corpo di forma diversa e `IsIdempotent` quando una scrittura può
essere ripetuta senza rischi.

<a id="errors"></a>

## Errori

Ogni richiesta fallita lancia una `DomainNameApiException`. Gli argomenti non validi lanciano `ArgumentException` prima che venga
inviato qualsiasi dato.

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

| Proprietà | Significato |
|---|---|
| `Kind` | Una classificazione stabile (tabella sotto). Ramifica su questa, non sul messaggio. |
| `Code` | Il codice di errore dell'API (come `Dna.DomainService:Domain:10007`) o uno dei codici dell'SDK qui sotto. |
| `Message` | Cosa è successo. I messaggi generici vengono sostituiti con i dettagli dell'API. |
| `StatusCode` | Lo stato HTTP, oppure null se non è arrivata alcuna risposta. |
| `Details`, `ValidationErrors` | Dettagli aggiuntivi e problemi a livello di campo restituiti dall'API. |
| `CorrelationId` | L'`X-Correlation-Id` del gateway. Citalo quando contatti il supporto. |
| `Hint` | Un breve suggerimento su cosa fare dopo. |
| `IsRetryable` | Indica se ripetere la stessa richiesta più tardi può riuscire. |
| `OutcomeUnknown` | Una richiesta che modifica qualcosa è fallita in rete, quindi potrebbe essere stata eseguita. |
| `Operation`, `Safety` | La richiesta fallita (`POST domains/renew`) e la sua classe di sicurezza. |
| `RetryAfter`, `ResponseBody` | Il `Retry-After` del gateway e la risposta grezza. |

| `Kind` | Quando |
|---|---|
| `Authentication` | Credenziali errate o mancanti, oppure reseller sconosciuto o sospeso in questo ambiente (401, redirect). |
| `Forbidden` | Autenticato ma non autorizzato (403). |
| `NotFound` | Il dominio, il contatto, l'ordine o un'altra risorsa non esiste (404). |
| `Validation` | L'API ha rifiutato l'input o una regola di business è fallita: campo errato, dominio bloccato, saldo insufficiente (400, 422). |
| `Conflict` | Lo stato della risorsa non consente l'operazione (409). |
| `RateLimited` | Limite di richieste ancora superato dopo i retry automatici (429). |
| `Server` | Un errore lato server (5xx). Spesso temporaneo. |
| `Network`, `Timeout` | Nessuna risposta: DNS, TLS, proxy, connessione interrotta, o nessuna risposta entro `Timeout`. |
| `ReadOnly` | Bloccata dalla modalità di sola lettura; non è stato inviato nulla. |
| `Unknown` | Qualsiasi altro caso, inclusa una risposta che l'SDK non ha saputo leggere. |

Codici impostati dall'SDK stesso:

| Codice | Significato |
|---|---|
| `INVALID_RESELLER` | Il gateway ha reindirizzato alla sua pagina di sospensione: il reseller ID è sconosciuto, inattivo o sospeso **in questo ambiente**. |
| `INVALID_API_KEY` | La chiave non corrisponde al reseller. Le chiavi di produzione e OTE sono diverse. |
| `MISSING_API_KEY`, `MISSING_RESELLER_ID`, `NO_CREDENTIALS` | Una credenziale è vuota. |
| `SUBRESELLER_NOT_ALLOWED` | Gli account sub-reseller non possono usare l'API. Usa le credenziali del reseller principale. |
| `UNAUTHORIZED` | Qualsiasi altro 401. |
| `RATE_LIMITED` | 429 dopo i retry. |
| `NETWORK`, `TIMEOUT` | Non è arrivata alcuna risposta. |
| `READ_ONLY` | Bloccata dalla modalità di sola lettura. |
| `EMPTY_RESPONSE`, `INVALID_RESPONSE` | Una risposta riuscita era vuota o non è stato possibile leggerla. |
| `HTTP_<status>` | L'API non ha inviato alcun codice di errore. |

<a id="safety-retries-and-rate-limits"></a>

## Sicurezza, retry e limiti di richieste

Ogni richiesta ha una classe di sicurezza, la stessa usata dalla CLI `dna`:

| `OperationSafety` | Significato | Esempi |
|---|---|---|
| `Read` | Nessun effetto collaterale, anche quando l'API usa POST o PUT | ricerca, dettagli, elenchi, verifiche di rinnovo e trasferimento, generazione della CSR |
| `Write` | Una modifica gratuita e reversibile | record DNS, blocco, privacy, nameserver, contatti |
| `Billable` | Addebita il tuo saldo | registrazione, rinnovo, trasferimento, ripristino, acquisto SSL, ordine di hosting |
| `Destructive` | Elimina qualcosa o è difficile da annullare | eliminazione di record DNS, rimozione DNSSEC, revoca del certificato |

- **Modalità di sola lettura** (`ReadOnly = true` o `DNA_READ_ONLY=true`) blocca, prima dell'invio, tutto ciò che non è `Read`.
  Usala mentre esplori, nei report e nelle dashboard.
- **Retry.** Le risposte 429 vengono sempre ritentate, rispettando `Retry-After`. È sicuro anche per le richieste a pagamento, perché
  il gateway le rifiuta prima che venga eseguito qualsiasi cosa. Errori di rete, timeout e risposte 502/503/504 vengono ritentati solo per
  le letture (e per le scritture che si possono ripetere senza rischi, come la sostituzione dei nameserver). Una registrazione o un rinnovo non viene mai inviato due volte.
- **Distanziamento.** Il gateway consente 5 richieste al secondo per chiave, 2 al secondo per le ricerche di domini e 20 al secondo per
  le chiamate alla zona DNS. Il client distanzia le richieste per restare entro questi limiti (chiamate alla zona a 1 secondo di distanza). Le informazioni sul dominio e
  l'elenco dei TLD non hanno limiti di richieste.
- **Soft quota.** Le ricerche e le consultazioni di informazioni costano 1 punto, tutto il resto 10. Oltre i 200 punti il gateway rallenta ogni
  richiesta fino a 5 secondi; i punti decadono del 20% al minuto. `SendWithResponseAsync` mostra lo stato in `Quota`.

<a id="dependency-injection"></a>

## Dependency injection

`DomainNameApiClient` funziona come `HttpClient` tipizzato:

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

L'SDK non segue i redirect sul proprio `HttpClient`. Un client creato da una factory può seguirli; l'SDK riconosce comunque
la pagina di sospensione del gateway e segnala `INVALID_RESELLER`.

<a id="testing-your-code"></a>

## Testare il tuo codice

Passa un `HttpClient` con un handler stub per testare il codice che usa l'SDK senza chiamare l'API:

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

Per i test sull'API reale, usa OTE con la sua chiave.

<a id="project-layout"></a>

## Struttura del progetto

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

Ogni cartella di area ha la stessa forma: `XxxClient.cs`, una cartella `Requests/` con una classe per operazione e una cartella `Models/`
con le risposte e gli enum. Tutti i tipi di un'area condividono un unico namespace, ad esempio `DomainNameApi.Domains`.

<a id="development"></a>

## Sviluppo

```bash
dotnet build
dotnet test
```

I test non chiamano mai l'API reale: usano un `HttpMessageHandler` finto. Su Windows girano su .NET 10 e su
.NET Framework 4.8; quest'ultimo esercita la build .NET Standard 2.0.

`tests/DomainNameApi.Tests/testdata/openapi.json` è il documento API che la CLI `dna` incorpora. `SpecCoverageTests` verifica
che ogni operazione al suo interno abbia esattamente una classe di richiesta con la classe di sicurezza della CLI. Quando l'API aggiunge un'operazione,
aggiorna il documento e il test fallisce finché la richiesta non esiste.

<a id="upgrading-from-1x"></a>

## Passare dalla 1.x

La 2.0 è un nuovo SDK. La 1.x era un port della libreria PHP e restituiva envelope `JsonNode` non tipizzati; la 2.0 ha richieste
e modelli tipizzati, metodi async ed eccezioni.

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

La 1.x inviava report di errore a Sentry. La 2.0 non invia nulla a nessuno, tranne che all'API.

## Link

- [CLI `dna` e server MCP](https://github.com/domainreseller/cli): la stessa API dal terminale e dagli agenti AI.
- [Documentazione dell'API](https://www.domainnameapi.com/docs/)
- [Changelog](CHANGELOG.md)

## Licenza

Apache License 2.0. Vedi [LICENSE](LICENSE).
