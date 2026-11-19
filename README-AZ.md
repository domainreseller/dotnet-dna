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

[Domain Name API](https://www.domainnameapi.com) reseller platforması üçün rəsmi .NET SDK. REST API-nin bütün
64 əməliyyatını tipli klientlər və modellərlə əhatə edir: domenlər, əlaqələr, DNS, DNSSEC, SSL sertifikatları,
hostinq, sifarişlər, məhsullar və hesabınız.

```csharp
using var client = new DomainNameApiClient(resellerId, apiKey, ApiEnvironment.Ote);

var search = await client.Domains.SearchAsync("example.com");
if (search.Info?.IsAvailable == true)
{
    await client.Domains.RegisterAsync("example.com", contact, ["tr.apiname.com", "eu.apiname.com"]);
}
```

- **Hər şey tiplidir.** Hər əməliyyat bir sorğu sinfidir, hər cavab isə IntelliSense sənədləşməsi olan modeldir. Lüğət (dictionary) və JSON parsinq yoxdur.
- **Öz sorğularınız.** Hər sorğu `BaseRequest`-dən törəyir. SDK-nın hələ əhatə etmədiyi endpoint üçün ondan alt sinif yaradın.
- **Pulla təhlükəsiz işləyir.** Hər sorğu bildirir ki, oxuyur, yazır, balansınızdan ödəniş alır, yoxsa nəyisə silir. Ödənişli sorğular heç vaxt təkrarlanmır, yalnız oxuma rejimi isə hər dəyişikliyi bloklayır.
- **Daxili davamlılıq.** Sorğu limitlərinə avtomatik əməl edilir, 429 cavabları təkrarlanır, oxuma sorğularında isə şəbəkə xətaları təkrarlanır.
- **Aydın xətalar.** Sabit növü, API-nin xəta kodu, sahə səviyyəsində yoxlama xətaları, korrelyasiya ID-si və məsləhəti olan tək bir istisna tipi.
- **Müasir .NET.** Ləğvetmə dəstəyi olan async, `IAsyncEnumerable` ilə avtomatik səhifələmə, `IHttpClientFactory` dəstəyi, nullable annotasiyaları.
- **Hər yerdə işləyir.** .NET 10 və .NET Standard 2.0-ı dəstəkləyən istənilən runtime (.NET Framework 4.6.2+, .NET 8, Mono, Unity).

O, [`dna`, Domain Name API CLI-nin](https://github.com/domainreseller/cli) .NET qarşılığıdır: eyni API
sənədi, eyni təhlükəsizlik sinifləri, təkrar qaydaları və xəta kodları.

## Məzmun

- [Quraşdırma](#install)
- [Giriş məlumatları](#credentials)
- [Sürətli başlanğıc](#quick-start)
- [Konfiqurasiya](#configuration)
- [İstifadə](#usage): [hesab](#account) · [domenlər](#domains) · [əlaqələr](#contacts) · [DNS](#dns) · [DNSSEC](#dnssec) · [SSL](#ssl) · [hostinq](#hosting) · [sifarişlər və məhsullar](#orders-and-products)
- [Sorğular və `BaseRequest`](#requests-and-baserequest)
- [Xətalar](#errors)
- [Təhlükəsizlik, təkrarlar və sorğu limitləri](#safety-retries-and-rate-limits)
- [Dependency injection](#dependency-injection)
- [Kodunuzu test etmək](#testing-your-code)
- [Layihənin strukturu](#project-layout)
- [İnkişaf](#development)
- [1.x versiyasından keçid](#upgrading-from-1x)

<a id="install"></a>

## Quraşdırma

> [!IMPORTANT]
> **Həmişə [son buraxılışdan](https://github.com/domainreseller/dotnet-dna/releases/latest) istifadə edin.** Buraxılış paketləri versiyalaşdırılır və test edilir.
> Yaşıl **Code → Download ZIP** düyməsi isə inkişaf branch-ini yükləyir.

```bash
dotnet add package DomainNameApi
```

Yüklənmiş `.nupkg` faylını quraşdırmaq üçün onu bir qovluğa qoyun və həmin qovluğu paket mənbəyi kimi istifadə edin:

```bash
dotnet add package DomainNameApi --source ./packages
```

Və ya layihəyə buraxılışın mənbə kodundan istinad edin:

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

**Tələblər:** .NET 10 və ya daha yeni versiya, ya da .NET Standard 2.0-ı dəstəkləyən istənilən runtime. Üçüncü tərəf
asılılığı yoxdur. .NET Standard 2.0-da paket Microsoft-un .NET-ə daxil olan `System.Text.Json` və
`Microsoft.Bcl.AsyncInterfaces` paketlərini əlavə edir.

<a id="credentials"></a>

## Giriş məlumatları

API hər sorğunu iki dəyərlə autentifikasiya edir, SDK onları sizin yerinizə göndərir:

| Dəyər | Header | Harada tapılır |
|---|---|---|
| Reseller ID (UUID) | `__reseller` | Reseller paneli → **API settings** |
| API açarı | `X-API-KEY` | Reseller paneli → **API settings** |

> [!NOTE]
> Bunlar **API giriş məlumatlarıdır**: panelə giriş məlumatlarınız burada işləmir. **Canlı mühit və OTE (test mühiti)
> fərqli API açarlarından istifadə edir.** Birini digərində işlətmək `INVALID_API_KEY` və ya `INVALID_RESELLER` xətası ilə nəticələnir.

<a id="quick-start"></a>

## Sürətli başlanğıc

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

[Nümunələr layihəsi](examples/DomainNameApi.Examples) bu kimi ssenariləri OTE-yə qarşı işə salır:

```bash
dotnet run --project examples/DomainNameApi.Examples -- search example.com
```

<a id="configuration"></a>

## Konfiqurasiya

Klient yaratmağın dörd yolu var:

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

| Seçim | Defolt | Mənası |
|---|---|---|
| `ResellerId`, `ApiKey` | (məcburi) | Giriş məlumatları. |
| `Environment` | `Production` | `Production` (`https://api.domainresellerapi.com`) və ya `Ote` (`https://ote.domainresellerapi.com`). |
| `BaseAddress` | yoxdur | Mühitin ünvanını əvəz edir, məsələn proxy üçün. |
| `Timeout` | 60 s | Bir cəhdin vaxt limiti. |
| `MaxRetries` | 2 | Uğursuz sorğunun neçə dəfə təkrarlanacağı ([təkrarlara](#safety-retries-and-rate-limits) baxın). 0 təkrarları söndürür. |
| `MaxRetryDelay` | 30 s | Təkrardan əvvəl ən uzun gözləmə. |
| `PaceRequests` | `true` | Gateway-in sorğu limitlərinin altında qalmaq üçün sorğular arasında fasilə qoyur. |
| `ReadOnly` | `false` | Oxuma olmayan hər sorğunu göndərilməzdən əvvəl bloklayır. |
| `UserAgent` | `DomainNameAPI-DotNet/<version>` | `User-Agent` header-i. |

`FromEnvironment()` bunları oxuyur:

| Dəyişən | Mənası |
|---|---|
| `DNA_RESELLER_ID`, `DNA_API_KEY` | Giriş məlumatları. |
| `DNA_ENV` | `production` və ya `ote` (həmçinin `prod`, `live`, `test`, `sandbox`). |
| `DNA_TEST_MODE` | `true`, `1`, `on` və ya `yes` OTE-ni seçir. `DNA_ENV` təyin edilməyibsə istifadə olunur. |
| `DNA_BASE_URL` | İstəyə bağlı baza ünvanı əvəzləməsi. |
| `DNA_READ_ONLY` | `true` yalnız oxuma rejimini aktiv edir. |

Klient thread-safe-dir. Hər giriş məlumatı dəsti üçün bir dənə yaradın və tətbiqin bütün işləmə müddətində təkrar istifadə edin.

<a id="usage"></a>

## İstifadə

API-nin hər sahəsinin öz klienti var. Metodlar sadə arqumentlər qəbul edir; daha çox seçim üçün hər metodun sorğu obyektini
qəbul edən overload-u (və ya qardaş metodu) də var.

<a id="account"></a>

### Hesab

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

### Domenlər

**Axtarış**

```csharp
var one = await client.Domains.SearchAsync("example.com", currency: "USD");
bool available = one.Info?.IsAvailable == true;

var many = await client.Domains.BulkSearchAsync(["example.com", "example.net", "example.com.tr"]);
foreach (var domain in many.Available)
{
    Console.WriteLine($"{domain.DomainName}: {domain.Price} {domain.Currency}");
}
```

**Siyahı və məlumat**

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

**Qeydiyyat, yenilənmə, transfer, bərpa** (bunlar balansınızdan ödəniş alır)

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

Sorğular göndərilməzdən əvvəl yoxlanılır. Dörd əlaqə rolunun hamısı olmayan qeydiyyat, e-poçt ünvanı olmayan əlaqə
və ya 0 illik müddət `ArgumentException` atır və API-yə heç nə çatmır.

**Parametrlər**

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

**URL yönləndirmə, child nameserver-lər və sənədlər**

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

### Əlaqələr

Əlaqə handle-ları təkrar istifadə edilə bilən əlaqələrdir. Birini bir dəfə yaradın və çox domen üçün istifadə edin.

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

Platforma nameserver-lərindəki domenlərin zona qeydləri. Domen həmin nameserver-lərdən istifadə etməyə başladıqdan sonra
qeydlər hamıya açıq şəkildə həll olunur; bunu `ApplyAsync` həyata keçirir.

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

SDK API-nin rədd edəcəyi halları əvvəlcədən yoxlayır: apex-də CNAME, üç hissəsi olmayan CAA dəyəri, 1–86400 aralığından
kənar TTL. Zona çağırışları bir-birindən 1 saniyə aralı göndərilir, bu gateway-in limitidir.

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

### Hostinq

```csharp
var packages = await client.Hosting.ListPackagesAsync();
var created = await client.Hosting.CreateAsync("example.com", "starter-linux");   // charges the balance
var status = await client.Hosting.GetStatusAsync("example.com");                   // panel access once ready
var hosting = await client.Hosting.GetAsync("example.com");
var plans = await client.Hosting.ListPlansAsync();
```

<a id="orders-and-products"></a>

### Sifarişlər və məhsullar

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

## Sorğular və `BaseRequest`

Hər əməliyyat bir sorğu sinfidir, məsələn `RenewDomainRequest`, `ListDomainsRequest` və ya `AddDnsRecordRequest`.
Yuxarıdakı klient metodları onları sizin üçün qurur, siz də onları özünüz yarada və göndərə bilərsiniz:

```csharp
var request = new RenewDomainRequest("example.com", period: 2);
Console.WriteLine($"{request} is {request.Safety}");   // "POST domains/renew is Billable"

DomainOperationResult result = await client.SendAsync(request);

// The same, with the HTTP details: correlation ID, soft-quota state, attempts, duration
var response = await client.SendWithResponseAsync(request);
Console.WriteLine($"{response.CorrelationId} quota {response.Quota?.Used}/{response.Quota?.Limit}");
```

Bütün sorğular üç baza sinfindən birindən törəyir:

| Baza sinif | Nə üçün |
|---|---|
| `BaseRequest` | Heç nə qaytarmayan əməliyyatlar (kilid, kilidi açmaq, DNS qeydi əlavə etmək, ...). |
| `BaseRequest<TResponse>` | Məlumat qaytaran əməliyyatlar. |
| `PagedRequest<TItem>` | Siyahı əməliyyatları. `SkipCount`, `MaxResultCount` və `Sorting` əlavə edir və `ListAllAsync` ilə işləyir. |

Sorğu özü haqqında bir neçə override ilə məlumat verir. POST və PUT üçün sorğu obyektinin özü JSON gövdəsidir (camelCase,
null dəyərlər buraxılır). Query parametrləri `AddQueryParameters` içində əlavə olunur, göndərmədən əvvəlki yoxlamalar isə `Validate` içinə yazılır.
SDK-nın hələ əhatə etmədiyi endpoint-i çağırmaq üçün özünüz yazın:

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

Sizin sorğunuz daxili sorğuların aldığı hər şeyi alır: giriş məlumatları, təhlükəsizlik sinfinə görə təkrarlar, sorğu tempi, yalnız oxuma
rejimi və xəta emalı. API fərqli gövdə forması gözləyirsə `GetBody()`, yazma sorğusunun təkrarlanması təhlükəsizdirsə
`IsIdempotent` override edin.

<a id="errors"></a>

## Xətalar

Hər uğursuz sorğu `DomainNameApiException` atır. Yanlış arqumentlər heç nə göndərilməmişdən əvvəl `ArgumentException` atır.

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

| Xassə | Mənası |
|---|---|
| `Kind` | Sabit təsnifat (aşağıdakı cədvəl). Mesaja görə deyil, buna görə şaxələnin. |
| `Code` | API-nin xəta kodu (məsələn `Dna.DomainService:Domain:10007`) və ya aşağıdakı SDK kodlarından biri. |
| `Message` | Nə baş verdi. Şablon mesajlar API-nin təfərrüatları ilə əvəz olunur. |
| `StatusCode` | HTTP statusu, cavab gəlmədikdə isə null. |
| `Details`, `ValidationErrors` | API-dən gələn əlavə təfərrüat və sahə səviyyəsində problemlər. |
| `CorrelationId` | Gateway-in `X-Correlation-Id` dəyəri. Dəstəyə müraciət edəndə onu göstərin. |
| `Hint` | Növbəti addım üçün qısa təklif. |
| `IsRetryable` | Eyni sorğunu sonra təkrarlamağın uğurlu ola bilib-bilməməsi. |
| `OutcomeUnknown` | Nəyisə dəyişən sorğu şəbəkədə uğursuz oldu, ona görə icra olunmuş ola bilər. |
| `Operation`, `Safety` | Uğursuz sorğu (`POST domains/renew`) və onun təhlükəsizlik sinfi. |
| `RetryAfter`, `ResponseBody` | Gateway-in `Retry-After` dəyəri və xam cavab. |

| `Kind` | Nə vaxt |
|---|---|
| `Authentication` | Yanlış və ya çatışmayan giriş məlumatları, ya da bu mühitdə naməlum və ya dayandırılmış reseller (401, yönləndirmə). |
| `Forbidden` | Autentifikasiya olunub, amma icazə yoxdur (403). |
| `NotFound` | Domen, əlaqə, sifariş və ya digər resurs mövcud deyil (404). |
| `Validation` | API girişi rədd etdi və ya biznes qaydası pozuldu: yanlış sahə, domen kilidlidir, balans çatmır (400, 422). |
| `Conflict` | Resursun vəziyyəti əməliyyata icazə vermir (409). |
| `RateLimited` | Avtomatik təkrarlardan sonra da sorğu limiti aşılıb (429). |
| `Server` | Server tərəfində xəta (5xx). Çox vaxt müvəqqətidir. |
| `Network`, `Timeout` | Cavab yoxdur: DNS, TLS, proxy, bağlantının kəsilməsi və ya `Timeout` daxilində cavab gəlməməsi. |
| `ReadOnly` | Yalnız oxuma rejimi tərəfindən bloklandı; heç nə göndərilmədi. |
| `Unknown` | Qalan hər şey, o cümlədən SDK-nın oxuya bilmədiyi cavab. |

SDK-nın özünün təyin etdiyi kodlar:

| Kod | Mənası |
|---|---|
| `INVALID_RESELLER` | Gateway öz dayandırma səhifəsinə yönləndirdi: reseller ID **bu mühitdə** naməlum, qeyri-aktiv və ya dayandırılmışdır. |
| `INVALID_API_KEY` | Açar reseller ilə uyğun gəlmir. Canlı mühit və OTE açarları fərqlidir. |
| `MISSING_API_KEY`, `MISSING_RESELLER_ID`, `NO_CREDENTIALS` | Giriş məlumatlarından biri boşdur. |
| `SUBRESELLER_NOT_ALLOWED` | Alt reseller hesabları API-dən istifadə edə bilməz. Əsas reseller-in giriş məlumatlarından istifadə edin. |
| `UNAUTHORIZED` | Digər bütün 401 cavabları. |
| `RATE_LIMITED` | Təkrarlardan sonra 429. |
| `NETWORK`, `TIMEOUT` | Cavab gəlmədi. |
| `READ_ONLY` | Yalnız oxuma rejimi tərəfindən bloklandı. |
| `EMPTY_RESPONSE`, `INVALID_RESPONSE` | Uğurlu cavab boş idi və ya oxunmadı. |
| `HTTP_<status>` | API xəta kodu göndərmədi. |

<a id="safety-retries-and-rate-limits"></a>

## Təhlükəsizlik, təkrarlar və sorğu limitləri

Hər sorğunun təhlükəsizlik sinfi var, `dna` CLI-nin istifadə etdiyi ilə eynidir:

| `OperationSafety` | Mənası | Nümunələr |
|---|---|---|
| `Read` | Yan təsiri yoxdur, API POST və ya PUT istifadə etsə belə | axtarış, məlumat, siyahılar, yenilənmə və transfer yoxlamaları, CSR yaradılması |
| `Write` | Geri qaytarıla bilən pulsuz dəyişiklik | DNS qeydləri, kilid, məxfilik, nameserver-lər, əlaqələr |
| `Billable` | Balansınızdan ödəniş alır | qeydiyyat, yenilənmə, transfer, bərpa, SSL alışı, hostinq sifarişi |
| `Destructive` | Nəyisə silir və ya geri qaytarmaq çətindir | DNS qeydinin silinməsi, DNSSEC-in götürülməsi, sertifikatın ləğvi |

- **Yalnız oxuma rejimi** (`ReadOnly = true` və ya `DNA_READ_ONLY=true`) `Read` olmayan hər şeyi göndərilməzdən əvvəl bloklayır.
  Onu araşdırma zamanı, hesabatlarda və dashboard-larda istifadə edin.
- **Təkrarlar.** 429 cavabları həmişə təkrarlanır, `Retry-After` nəzərə alınır. Bu, ödənişli sorğular üçün də təhlükəsizdir,
  çünki gateway onları heç nə icra olunmamış rədd edir. Şəbəkə xətaları, timeout-lar və 502/503/504 cavabları yalnız
  oxuma sorğuları üçün (və təkrarlanması təhlükəsiz olan yazma sorğuları, məsələn nameserver-lərin əvəzlənməsi üçün) təkrarlanır.
  Qeydiyyat və ya yenilənmə heç vaxt iki dəfə göndərilmir.
- **Templəmə.** Gateway hər açar üçün saniyədə 5 sorğuya, domen axtarışları üçün saniyədə 2, DNS zona çağırışları üçün
  saniyədə 20 sorğuya icazə verir. Klient bu limitlərin altında qalmaq üçün sorğular arasında fasilə qoyur (zona çağırışları 1 saniyə aralı). Domen məlumatı və
  TLD siyahısı üçün sorğu limiti yoxdur.
- **Yumşaq kvota.** Axtarışlar və məlumat sorğuları 1 xal, qalan hər şey 10 xal çəkir. 200 xaldan sonra gateway hər
  sorğunu 5 saniyəyə qədər ləngidir; xallar dəqiqədə 20% azalır. `SendWithResponseAsync` vəziyyəti `Quota` içində göstərir.

<a id="dependency-injection"></a>

## Dependency injection

`DomainNameApiClient` tipli `HttpClient` kimi işləyir:

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

SDK öz `HttpClient`-ində yönləndirmələri izləmir. Factory ilə yaradılmış klient onları izləyə bilər; SDK yenə də
gateway-in dayandırma səhifəsini tanıyır və `INVALID_RESELLER` bildirir.

<a id="testing-your-code"></a>

## Kodunuzu test etmək

SDK-dan istifadə edən kodu API-ni çağırmadan test etmək üçün stub handler olan `HttpClient` ötürün:

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

Real API-yə qarşı testlər üçün öz açarı olan OTE-dən istifadə edin.

<a id="project-layout"></a>

## Layihənin strukturu

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

Hər sahə qovluğu eyni formadadır: `XxxClient.cs`, hər əməliyyat üçün bir sinif olan `Requests/` qovluğu və cavabları ilə
enum-ları saxlayan `Models/` qovluğu. Bir sahənin bütün tipləri eyni namespace-i paylaşır, məsələn `DomainNameApi.Domains`.

<a id="development"></a>

## İnkişaf

```bash
dotnet build
dotnet test
```

Testlər heç vaxt real API-ni çağırmır: saxta `HttpMessageHandler` istifadə edirlər. Windows-da .NET 10-da və
.NET Framework 4.8-də işləyirlər; sonuncu .NET Standard 2.0 build-ini yoxlayır.

`tests/DomainNameApi.Tests/testdata/openapi.json` `dna` CLI-nin daxilinə yerləşdirdiyi API sənədidir. `SpecCoverageTests` yoxlayır ki,
ondakı hər əməliyyatın CLI-nin təhlükəsizlik sinfi ilə tam bir sorğu sinfi var. API yeni əməliyyat qazandıqda
sənədi yeniləyin; sorğu yaradılana qədər test uğursuz olacaq.

<a id="upgrading-from-1x"></a>

## 1.x versiyasından keçid

2.0 yeni SDK-dır. 1.x PHP kitabxanasının port-u idi və tipsiz `JsonNode` zərfləri qaytarırdı; 2.0-da tipli sorğular
və modellər, async metodlar və istisnalar var.

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

1.x xəta hesabatlarını Sentry-yə göndərirdi. 2.0 API-dən başqa heç yerə heç nə göndərmir.

## Keçidlər

- [`dna` CLI və MCP serveri](https://github.com/domainreseller/cli): eyni API terminaldan və AI agentlərindən.
- [API sənədləri](https://www.domainnameapi.com/docs/)
- [Dəyişikliklər jurnalı](CHANGELOG.md)

## Lisenziya

Apache License 2.0. [LICENSE](LICENSE) faylına baxın.
