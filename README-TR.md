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

[Domain Name API](https://www.domainnameapi.com) bayi platformunun resmi .NET SDK'sı. REST API'nin 67 işleminin
tamamını tipli istemciler ve modellerle kapsar: alan adları, iletişim bilgileri, DNS, DNSSEC, SSL sertifikaları,
hosting, siparişler, ürünler ve hesabınız.

```csharp
using var client = new DomainNameApiClient(resellerId, apiKey, ApiEnvironment.Ote);

var search = await client.Domains.SearchAsync("example.com");
if (search.Info?.IsAvailable == true)
{
    await client.Domains.RegisterAsync("example.com", contact, ["tr.apiname.com", "eu.apiname.com"]);
}
```

- **Her şey tipli.** Her işlem bir istek sınıfı, her yanıt IntelliSense dokümantasyonlu bir modeldir. Sözlük yok, JSON ayrıştırma yok.
- **Kendi istekleriniz.** Her istek `BaseRequest`'ten türer. SDK'nın henüz kapsamadığı bir uç nokta için ondan kendi sınıfınızı türetin.
- **Para harcayan işlemlerde güvenli.** Her istek, okuma mı yaptığını, yazma mı yaptığını, bakiyenizden ücret mi aldığını yoksa bir şey mi sildiğini bildirir. Ücretli istekler asla yeniden denenmez, salt-okunur mod ise her değişikliği engeller.
- **Yerleşik dayanıklılık.** Hız sınırlarına otomatik uyulur, 429 yanıtları yeniden denenir, okumalarda ağ hataları yeniden denenir.
- **Net hatalar.** Sabit bir türü, API'nin hata kodunu, alan düzeyinde doğrulama hatalarını, correlation ID'yi ve bir ipucunu taşıyan tek bir istisna türü.
- **Modern .NET.** İptal destekli async, `IAsyncEnumerable` ile otomatik sayfalama, `IHttpClientFactory` desteği, nullable ek açıklamaları.
- **Her yerde çalışır.** .NET 10 ve .NET Standard 2.0'ı destekleyen her çalışma zamanı (.NET Framework 4.6.2+, .NET 8, Mono, Unity).

[Domain Name API CLI'si `dna`](https://github.com/domainreseller/cli)'nın .NET karşılığıdır: aynı API
dokümanı, aynı güvenlik sınıfları, yeniden deneme kuralları ve hata kodları.

## İçindekiler

- [Kurulum](#install)
- [Kimlik bilgileri](#credentials)
- [Hızlı başlangıç](#quick-start)
- [Yapılandırma](#configuration)
- [Kullanım](#usage): [hesap](#account) · [alan adları](#domains) · [iletişim bilgileri](#contacts) · [DNS](#dns) · [DNSSEC](#dnssec) · [SSL](#ssl) · [hosting](#hosting) · [siparişler ve ürünler](#orders-and-products)
- [İstekler ve `BaseRequest`](#requests-and-baserequest)
- [Hatalar](#errors)
- [Güvenlik, yeniden denemeler ve hız sınırları](#safety-retries-and-rate-limits)
- [Dependency injection](#dependency-injection)
- [Kodunuzu test etme](#testing-your-code)
- [Proje yapısı](#project-layout)
- [Geliştirme](#development)
- [1.x'ten yükseltme](#upgrading-from-1x)

<a id="install"></a>

## Kurulum

> [!IMPORTANT]
> **Her zaman [en son sürümü](https://github.com/domainreseller/dotnet-dna/releases/latest) kullanın.** Yayınlanan paketler sürüm numaralıdır ve test edilmiştir.
> Yeşil **Code → Download ZIP** düğmesi ise geliştirme dalını indirir.

```bash
dotnet add package DomainNameApi
```

İndirdiğiniz bir `.nupkg` dosyasını kurmak için onu bir klasöre koyun ve o klasörü paket kaynağı olarak kullanın:

```bash
dotnet add package DomainNameApi --source ./packages
```

Ya da bir sürümün kaynağındaki projeye referans verin:

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

**Gereksinimler:** .NET 10 veya üstü ya da .NET Standard 2.0'ı destekleyen herhangi bir çalışma zamanı. Üçüncü taraf
bağımlılığı yoktur. .NET Standard 2.0'da paket, Microsoft'un .NET'e dahil olan `System.Text.Json` ve
`Microsoft.Bcl.AsyncInterfaces` paketlerini ekler.

<a id="credentials"></a>

## Kimlik bilgileri

API her isteği iki değerle doğrular; SDK bunları sizin yerinize gönderir:

| Değer | Başlık | Nerede bulunur |
|---|---|---|
| Reseller ID (bir UUID) | `__reseller` | Bayi paneli → **API ayarları** |
| API anahtarı | `X-API-KEY` | Bayi paneli → **API ayarları** |

> [!NOTE]
> Bunlar **API kimlik bilgileridir**: panel girişiniz burada çalışmaz. **Canlı ortam ve OTE (test ortamı) farklı
> API anahtarları kullanır.** Birini diğerinde kullanmak `INVALID_API_KEY` veya `INVALID_RESELLER` hatasıyla sonuçlanır.

<a id="quick-start"></a>

## Hızlı başlangıç

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

[Örnekler projesi](examples/DomainNameApi.Examples), bunlara benzer senaryoları OTE'ye karşı çalıştırır:

```bash
dotnet run --project examples/DomainNameApi.Examples -- search example.com
```

<a id="configuration"></a>

## Yapılandırma

Bir istemci oluşturmanın dört yolu vardır:

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

| Seçenek | Varsayılan | Anlamı |
|---|---|---|
| `ResellerId`, `ApiKey` | (zorunlu) | Kimlik bilgileri. |
| `Environment` | `Production` | `Production` (`https://api.domainresellerapi.com`) veya `Ote` (`https://ote.domainresellerapi.com`). |
| `BaseAddress` | yok | Ortamın adresini geçersiz kılar, örneğin bir proxy için. |
| `Timeout` | 60 sn | Tek bir denemenin zaman sınırı. |
| `MaxRetries` | 2 | Başarısız bir isteğin kaç kez tekrarlanacağı ([yeniden denemeler](#safety-retries-and-rate-limits) bölümüne bakın). 0, yeniden denemeyi kapatır. |
| `MaxRetryDelay` | 30 sn | Bir yeniden denemeden önceki en uzun bekleme. |
| `PaceRequests` | `true` | İstekleri, geçidin hız sınırlarının altında kalacak şekilde aralıklar. |
| `ReadOnly` | `false` | Okuma olmayan her isteği, gönderilmeden önce engeller. |
| `UserAgent` | `DomainNameAPI-DotNet/<version>` | `User-Agent` başlığı. |

`FromEnvironment()` şunları okur:

| Değişken | Anlamı |
|---|---|
| `DNA_RESELLER_ID`, `DNA_API_KEY` | Kimlik bilgileri. |
| `DNA_ENV` | `production` veya `ote` (ayrıca `prod`, `live`, `test`, `sandbox`). |
| `DNA_TEST_MODE` | `true`, `1`, `on` veya `yes` OTE'yi seçer. `DNA_ENV` ayarlı değilse kullanılır. |
| `DNA_BASE_URL` | İsteğe bağlı temel adres geçersiz kılma. |
| `DNA_READ_ONLY` | `true`, salt-okunur modu açar. |

İstemci thread-safe'tir. Her kimlik bilgisi kümesi için bir tane oluşturun ve uygulamanın ömrü boyunca yeniden kullanın.

<a id="usage"></a>

## Kullanım

API'nin her alanının kendi istemcisi vardır. Metotlar basit argümanlar alır; daha fazla seçenek için her metodun istek
nesnesini alan bir aşırı yüklemesi (veya kardeş metodu) da vardır.

<a id="account"></a>

### Hesap

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

### Alan adları

**Arama**

```csharp
var one = await client.Domains.SearchAsync("example.com", currency: "USD");
bool available = one.Info?.IsAvailable == true;

var many = await client.Domains.BulkSearchAsync(["example.com", "example.net", "example.com.tr"]);
foreach (var domain in many.Available)
{
    Console.WriteLine($"{domain.DomainName}: {domain.Price} {domain.Currency}");
}
```

**Listeleme ve ayrıntılar**

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

**Kayıt, yenileme, transfer, geri yükleme** (bunlar bakiyenizden ücret alır)

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

İstekler gönderilmeden önce denetlenir. Dört iletişim rolünün tamamını içermeyen bir kayıt, e-posta adresi olmayan bir
iletişim kişisi veya 0 yıllık bir süre `ArgumentException` fırlatır ve API'ye hiçbir şey ulaşmaz.

**Ayarlar**

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

**URL yönlendirme, child nameserver'lar ve belgeler**

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

### İletişim bilgileri

İletişim kişisi tanımlayıcıları (handle), yeniden kullanılabilir iletişim kayıtlarıdır. Bir kez oluşturun, birçok alan adında kullanın.

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

Platformun nameserver'larındaki alan adlarının bölge (zone) kayıtları. Alan adı bu nameserver'ları kullanmaya başlayınca
kayıtlar herkese açık olarak çözümlenir; bunu `ApplyAsync` sağlar.

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

SDK, API'nin reddedeceği durumları önceden denetler: apex'te CNAME, üç parçası eksik bir CAA değeri, 1–86400 dışında
bir TTL. Bölge çağrıları, geçidin sınırı olan 1 saniye arayla yapılır.

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

Bir alan adının birden fazla sertifikası olabilir; bu nedenle mevcut bir sertifika alan adıyla değil,
**sertifika kimliği** (bir `Guid`) ile belirtilir. `PurchaseAsync` ve `RenewAsync` bu kimliği döndürür: saklayın.
`ListAsync`, elinizdeki sertifikaların kimliklerini bulur.

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

**Sertifikaları listeleme ve kimlik bulma**

```csharp
var page = await client.Ssl.ListAsync(new ListCertificatesRequest { DomainName = "example.com" });   // newest first

await foreach (var certificate in client.Ssl.ListAllAsync())
{
    Console.WriteLine($"{certificate.CertificateId} {certificate.DomainName} {certificate.Status} {certificate.SubscriptionExpiresAt:d}");
}

// The approver addresses of a certificate you already have
var approverEmails = await client.Ssl.GetApproverEmailsAsync(certificateId);
```

**Yenileme** (bu işlem bakiyenizden ücret alır)

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

Satın alınan veya yenilenen bir sertifika 15 gün içinde yapılandırılmalıdır; aksi halde iptal edilir. Yenilemek için
önce `RenewAsync`, ardından yeni kimlikle `ConfigureAsync` çağırın. `PurchaseAsync` ile yeniden satın almayın: bu, satın
alma fiyatı üzerinden ayrı bir satın alma olur ve eski sertifika hiçbir zaman yenilenmiş olarak işaretlenmez. Boş bir
sertifika kimliği (`Guid.Empty`), hiçbir şey gönderilmeden önce `ArgumentException` fırlatır.

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

### Siparişler ve ürünler

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

## İstekler ve `BaseRequest`

Her işlem bir istek sınıfıdır; örneğin `RenewDomainRequest`, `ListDomainsRequest` veya `AddDnsRecordRequest`.
Yukarıdaki istemci metotları bunları sizin yerinize oluşturur; isterseniz bunları kendiniz de oluşturup gönderebilirsiniz:

```csharp
var request = new RenewDomainRequest("example.com", period: 2);
Console.WriteLine($"{request} is {request.Safety}");   // "POST domains/renew is Billable"

DomainOperationResult result = await client.SendAsync(request);

// The same, with the HTTP details: correlation ID, soft-quota state, attempts, duration
var response = await client.SendWithResponseAsync(request);
Console.WriteLine($"{response.CorrelationId} quota {response.Quota?.Used}/{response.Quota?.Limit}");
```

Tüm istekler üç temel sınıftan birinden türer:

| Temel sınıf | Ne için |
|---|---|
| `BaseRequest` | Hiçbir şey döndürmeyen işlemler (kilitleme, kilit açma, DNS kaydı ekleme, ...). |
| `BaseRequest<TResponse>` | Veri döndüren işlemler. |
| `PagedRequest<TItem>` | Liste işlemleri. `SkipCount`, `MaxResultCount` ve `Sorting` ekler, `ListAllAsync` ile çalışır. |

Bir istek, ne olduğunu birkaç override ile bildirir. POST ve PUT için istek nesnesinin kendisi JSON gövdesidir
(camelCase, null değerler dışarıda bırakılır). Sorgu parametreleri `AddQueryParameters` içinde eklenir, göndermeden
önceki denetimler `Validate` içine yazılır. SDK'nın henüz kapsamadığı bir uç noktayı çağırmak için kendi isteğinizi yazın:

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

İsteğiniz, yerleşik isteklerin aldığı her şeyi alır: kimlik bilgileri, güvenlik sınıfına göre yeniden denemeler, hız
ayarlama, salt-okunur mod ve hata yönetimi. API farklı bir gövde biçimi bekliyorsa `GetBody()` metodunu, bir yazma
işlemi tekrarlanırken güvenliyse `IsIdempotent` özelliğini override edin.

<a id="errors"></a>

## Hatalar

Başarısız her istek bir `DomainNameApiException` fırlatır. Geçersiz argümanlar, hiçbir şey gönderilmeden önce
`ArgumentException` fırlatır.

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

| Özellik | Anlamı |
|---|---|
| `Kind` | Sabit bir sınıflandırma (aşağıdaki tablo). Mesaja göre değil, buna göre dallanın. |
| `Code` | API'nin hata kodu (örneğin `Dna.DomainService:Domain:10007`) veya aşağıdaki SDK kodlarından biri. |
| `Message` | Ne olduğu. Kalıp mesajların yerine API'nin ayrıntıları konur. |
| `StatusCode` | HTTP durumu; yanıt gelmediyse null. |
| `Details`, `ValidationErrors` | API'den gelen ek ayrıntılar ve alan düzeyindeki sorunlar. |
| `CorrelationId` | Geçidin `X-Correlation-Id` değeri. Destekle iletişime geçerken bunu belirtin. |
| `Hint` | Sonra ne yapılacağına dair kısa bir öneri. |
| `IsRetryable` | Aynı isteği daha sonra tekrarlamanın başarılı olup olamayacağı. |
| `OutcomeUnknown` | Bir şeyi değiştiren istek ağda başarısız oldu, bu yüzden çalışmış olabilir. |
| `Operation`, `Safety` | Başarısız olan istek (`POST domains/renew`) ve onun güvenlik sınıfı. |
| `RetryAfter`, `ResponseBody` | Geçidin `Retry-After` değeri ve ham yanıt. |

| `Kind` | Ne zaman |
|---|---|
| `Authentication` | Yanlış veya eksik kimlik bilgileri ya da bu ortamda bilinmeyen veya askıya alınmış bir bayi (401, yönlendirme). |
| `Forbidden` | Kimlik doğrulandı ama izin yok (403). |
| `NotFound` | Alan adı, iletişim kişisi, sipariş veya başka bir kaynak yok (404). |
| `Validation` | API girdiyi reddetti ya da bir iş kuralı başarısız oldu: hatalı alan, kilitli alan adı, yetersiz bakiye (400, 422). |
| `Conflict` | Kaynağın durumu işleme izin vermiyor (409). |
| `RateLimited` | Otomatik yeniden denemelerden sonra hâlâ hız sınırında (429). |
| `Server` | Sunucu tarafı hata (5xx). Çoğu zaman geçicidir. |
| `Network`, `Timeout` | Yanıt yok: DNS, TLS, proxy, bağlantı sıfırlanması veya `Timeout` içinde yanıt gelmemesi. |
| `ReadOnly` | Salt-okunur mod tarafından engellendi; hiçbir şey gönderilmedi. |
| `Unknown` | SDK'nın okuyamadığı bir yanıt dahil, geri kalan her şey. |

SDK'nın kendisinin belirlediği kodlar:

| Kod | Anlamı |
|---|---|
| `INVALID_RESELLER` | Geçit, askıya alma sayfasına yönlendirdi: reseller ID **bu ortamda** bilinmiyor, etkin değil veya askıya alınmış. |
| `INVALID_API_KEY` | Anahtar bayiyle eşleşmiyor. Canlı ortam ve OTE anahtarları farklıdır. |
| `MISSING_API_KEY`, `MISSING_RESELLER_ID`, `NO_CREDENTIALS` | Bir kimlik bilgisi boş. |
| `SUBRESELLER_NOT_ALLOWED` | Alt bayi hesapları API'yi kullanamaz. Ana bayinin kimlik bilgilerini kullanın. |
| `UNAUTHORIZED` | Diğer tüm 401 yanıtları. |
| `RATE_LIMITED` | Yeniden denemelerden sonra 429. |
| `NETWORK`, `TIMEOUT` | Yanıt gelmedi. |
| `READ_ONLY` | Salt-okunur mod tarafından engellendi. |
| `EMPTY_RESPONSE`, `INVALID_RESPONSE` | Başarılı bir yanıt boştu veya okunamadı. |
| `HTTP_<status>` | API hata kodu göndermedi. |

<a id="safety-retries-and-rate-limits"></a>

## Güvenlik, yeniden denemeler ve hız sınırları

Her isteğin bir güvenlik sınıfı vardır; `dna` CLI'nin kullandığıyla aynıdır:

| `OperationSafety` | Anlamı | Örnekler |
|---|---|---|
| `Read` | API POST veya PUT kullansa bile yan etkisi yok | arama, ayrıntılar, listeler, yenileme ve transfer denetimleri, CSR üretimi |
| `Write` | Geri alınabilen ücretsiz bir değişiklik | DNS kayıtları, kilit, gizlilik, nameserver'lar, iletişim bilgileri |
| `Billable` | Bakiyenizden ücret alır | kayıt, yenileme, transfer, geri yükleme, SSL satın alma ve yenileme, hosting siparişi |
| `Destructive` | Bir şeyi siler veya geri alması zordur | DNS kaydı silme, DNSSEC kaldırma, sertifika iptali |

- **Salt-okunur mod** (`ReadOnly = true` veya `DNA_READ_ONLY=true`), `Read` olmayan her şeyi gönderilmeden önce engeller.
  Keşif yaparken, raporlarda ve panolarda kullanın.
- **Yeniden denemeler.** 429 yanıtları her zaman, `Retry-After` dikkate alınarak yeniden denenir. Bu, ücretli istekler
  için bile güvenlidir, çünkü geçit bunları hiçbir şey çalışmadan önce reddeder. Ağ hataları, zaman aşımları ve
  502/503/504 yanıtları yalnızca okumalarda (ve nameserver değiştirme gibi tekrarlanması güvenli yazmalarda)
  yeniden denenir. Bir kayıt veya yenileme asla iki kez gönderilmez.
- **Hız ayarlama.** Geçit, anahtar başına saniyede 5 isteğe, alan adı aramalarında saniyede 2 isteğe ve DNS bölge
  çağrılarında saniyede 20 isteğe izin verir. İstemci, istekleri bu sınırların altında kalacak şekilde aralıklar
  (bölge çağrıları 1 saniye arayla). Alan adı bilgisi ve TLD listesi hız sınırına tabi değildir.
- **Yumuşak kota.** Aramalar ve bilgi sorguları 1 puan, diğer her şey 10 puan harcar. 200 puanın üzerinde geçit her
  isteği 5 saniyeye kadar yavaşlatır; puanlar dakikada %20 azalır. `SendWithResponseAsync`, durumu `Quota` içinde gösterir.

<a id="dependency-injection"></a>

## Dependency injection

`DomainNameApiClient`, tipli bir `HttpClient` olarak çalışır:

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

SDK, kendi `HttpClient`'ında yönlendirmeleri izlemez. Factory ile üretilen bir istemci yönlendirmeleri izleyebilir;
SDK yine de geçidin askıya alma sayfasını tanır ve `INVALID_RESELLER` bildirir.

<a id="testing-your-code"></a>

## Kodunuzu test etme

SDK'yı kullanan kodu API'yi çağırmadan test etmek için, stub handler'lı bir `HttpClient` verin:

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

Gerçek API'ye karşı testler için, kendi anahtarıyla OTE'yi kullanın.

<a id="project-layout"></a>

## Proje yapısı

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

Her alan klasörü aynı biçimdedir: `XxxClient.cs`, işlem başına bir sınıf içeren bir `Requests/` klasörü ve yanıtları
ile enum'ları içeren bir `Models/` klasörü. Bir alanın tüm tipleri tek bir namespace'i paylaşır, örneğin `DomainNameApi.Domains`.

<a id="development"></a>

## Geliştirme

```bash
dotnet build
dotnet test
```

Testler gerçek API'yi asla çağırmaz: sahte bir `HttpMessageHandler` kullanır. Windows'ta .NET 10 üzerinde ve
.NET Framework 4.8 üzerinde çalışırlar; ikincisi .NET Standard 2.0 derlemesini sınar.

`tests/DomainNameApi.Tests/testdata/openapi.json`, `dna` CLI'nin içine gömdüğü API dokümanıdır. `SpecCoverageTests`,
içindeki her işlemin, CLI'nin güvenlik sınıfıyla birlikte tam olarak bir istek sınıfına sahip olduğunu denetler. API'ye
yeni bir işlem eklendiğinde dokümanı güncelleyin; istek yazılana kadar test başarısız olur.

<a id="upgrading-from-1x"></a>

## 1.x'ten yükseltme

2.0 yeni bir SDK'dır. 1.x, PHP kütüphanesinin bir uyarlamasıydı ve tipsiz `JsonNode` zarfları döndürüyordu; 2.0'da tipli
istekler ve modeller, async metotlar ve istisnalar vardır.

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

1.x, hata raporlarını Sentry'ye gönderiyordu. 2.0, API dışında hiçbir yere hiçbir şey göndermez.

## Bağlantılar

- [`dna` CLI ve MCP sunucusu](https://github.com/domainreseller/cli): aynı API, terminalden ve yapay zeka ajanlarından.
- [API dokümantasyonu](https://www.domainnameapi.com/docs/)
- [Değişiklik günlüğü](CHANGELOG.md)

## Lisans

Apache License 2.0. Bkz. [LICENSE](LICENSE).
