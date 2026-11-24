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

<div dir="rtl">

# حزمة DomainNameAPI لـ .NET (dotnet-dna)

</div>

<p>
  <a href="https://github.com/domainreseller/dotnet-dna/actions/workflows/ci.yml"><img src="https://img.shields.io/github/actions/workflow/status/domainreseller/dotnet-dna/ci.yml?branch=main&label=CI&style=flat-square" alt="CI"></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/license-Apache--2.0-0B8DB5?style=flat-square" alt="Apache-2.0"></a>
</p>

<div dir="rtl">

حزمة SDK الرسمية لـ .NET الخاصة بمنصة الموزّعين [Domain Name API](https://www.domainnameapi.com). تغطي جميع عمليات REST API
البالغ عددها 67 عملية بعملاء ونماذج مكتوبة بأنواع صريحة: النطاقات، وجهات الاتصال، وDNS، وDNSSEC، وشهادات SSL،
والاستضافة، والطلبات، والمنتجات، وحسابك.

</div>

```csharp
using var client = new DomainNameApiClient(resellerId, apiKey, ApiEnvironment.Ote);

var search = await client.Domains.SearchAsync("example.com");
if (search.Info?.IsAvailable == true)
{
    await client.Domains.RegisterAsync("example.com", contact, ["tr.apiname.com", "eu.apiname.com"]);
}
```

<div dir="rtl">

- **كل شيء مكتوب بأنواع صريحة.** كل عملية هي فئة طلب، وكل إجابة هي نموذج مزوّد بتوثيق IntelliSense. لا قواميس ولا تحليل JSON.
- **طلباتك الخاصة.** كل طلب يشتق من `BaseRequest`. ورّث منه لنقطة نهاية لا تغطيها الحزمة بعد.
- **آمنة مع الأموال.** يعلن كل طلب ما إذا كان يقرأ، أو يكتب، أو يخصم من رصيدك، أو يحذف شيئًا. لا يُعاد إرسال الطلبات المدفوعة أبدًا، ووضع القراءة فقط يمنع أي تغيير.
- **مرونة مدمجة.** تُراعى حدود معدل الطلبات تلقائيًا، وتُعاد محاولة إجابات 429، وتُعاد محاولة أخطاء الشبكة في عمليات القراءة.
- **أخطاء واضحة.** نوع استثناء واحد مع تصنيف ثابت، ورمز الخطأ من API، وأخطاء التحقق على مستوى الحقول، ومعرّف الارتباط (correlation ID) وتلميح.
- **.NET حديث.** عمليات غير متزامنة مع الإلغاء، وترقيم صفحات تلقائي عبر `IAsyncEnumerable`، ودعم `IHttpClientFactory`، وتعليقات nullable التوضيحية.
- **تعمل في كل مكان.** .NET 10، وأي بيئة تشغيل تدعم .NET Standard 2.0 (‏.NET Framework 4.6.2 فما فوق، و.NET 8، وMono، وUnity).

وهي النظير في .NET لـ [`dna`، أداة CLI الخاصة بـ Domain Name API](https://github.com/domainreseller/cli): وثيقة API نفسها،
وفئات الأمان نفسها، وقواعد إعادة المحاولة ورموز الأخطاء نفسها.

## المحتويات

- [التثبيت](#install)
- [بيانات الاعتماد](#credentials)
- [البدء السريع](#quick-start)
- [الإعداد](#configuration)
- [الاستخدام](#usage): [الحساب](#account) · [النطاقات](#domains) · [جهات الاتصال](#contacts) · [DNS](#dns) · [DNSSEC](#dnssec) · [SSL](#ssl) · [الاستضافة](#hosting) · [الطلبات والمنتجات](#orders-and-products)
- [الطلبات و`BaseRequest`](#requests-and-baserequest)
- [الأخطاء](#errors)
- [الأمان وإعادة المحاولة وحدود معدل الطلبات](#safety-retries-and-rate-limits)
- [حقن التبعيات](#dependency-injection)
- [اختبار شيفرتك](#testing-your-code)
- [بنية المشروع](#project-layout)
- [التطوير](#development)
- [الترقية من 1.x](#upgrading-from-1x)

<a id="install"></a>

## التثبيت

> [!IMPORTANT]
> **استخدم دائمًا [أحدث إصدار](https://github.com/domainreseller/dotnet-dna/releases/latest).** حزم الإصدارات مرقّمة بإصدارات ومُختبرة.
> أما زر **Code → Download ZIP** الأخضر فينزّل فرع التطوير بدلًا منها.

</div>

```bash
dotnet add package DomainNameApi
```

<div dir="rtl">

لتثبيت ملف `.nupkg` نزّلته، ضعه في مجلد واستخدم هذا المجلد كمصدر للحزم:

</div>

```bash
dotnet add package DomainNameApi --source ./packages
```

<div dir="rtl">

أو أضف مرجعًا إلى المشروع من المصدر البرمجي لأحد الإصدارات:

</div>

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

<div dir="rtl">

**المتطلبات:** .NET 10 أو أحدث، أو أي بيئة تشغيل تدعم .NET Standard 2.0. لا توجد تبعيات من أطراف ثالثة.
على .NET Standard 2.0 تضيف الحزمة `System.Text.Json` و`Microsoft.Bcl.AsyncInterfaces` من Microsoft، وهما مدمجتان في .NET.

<a id="credentials"></a>

## بيانات الاعتماد

تصادق API على كل طلب بقيمتين، وترسلهما الحزمة نيابةً عنك:

| القيمة | الترويسة (Header) | أين تجدها |
|---|---|---|
| Reseller ID (معرّف UUID) | `__reseller` | لوحة الموزّع ← **API settings** |
| API key | `X-API-KEY` | لوحة الموزّع ← **API settings** |

> [!NOTE]
> هذه **بيانات اعتماد API**: بيانات تسجيل الدخول إلى اللوحة لا تعمل هنا. **تستخدم بيئة الإنتاج وبيئة OTE (بيئة الاختبار)
> مفاتيح API مختلفة.** استخدام مفتاح إحداهما مع الأخرى يفشل بالخطأ `INVALID_API_KEY` أو `INVALID_RESELLER`.

<a id="quick-start"></a>

## البدء السريع

</div>

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

<div dir="rtl">

يشغّل [مشروع الأمثلة](examples/DomainNameApi.Examples) سيناريوهات مثل هذه على OTE:

</div>

```bash
dotnet run --project examples/DomainNameApi.Examples -- search example.com
```

<div dir="rtl">

<a id="configuration"></a>

## الإعداد

هناك أربع طرق لإنشاء عميل:

</div>

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

<div dir="rtl">

| الخيار | الافتراضي | المعنى |
|---|---|---|
| `ResellerId`, `ApiKey` | (مطلوب) | بيانات الاعتماد. |
| `Environment` | `Production` | `Production` (‏`https://api.domainresellerapi.com`) أو `Ote` (‏`https://ote.domainresellerapi.com`). |
| `BaseAddress` | لا شيء | يتجاوز عنوان البيئة، مثلًا عند استخدام وكيل (proxy). |
| `Timeout` | 60 ثانية | المهلة الزمنية لمحاولة واحدة. |
| `MaxRetries` | 2 | عدد مرات تكرار الطلب الفاشل (انظر [إعادة المحاولة](#safety-retries-and-rate-limits)). القيمة 0 توقف إعادة المحاولة. |
| `MaxRetryDelay` | 30 ثانية | أطول انتظار قبل إعادة المحاولة. |
| `PaceRequests` | `true` | يوزّع الطلبات زمنيًا لتبقى ضمن حدود معدل الطلبات في البوابة (gateway). |
| `ReadOnly` | `false` | يمنع كل طلب ليس قراءةً، قبل إرساله. |
| `UserAgent` | `DomainNameAPI-DotNet/<version>` | ترويسة `User-Agent`. |
| `AppName` | `dotnet` | التطبيق الذي يستخدم الـ SDK، ويُرسل في ترويسة `X-App` (انظر أدناه). |
| `AppVersion` | إصدار الـ SDK | إصدار هذا التطبيق، ويُرسل في ترويسة `X-Version`. |

تقرأ `FromEnvironment()` ما يلي:

| المتغير | المعنى |
|---|---|
| `DNA_RESELLER_ID`, `DNA_API_KEY` | بيانات الاعتماد. |
| `DNA_ENV` | `production` أو `ote` (وأيضًا `prod` و`live` و`test` و`sandbox`). |
| `DNA_TEST_MODE` | القيم `true` أو `1` أو `on` أو `yes` تختار OTE. تُستخدم عند عدم ضبط `DNA_ENV`. |
| `DNA_BASE_URL` | تجاوز اختياري للعنوان الأساسي. |
| `DNA_READ_ONLY` | القيمة `true` تفعّل وضع القراءة فقط. |
| `DNA_APP`, `DNA_APP_VERSION` | `AppName` و`AppVersion`. |

يحمل كل طلب أيضًا الترويستين `X-App` و`X-Version`، اللتين تُعلمان DomainNameAPI بالتكامل الذي أرسل الطلب. القيمتان الافتراضيتان هما `dotnet` وإصدار الـ SDK. إذا كنت تبني وحدة للوحة تحكم أو تكاملًا آخر فوق الـ SDK، فأرسل قيمك الخاصة:

</div>

```csharp
var client = new DomainNameApiClient(new DomainNameApiOptions
{
    ResellerId = resellerId,
    ApiKey = apiKey,
    AppName = "WiseCP",      // X-App: wisecp
    AppVersion = "V2.0.20",  // X-Version: 2.0.20
});
```

<div dir="rtl">

تُحوَّل `AppName` إلى أحرف صغيرة ولا يُبقى فيها إلا `a-z` و`0-9` و`.` و`_` و`-`؛ ولا يُبقى في `AppVersion` إلا الأرقام والنقاط. الاسم الفارغ أو غير الصالح يعود إلى `dotnet`، والإصدار الفارغ يعود إلى إصدار الـ SDK. تعرض `client.AppName` و`client.AppVersion` القيم المُرسلة.

العميل آمن للاستخدام من عدة خيوط (thread-safe). أنشئ عميلًا واحدًا لكل مجموعة بيانات اعتماد، وأعد استخدامه طوال عمر التطبيق.

<a id="usage"></a>

## الاستخدام

لكل مجال في API عميله الخاص. تأخذ الدوال وسائط بسيطة؛ ولخيارات أكثر، لكل دالة أيضًا
نسخة محمّلة (overload) أو دالة شقيقة تأخذ كائن الطلب.

<a id="account"></a>

### الحساب

</div>

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

<div dir="rtl">

<a id="domains"></a>

### النطاقات

**البحث**

</div>

```csharp
var one = await client.Domains.SearchAsync("example.com", currency: "USD");
bool available = one.Info?.IsAvailable == true;

var many = await client.Domains.BulkSearchAsync(["example.com", "example.net", "example.com.tr"]);
foreach (var domain in many.Available)
{
    Console.WriteLine($"{domain.DomainName}: {domain.Price} {domain.Currency}");
}
```

<div dir="rtl">

**القوائم والتفاصيل**

</div>

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

<div dir="rtl">

**التسجيل والتجديد والنقل والاستعادة** (تخصم من رصيدك)

</div>

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

<div dir="rtl">

تُفحص الطلبات قبل إرسالها. التسجيل دون أدوار جهات الاتصال الأربعة جميعها، أو جهة اتصال بلا عنوان بريد إلكتروني،
أو مدة 0 سنة، يُطلق `ArgumentException`، ولا يصل شيء إلى API.

**الإعدادات**

</div>

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

<div dir="rtl">

**إعادة توجيه URL وخوادم الأسماء الفرعية والمستندات**

</div>

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

<div dir="rtl">

<a id="contacts"></a>

### جهات الاتصال

معرّفات جهات الاتصال (handles) هي جهات اتصال قابلة لإعادة الاستخدام. أنشئ واحدة مرة واحدة واستخدمها لنطاقات كثيرة.

</div>

```csharp
var created = await client.Contacts.CreateAsync(contact);
string handle = created.Handle!;                          // for example DNA-12345

var details = await client.Contacts.GetAsync(handle);     // the contact and its verification state
var result = await client.Contacts.UpdateAsync(handle, contact);
Console.WriteLine($"{result.Stats?.SuccessCount} of {result.Stats?.TotalDomainCount} domains updated");

var verifications = await client.Contacts.GetVerificationAsync(handle);
await client.Contacts.ResendVerificationAsync(handle);
```

<div dir="rtl">

<a id="dns"></a>

### DNS

سجلات المنطقة (zone) للنطاقات الموجودة على خوادم أسماء المنصة. تُحَلّ السجلات علنًا بمجرد أن يستخدم النطاق تلك الخوادم،
وهذا ما تتولاه `ApplyAsync`.

</div>

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

<div dir="rtl">

تفحص الحزمة ما سترفضه API: سجل CNAME عند الجذر (apex)، أو قيمة CAA ناقصة الأجزاء الثلاثة، أو TTL خارج النطاق
1–86400. تُباعَد استدعاءات المنطقة بثانية واحدة، وهي حدّ البوابة.

<a id="dnssec"></a>

### DNSSEC

</div>

```csharp
var info = await client.Dnssec.GetAsync("example.com");   // Status, DsRecords, AllowedAlgorithms, ...

var record = DsRecordData.Parse("12345 13 2 49FD46E6C4B45C55D4AC69CBD3CD34AC1AFE51DE");
await client.Dnssec.AddAsync("example.com", record);
await client.Dnssec.RemoveAsync("example.com", record);
await client.Dnssec.DisableAsync("example.com");          // removes every DS record
```

<div dir="rtl">

<a id="ssl"></a>

### SSL

يمكن أن يكون للنطاق الواحد عدة شهادات، ولذلك تُحدَّد الشهادة القائمة عبر **معرّف الشهادة** (من النوع `Guid`)
لا عبر اسم النطاق. تعيد `PurchaseAsync` و`RenewAsync` هذا المعرّف: احتفظ به. وتعثر `ListAsync` على معرّفات الشهادات
التي تملكها بالفعل.

</div>

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

<div dir="rtl">

**عرض الشهادات والعثور على المعرّف**

</div>

```csharp
var page = await client.Ssl.ListAsync(new ListCertificatesRequest { DomainName = "example.com" });   // newest first

await foreach (var certificate in client.Ssl.ListAllAsync())
{
    Console.WriteLine($"{certificate.CertificateId} {certificate.DomainName} {certificate.Status} {certificate.SubscriptionExpiresAt:d}");
}

// The approver addresses of a certificate you already have
var approverEmails = await client.Ssl.GetApproverEmailsAsync(certificateId);
```

<div dir="rtl">

**التجديد** (يخصم من رصيدك)

</div>

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

<div dir="rtl">

يجب إعداد الشهادة المشتراة أو المجدَّدة خلال 15 يومًا، وإلا أُلغيت. للتجديد استدعِ `RenewAsync` ثم `ConfigureAsync`
بالمعرّف الجديد. لا تشترِ مجددًا عبر `PurchaseAsync`: فذلك شراء مستقل بسعر الشراء، ولن توسَم الشهادة القديمة بأنها
مجدَّدة أبدًا. معرّف الشهادة الفارغ (`Guid.Empty`) يُطلق `ArgumentException` قبل إرسال أي شيء.

<a id="hosting"></a>

### الاستضافة

</div>

```csharp
var packages = await client.Hosting.ListPackagesAsync();
var created = await client.Hosting.CreateAsync("example.com", "starter-linux");   // charges the balance
var status = await client.Hosting.GetStatusAsync("example.com");                   // panel access once ready
var hosting = await client.Hosting.GetAsync("example.com");
var plans = await client.Hosting.ListPlansAsync();
```

<div dir="rtl">

<a id="orders-and-products"></a>

### الطلبات والمنتجات

</div>

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

<div dir="rtl">

<a id="requests-and-baserequest"></a>

## الطلبات و`BaseRequest`

كل عملية هي فئة طلب، مثل `RenewDomainRequest` أو `ListDomainsRequest` أو `AddDnsRecordRequest`.
تبنيها لك دوال العميل المذكورة أعلاه، ويمكنك أيضًا إنشاؤها وإرسالها بنفسك:

</div>

```csharp
var request = new RenewDomainRequest("example.com", period: 2);
Console.WriteLine($"{request} is {request.Safety}");   // "POST domains/renew is Billable"

DomainOperationResult result = await client.SendAsync(request);

// The same, with the HTTP details: correlation ID, soft-quota state, attempts, duration
var response = await client.SendWithResponseAsync(request);
Console.WriteLine($"{response.CorrelationId} quota {response.Quota?.Used}/{response.Quota?.Limit}");
```

<div dir="rtl">

تشتق جميع الطلبات من إحدى ثلاث فئات أساسية:

| الفئة الأساسية | الغرض |
|---|---|
| `BaseRequest` | العمليات التي لا تُعيد شيئًا (قفل، فك القفل، إضافة سجل DNS، ...). |
| `BaseRequest<TResponse>` | العمليات التي تُعيد بيانات. |
| `PagedRequest<TItem>` | عمليات القوائم. تضيف `SkipCount` و`MaxResultCount` و`Sorting`، وتعمل مع `ListAllAsync`. |

يصف الطلبُ نفسه عبر بضع دوال مُعاد تعريفها (overrides). في POST وPUT يكون كائن الطلب نفسه هو جسم JSON (بصيغة camelCase،
مع حذف القيم null). تُضاف معاملات الاستعلام في `AddQueryParameters`، وتوضع الفحوص التي تسبق الإرسال في `Validate`.
لاستدعاء نقطة نهاية لا تغطيها الحزمة بعد، اكتب طلبك الخاص:

</div>

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

<div dir="rtl">

يحصل طلبك على كل ما تحصل عليه الطلبات المدمجة: بيانات الاعتماد، وإعادة المحاولة بحسب فئة الأمان، وتنظيم وتيرة الطلبات، ووضع القراءة فقط،
ومعالجة الأخطاء. أعد تعريف `GetBody()` عندما تتوقع API شكلًا مختلفًا للجسم، وأعد تعريف `IsIdempotent` عندما
يكون تكرار عملية الكتابة آمنًا.

<a id="errors"></a>

## الأخطاء

كل طلب فاشل يُطلق `DomainNameApiException`. أما الوسائط غير الصالحة فتُطلق `ArgumentException` قبل إرسال أي شيء.

</div>

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

<div dir="rtl">

| الخاصية | المعنى |
|---|---|
| `Kind` | تصنيف ثابت (الجدول أدناه). اتخذ قراراتك بناءً عليه، لا على الرسالة. |
| `Code` | رمز الخطأ من API (مثل `Dna.DomainService:Domain:10007`)، أو أحد رموز الحزمة أدناه. |
| `Message` | ما الذي حدث. تُستبدل الرسائل الجاهزة العامة بتفاصيل API. |
| `StatusCode` | حالة HTTP، أو null إن لم تصل أي إجابة. |
| `Details`, `ValidationErrors` | تفاصيل إضافية ومشكلات على مستوى الحقول من API. |
| `CorrelationId` | قيمة `X-Correlation-Id` من البوابة. اذكرها عند التواصل مع الدعم. |
| `Hint` | اقتراح قصير بما يجب فعله بعد ذلك. |
| `IsRetryable` | هل يمكن أن ينجح تكرار الطلب نفسه لاحقًا. |
| `OutcomeUnknown` | فشل طلب يُغيّر شيئًا بسبب الشبكة، فقد يكون قد نُفّذ. |
| `Operation`, `Safety` | الطلب الفاشل (`POST domains/renew`) وفئة الأمان الخاصة به. |
| `RetryAfter`, `ResponseBody` | قيمة `Retry-After` من البوابة والإجابة الخام. |

| `Kind` | متى |
|---|---|
| `Authentication` | بيانات اعتماد خاطئة أو مفقودة، أو موزّع غير معروف أو موقوف في هذه البيئة (401، إعادة توجيه). |
| `Forbidden` | مصادَق عليه لكن غير مسموح له (403). |
| `NotFound` | النطاق أو جهة الاتصال أو الطلب أو أي مورد آخر غير موجود (404). |
| `Validation` | رفضت API المدخلات أو فشلت قاعدة عمل: حقل غير صالح، أو نطاق مقفل، أو رصيد غير كافٍ (400، 422). |
| `Conflict` | حالة المورد لا تسمح بالعملية (409). |
| `RateLimited` | ما زال الطلب محدودًا بمعدل الطلبات بعد إعادة المحاولة التلقائية (429). |
| `Server` | فشل من جهة الخادم (5xx). غالبًا مؤقت. |
| `Network`, `Timeout` | لا إجابة: DNS أو TLS أو وكيل (proxy) أو إعادة ضبط الاتصال، أو لا إجابة خلال `Timeout`. |
| `ReadOnly` | مُنع بسبب وضع القراءة فقط؛ ولم يُرسَل شيء. |
| `Unknown` | أي حالة أخرى، بما فيها إجابة لم تستطع الحزمة قراءتها. |

الرموز التي تضبطها الحزمة بنفسها:

| الرمز | المعنى |
|---|---|
| `INVALID_RESELLER` | أعادت البوابة التوجيه إلى صفحة الإيقاف: معرّف الموزّع غير معروف أو غير نشط أو موقوف **في هذه البيئة**. |
| `INVALID_API_KEY` | المفتاح لا يطابق الموزّع. مفاتيح الإنتاج وOTE مختلفة. |
| `MISSING_API_KEY`, `MISSING_RESELLER_ID`, `NO_CREDENTIALS` | إحدى بيانات الاعتماد فارغة. |
| `SUBRESELLER_NOT_ALLOWED` | لا يمكن لحسابات الموزّعين الفرعيين استخدام API. استخدم بيانات اعتماد الموزّع الرئيسي. |
| `UNAUTHORIZED` | أي 401 آخر. |
| `RATE_LIMITED` | 429 بعد إعادة المحاولة. |
| `NETWORK`, `TIMEOUT` | لم تصل أي إجابة. |
| `READ_ONLY` | مُنع بسبب وضع القراءة فقط. |
| `EMPTY_RESPONSE`, `INVALID_RESPONSE` | كانت الإجابة الناجحة فارغة أو تعذّرت قراءتها. |
| `HTTP_<status>` | لم ترسل API رمز خطأ. |

<a id="safety-retries-and-rate-limits"></a>

## الأمان وإعادة المحاولة وحدود معدل الطلبات

لكل طلب فئة أمان، وهي نفسها التي تستخدمها أداة `dna` CLI:

| `OperationSafety` | تعني | أمثلة |
|---|---|---|
| `Read` | بلا آثار جانبية، حتى لو استخدمت API الأسلوب POST أو PUT | البحث، التفاصيل، القوائم، فحوص التجديد والنقل، إنشاء CSR |
| `Write` | تغيير مجاني يمكن التراجع عنه | سجلات DNS، القفل، الخصوصية، خوادم الأسماء، جهات الاتصال |
| `Billable` | يخصم من رصيدك | التسجيل، التجديد، النقل، الاستعادة، شراء SSL وتجديده، طلب استضافة |
| `Destructive` | يحذف شيئًا أو يصعب التراجع عنه | حذف سجل DNS، إزالة DNSSEC، إلغاء شهادة |

- **وضع القراءة فقط** (‏`ReadOnly = true` أو `DNA_READ_ONLY=true`) يمنع كل ما ليس `Read` قبل إرساله.
  استخدمه أثناء الاستكشاف، وفي التقارير ولوحات المعلومات.
- **إعادة المحاولة.** تُعاد محاولة إجابات 429 دائمًا، مع احترام `Retry-After`. وهذا آمن حتى للطلبات المدفوعة، لأن
  البوابة ترفضها قبل تنفيذ أي شيء. أما أخطاء الشبكة والمهلات وإجابات 502/503/504 فتُعاد محاولتها لعمليات القراءة فقط
  (ولعمليات الكتابة التي يكون تكرارها آمنًا، مثل استبدال خوادم الأسماء). لا يُرسَل التسجيل أو التجديد مرتين أبدًا.
- **تنظيم وتيرة الطلبات.** تسمح البوابة بـ 5 طلبات في الثانية لكل مفتاح، و2 في الثانية لعمليات البحث عن النطاقات، و20 في الثانية
  لاستدعاءات منطقة DNS. يباعد العميل بين الطلبات ليبقى ضمن هذه الحدود (وبين استدعاءات المنطقة بثانية واحدة). معلومات النطاق وقائمة
  TLD غير محدودتين بمعدل الطلبات.
- **الحصة المرنة (soft quota).** تكلّف عمليات البحث والاستعلام عن المعلومات نقطة واحدة، وكل ما عداها 10 نقاط. بعد تجاوز 200 نقطة تبطئ البوابة كل
  طلب بما يصل إلى 5 ثوانٍ؛ وتتناقص النقاط بنسبة 20% في الدقيقة. تعرض `SendWithResponseAsync` الحالة في `Quota`.

<a id="dependency-injection"></a>

## حقن التبعيات

يعمل `DomainNameApiClient` كـ `HttpClient` مكتوب بنوع صريح (typed):

</div>

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

<div dir="rtl">

لا تتبع الحزمة أي عمليات إعادة توجيه على `HttpClient` الخاص بها. أما العميل الذي ينشئه المصنع (factory) فقد يتبعها؛ وتظل الحزمة تتعرف
على صفحة الإيقاف في البوابة وتُبلغ بالخطأ `INVALID_RESELLER`.

<a id="testing-your-code"></a>

## اختبار شيفرتك

مرّر `HttpClient` مع معالج وهمي (stub handler) لاختبار الشيفرة التي تستخدم الحزمة دون استدعاء API:

</div>

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

<div dir="rtl">

للاختبار على API الحقيقية، استخدم OTE بمفتاحها الخاص.

<a id="project-layout"></a>

## بنية المشروع

</div>

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

<div dir="rtl">

لكل مجلد مجال الشكل نفسه: `XxxClient.cs`، ومجلد `Requests/` فيه فئة واحدة لكل عملية، ومجلد `Models/`
فيه الإجابات والقيم المُعدَّدة (enums). تشترك جميع أنواع المجال في مساحة أسماء (namespace) واحدة، مثل `DomainNameApi.Domains`.

<a id="development"></a>

## التطوير

</div>

```bash
dotnet build
dotnet test
```

<div dir="rtl">

لا تستدعي الاختبارات API الحقيقية أبدًا: فهي تستخدم `HttpMessageHandler` وهميًا. على Windows تعمل على .NET 10 وعلى
.NET Framework 4.8؛ والأخير يختبر إصدار .NET Standard 2.0.

الملف `tests/DomainNameApi.Tests/testdata/openapi.json` هو وثيقة API التي تضمّنها أداة `dna` CLI. يتحقق `SpecCoverageTests`
من أن لكل عملية فيه فئة طلب واحدة بالضبط بفئة الأمان نفسها في CLI. عندما تضيف API عملية جديدة،
حدّث الوثيقة، وسيفشل الاختبار إلى أن يوجد الطلب.

<a id="upgrading-from-1x"></a>

## الترقية من 1.x

الإصدار 2.0 حزمة SDK جديدة. كان الإصدار 1.x نسخة منقولة من مكتبة PHP ويُعيد أغلفة `JsonNode` غير مكتوبة بأنواع؛ أما 2.0 فيعتمد
طلبات ونماذج مكتوبة بأنواع صريحة، ودوالَّ غير متزامنة، واستثناءات.

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

كان الإصدار 1.x يرسل تقارير الأخطاء إلى Sentry. أما 2.0 فلا يرسل شيئًا إلى أي جهة سوى API.

## الروابط

- [`dna` CLI وخادم MCP](https://github.com/domainreseller/cli): API نفسها من الطرفية ومن وكلاء الذكاء الاصطناعي.
- [توثيق API](https://www.domainnameapi.com/docs/)
- [سجل التغييرات](CHANGELOG.md)

## الترخيص

Apache License 2.0. راجع [LICENSE](LICENSE).

</div>
