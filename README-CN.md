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

[Domain Name API](https://www.domainnameapi.com) 经销商平台的官方 .NET SDK。它用带类型的客户端和模型覆盖了 REST API 的全部
64 个操作：域名、联系人、DNS、DNSSEC、SSL 证书、主机、订单、产品和你的账户。

```csharp
using var client = new DomainNameApiClient(resellerId, apiKey, ApiEnvironment.Ote);

var search = await client.Domains.SearchAsync("example.com");
if (search.Info?.IsAvailable == true)
{
    await client.Domains.RegisterAsync("example.com", contact, ["tr.apiname.com", "eu.apiname.com"]);
}
```

- **处处有类型。** 每个操作都是一个请求类，每个响应都是一个带 IntelliSense 文档的模型。没有字典，也不用解析 JSON。
- **自定义请求。** 每个请求都派生自 `BaseRequest`。SDK 尚未覆盖的端点，可以通过子类化来调用。
- **涉及资金时更安全。** 每个请求都会声明自己是读取、写入、扣除余额还是删除数据。需付费的请求绝不会重试，只读模式会阻止一切更改。
- **内置容错。** 自动遵守速率限制，429 响应会重试，读取请求遇到网络错误也会重试。
- **清晰的错误。** 只有一种异常类型，带有稳定的错误类别、API 的错误代码、字段级验证错误、关联 ID 和提示。
- **现代 .NET。** 支持取消的异步方法、基于 `IAsyncEnumerable` 的自动分页、`IHttpClientFactory` 支持、可空注解。
- **随处运行。** 支持 .NET 10，以及任何支持 .NET Standard 2.0 的运行时（.NET Framework 4.6.2+、.NET 8、Mono、Unity）。

它是 [`dna`（Domain Name API CLI）](https://github.com/domainreseller/cli) 的 .NET 对应版本：使用同一份 API
文档、同样的安全类别、重试规则和错误代码。

## 目录

- [安装](#install)
- [凭据](#credentials)
- [快速开始](#quick-start)
- [配置](#configuration)
- [用法](#usage)：[账户](#account) · [域名](#domains) · [联系人](#contacts) · [DNS](#dns) · [DNSSEC](#dnssec) · [SSL](#ssl) · [主机](#hosting) · [订单与产品](#orders-and-products)
- [请求与 `BaseRequest`](#requests-and-baserequest)
- [错误](#errors)
- [安全、重试与速率限制](#safety-retries-and-rate-limits)
- [依赖注入](#dependency-injection)
- [测试你的代码](#testing-your-code)
- [项目结构](#project-layout)
- [开发](#development)
- [从 1.x 升级](#upgrading-from-1x)

<a id="install"></a>

## 安装

> [!IMPORTANT]
> **请始终使用[最新版本](https://github.com/domainreseller/dotnet-dna/releases/latest)。** 发布包均带有版本号并经过测试。
> 绿色的 **Code → Download ZIP** 按钮下载的是开发分支。

```bash
dotnet add package DomainNameApi
```

要安装已下载的 `.nupkg`，请将其放入一个文件夹，并把该文件夹用作包源：

```bash
dotnet add package DomainNameApi --source ./packages
```

或者引用某个发布版源码中的项目：

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

**要求：** .NET 10 或更高版本，或任何支持 .NET Standard 2.0 的运行时。没有第三方依赖。在 .NET Standard 2.0 上，
该包会引入 Microsoft 的 `System.Text.Json` 和 `Microsoft.Bcl.AsyncInterfaces`，它们已内置于 .NET 中。

<a id="credentials"></a>

## 凭据

API 用两个值验证每个请求，SDK 会替你发送它们：

| 值 | 请求头 | 在哪里找到 |
|---|---|---|
| Reseller ID（一个 UUID） | `__reseller` | 经销商面板 → **API 设置** |
| API Key | `X-API-KEY` | 经销商面板 → **API 设置** |

> [!NOTE]
> 这些是 **API 凭据**：你的面板登录信息在这里无法使用。**正式环境和 OTE（测试环境）使用
> 不同的 API Key。** 混用会失败，并返回 `INVALID_API_KEY` 或 `INVALID_RESELLER`。

<a id="quick-start"></a>

## 快速开始

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

[示例项目](examples/DomainNameApi.Examples)会针对 OTE 运行类似的场景：

```bash
dotnet run --project examples/DomainNameApi.Examples -- search example.com
```

<a id="configuration"></a>

## 配置

创建客户端有四种方式：

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

| 选项 | 默认值 | 含义 |
|---|---|---|
| `ResellerId`、`ApiKey` | （必填） | 凭据。 |
| `Environment` | `Production` | `Production`（`https://api.domainresellerapi.com`）或 `Ote`（`https://ote.domainresellerapi.com`）。 |
| `BaseAddress` | 无 | 覆盖环境的地址，例如用于代理。 |
| `Timeout` | 60 秒 | 单次尝试的时间限制。 |
| `MaxRetries` | 2 | 失败的请求重复多少次（见[重试](#safety-retries-and-rate-limits)）。0 表示关闭重试。 |
| `MaxRetryDelay` | 30 秒 | 重试前的最长等待时间。 |
| `PaceRequests` | `true` | 控制请求间隔，使其保持在网关的速率限制之内。 |
| `ReadOnly` | `false` | 在发送之前阻止所有非读取请求。 |
| `UserAgent` | `DomainNameAPI-DotNet/<version>` | `User-Agent` 请求头。 |

`FromEnvironment()` 会读取：

| 变量 | 含义 |
|---|---|
| `DNA_RESELLER_ID`、`DNA_API_KEY` | 凭据。 |
| `DNA_ENV` | `production` 或 `ote`（也可用 `prod`、`live`、`test`、`sandbox`）。 |
| `DNA_TEST_MODE` | `true`、`1`、`on` 或 `yes` 表示选择 OTE。未设置 `DNA_ENV` 时使用。 |
| `DNA_BASE_URL` | 可选，覆盖基础地址。 |
| `DNA_READ_ONLY` | `true` 表示开启只读模式。 |

客户端是线程安全的。每组凭据创建一个客户端，并在应用的整个生命周期内复用。

<a id="usage"></a>

## 用法

API 的每个领域都有各自的客户端。方法接受简单的参数；如需更多选项，每个方法还有一个接受请求对象的
重载（或同类方法）。

<a id="account"></a>

### 账户

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

### 域名

**搜索**

```csharp
var one = await client.Domains.SearchAsync("example.com", currency: "USD");
bool available = one.Info?.IsAvailable == true;

var many = await client.Domains.BulkSearchAsync(["example.com", "example.net", "example.com.tr"]);
foreach (var domain in many.Available)
{
    Console.WriteLine($"{domain.DomainName}: {domain.Price} {domain.Currency}");
}
```

**列表与详情**

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

**注册、续费、转移、恢复**（这些操作会扣除你的余额）

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

请求在发送之前会先经过检查。缺少四种联系人角色之一的注册、没有电子邮件地址的联系人，或注册期限为 0 年，
都会抛出 `ArgumentException`，并且不会向 API 发送任何内容。

**设置**

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

**URL 转发、子名称服务器与文档**

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

### 联系人

联系人句柄（handle）是可重复使用的联系人。创建一次，即可用于多个域名。

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

使用平台名称服务器的域名的区域记录。只有当域名使用这些名称服务器后，记录才会在公网上解析，
这一步由 `ApplyAsync` 负责。

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

SDK 会提前检查 API 会拒绝的情况：根域上的 CNAME、缺少三个部分的 CAA 值、超出
1–86400 范围的 TTL。区域调用之间会保持 1 秒间隔，这是网关的限制。

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

### 主机

```csharp
var packages = await client.Hosting.ListPackagesAsync();
var created = await client.Hosting.CreateAsync("example.com", "starter-linux");   // charges the balance
var status = await client.Hosting.GetStatusAsync("example.com");                   // panel access once ready
var hosting = await client.Hosting.GetAsync("example.com");
var plans = await client.Hosting.ListPlansAsync();
```

<a id="orders-and-products"></a>

### 订单与产品

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

## 请求与 `BaseRequest`

每个操作都是一个请求类，例如 `RenewDomainRequest`、`ListDomainsRequest` 或 `AddDnsRecordRequest`。
上面的客户端方法会替你构建它们，你也可以自己创建并发送：

```csharp
var request = new RenewDomainRequest("example.com", period: 2);
Console.WriteLine($"{request} is {request.Safety}");   // "POST domains/renew is Billable"

DomainOperationResult result = await client.SendAsync(request);

// The same, with the HTTP details: correlation ID, soft-quota state, attempts, duration
var response = await client.SendWithResponseAsync(request);
Console.WriteLine($"{response.CorrelationId} quota {response.Quota?.Used}/{response.Quota?.Limit}");
```

所有请求都派生自三个基类之一：

| 基类 | 用途 |
|---|---|
| `BaseRequest` | 不返回数据的操作（锁定、解锁、添加 DNS 记录等）。 |
| `BaseRequest<TResponse>` | 返回数据的操作。 |
| `PagedRequest<TItem>` | 列表操作。增加 `SkipCount`、`MaxResultCount` 和 `Sorting`，并可配合 `ListAllAsync` 使用。 |

请求通过几个重写成员来描述自己。对于 POST 和 PUT，请求对象本身就是 JSON 请求体（camelCase，
省略 null 值）。查询参数在 `AddQueryParameters` 中添加，发送前的检查写在 `Validate` 中。
要调用 SDK 尚未覆盖的端点，请编写你自己的请求：

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

你的请求可以获得内置请求的一切能力：凭据、按安全类别执行的重试、请求间隔控制、只读模式和
错误处理。当 API 期望不同形状的请求体时，请重写 `GetBody()`；当某个写入操作可以安全地重复执行时，请重写 `IsIdempotent`。

<a id="errors"></a>

## 错误

每个失败的请求都会抛出 `DomainNameApiException`。无效参数会在发送任何内容之前抛出 `ArgumentException`。

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

| 属性 | 含义 |
|---|---|
| `Kind` | 稳定的分类（见下表）。请根据它分支处理，而不是根据消息。 |
| `Code` | API 的错误代码（如 `Dna.DomainService:Domain:10007`），或下面列出的 SDK 自身的代码之一。 |
| `Message` | 发生了什么。模板式的消息会被替换为 API 给出的详情。 |
| `StatusCode` | HTTP 状态码；没有收到响应时为 null。 |
| `Details`、`ValidationErrors` | 来自 API 的附加详情和字段级问题。 |
| `CorrelationId` | 网关的 `X-Correlation-Id`。联系支持人员时请提供它。 |
| `Hint` | 关于下一步该怎么做的简短建议。 |
| `IsRetryable` | 稍后重复同一请求是否可能成功。 |
| `OutcomeUnknown` | 一个会更改数据的请求在网络上失败，因此它可能已经执行。 |
| `Operation`、`Safety` | 失败的请求（`POST domains/renew`）及其安全类别。 |
| `RetryAfter`、`ResponseBody` | 网关的 `Retry-After` 和原始响应。 |

| `Kind` | 何时出现 |
|---|---|
| `Authentication` | 凭据错误或缺失，或者该经销商在此环境中未知或已被停用（401、重定向）。 |
| `Forbidden` | 已通过身份验证，但无权执行（403）。 |
| `NotFound` | 域名、联系人、订单或其他资源不存在（404）。 |
| `Validation` | API 拒绝了输入，或业务规则未通过：字段错误、域名已锁定、余额不足（400、422）。 |
| `Conflict` | 资源的状态不允许该操作（409）。 |
| `RateLimited` | 自动重试后仍受速率限制（429）。 |
| `Server` | 服务器端故障（5xx）。通常是暂时的。 |
| `Network`、`Timeout` | 没有响应：DNS、TLS、代理、连接被重置，或在 `Timeout` 内没有响应。 |
| `ReadOnly` | 被只读模式阻止；未发送任何内容。 |
| `Unknown` | 其他任何情况，包括 SDK 无法读取的响应。 |

SDK 自行设置的代码：

| 代码 | 含义 |
|---|---|
| `INVALID_RESELLER` | 网关重定向到了它的停用页面：该 Reseller ID 在**此环境中**未知、未激活或已被停用。 |
| `INVALID_API_KEY` | 该密钥与经销商不匹配。正式环境和 OTE 的密钥不同。 |
| `MISSING_API_KEY`、`MISSING_RESELLER_ID`、`NO_CREDENTIALS` | 某个凭据为空。 |
| `SUBRESELLER_NOT_ALLOWED` | 子经销商账户不能使用 API。请使用主经销商的凭据。 |
| `UNAUTHORIZED` | 其他任何 401。 |
| `RATE_LIMITED` | 重试之后仍返回 429。 |
| `NETWORK`、`TIMEOUT` | 没有收到响应。 |
| `READ_ONLY` | 被只读模式阻止。 |
| `EMPTY_RESPONSE`、`INVALID_RESPONSE` | 成功的响应为空或无法读取。 |
| `HTTP_<status>` | API 没有返回错误代码。 |

<a id="safety-retries-and-rate-limits"></a>

## 安全、重试与速率限制

每个请求都有一个安全类别，与 `dna` CLI 使用的相同：

| `OperationSafety` | 含义 | 示例 |
|---|---|---|
| `Read` | 没有副作用，即使 API 使用 POST 或 PUT | 搜索、详情、列表、续费和转移检查、CSR 生成 |
| `Write` | 免费且可撤销的更改 | DNS 记录、锁定、隐私保护、名称服务器、联系人 |
| `Billable` | 扣除你的余额 | 注册、续费、转移、恢复、购买 SSL、订购主机 |
| `Destructive` | 删除数据或难以撤销 | 删除 DNS 记录、移除 DNSSEC、吊销证书 |

- **只读模式**（`ReadOnly = true` 或 `DNA_READ_ONLY=true`）会在发送之前阻止一切不属于 `Read` 的请求。
  浏览探索、生成报表和搭建仪表板时可以使用。
- **重试。** 429 响应总是会重试，并遵循 `Retry-After`。即使对需付费的请求，这样做也是安全的，因为
  网关会在执行任何操作之前拒绝它们。网络错误、超时和 502/503/504 响应只会对读取请求重试
  （以及可以安全重复的写入请求，例如替换名称服务器）。注册或续费绝不会被发送两次。
- **请求间隔控制。** 网关允许每个密钥每秒 5 个请求，域名搜索每秒 2 个，DNS 区域调用每秒 20 个。
  客户端会控制请求间隔，使其保持在这些限制之内（区域调用间隔 1 秒）。域名信息和
  TLD 列表不受速率限制。
- **软配额。** 搜索和信息查询消耗 1 点，其他一切消耗 10 点。超过 200 点后，网关会让每个
  请求最多延迟 5 秒；点数每分钟衰减 20%。`SendWithResponseAsync` 会在 `Quota` 中显示当前状态。

<a id="dependency-injection"></a>

## 依赖注入

`DomainNameApiClient` 可以作为类型化的 `HttpClient` 使用：

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

SDK 自己创建的 `HttpClient` 不会跟随重定向。由工厂创建的客户端可能会跟随；SDK 仍然能识别
网关的停用页面，并报告 `INVALID_RESELLER`。

<a id="testing-your-code"></a>

## 测试你的代码

传入一个带有桩处理程序的 `HttpClient`，即可在不调用 API 的情况下测试使用该 SDK 的代码：

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

要针对真实 API 测试，请使用 OTE 及其专用的密钥。

<a id="project-layout"></a>

## 项目结构

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

每个领域的文件夹结构相同：`XxxClient.cs`、一个每个操作对应一个类的 `Requests/` 文件夹，以及一个存放响应和枚举的
`Models/` 文件夹。同一领域的所有类型共用一个命名空间，例如 `DomainNameApi.Domains`。

<a id="development"></a>

## 开发

```bash
dotnet build
dotnet test
```

测试从不调用真实 API：它们使用一个伪造的 `HttpMessageHandler`。在 Windows 上，测试会运行在 .NET 10 和
.NET Framework 4.8 上；后者用于检验 .NET Standard 2.0 版本的构建。

`tests/DomainNameApi.Tests/testdata/openapi.json` 是 `dna` CLI 内嵌的 API 文档。`SpecCoverageTests` 会检查
其中的每个操作都有且只有一个请求类，并且安全类别与 CLI 一致。当 API 新增操作时，
请更新该文档；在对应的请求类创建出来之前，测试都会失败。

<a id="upgrading-from-1x"></a>

## 从 1.x 升级

2.0 是全新的 SDK。1.x 是 PHP 库的移植版本，返回无类型的 `JsonNode` 信封；2.0 提供带类型的请求
和模型、异步方法以及异常。

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

1.x 会向 Sentry 发送错误报告。2.0 除了 API 之外，不会向任何地方发送任何内容。

## 链接

- [`dna` CLI 和 MCP 服务器](https://github.com/domainreseller/cli)：在终端和 AI 智能体中使用同一套 API。
- [API 文档](https://www.domainnameapi.com/docs/)
- [更新日志](CHANGELOG.md)

## 许可证

Apache License 2.0。参见 [LICENSE](LICENSE)。
