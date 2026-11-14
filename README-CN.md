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

# DomainNameAPI .NET 库 (dotnet-dna)

DomainNameAPI 经销商 REST API 的官方 .NET 客户端：域名搜索、注册、续费、转移、联系人、名称服务器、子名称服务器、DNS 区域以及 URL 转发。

dotnet-dna 是 [php-dna](https://github.com/domainreseller/php-dna) 的忠实移植版本：相同的方法、在网络上发送相同的请求、返回相同的值。它通过 php-dna 的一致性测试用例（conformance fixtures，共 134 个场景）进行校验，因此针对 PHP 库编写的代码可以一一对应地迁移过来。

## 📦 下载 — 请务必使用 Releases！

⬇️ **在此获取最新的经过测试的版本：https://github.com/domainreseller/dotnet-dna/releases/latest**

> ⚠️ 请**不要**使用绿色的 **Code → Download ZIP** 按钮 — 它下载的是未经测试的开发分支。Release 包经过版本管理和测试，可直接用于生产环境。

## 安装和集成指南

### 最低要求

- .NET 8 或更高版本，**或**任何支持 .NET Standard 2.0 的运行时（.NET Framework 4.6.2+、.NET Core 2.0+、Mono、Xamarin、Unity）。
- 无第三方依赖。本库使用 `HttpClient` 和 `System.Text.Json`（在 .NET Standard 2.0 上会自动引入 `System.Text.Json` 包）。

### 安装

**A) NuGet 包** — 包 ID 为 `DomainNameApi`。每个发布版本都附带 `.nupkg` 文件；一旦该包在 NuGet.org 上架，您也可以直接安装：

```bash
dotnet add package DomainNameApi
```

要安装已下载的 `.nupkg`，请将其放入一个文件夹，并将该文件夹用作包源：

```bash
dotnet add package DomainNameApi --source ./packages
```

**B) 项目引用** — 下载发布版本的源代码并引用该项目：

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

## 🔑 API 凭据 — Reseller ID 与 API Key

本库通过 DomainNameAPI REST API 通信，只需向构造函数传入两项凭据：

| 参数 | 值 |
|---|---|
| 第一个 | Reseller ID — 形如 `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` 的 UUID |
| 第二个 | API Key |

> 💡 您可以在 DomainNameAPI 面板的 **API 设置** 中找到您的 **Reseller ID** 和 **API Key**。
> ⚠️ 这些是 **API 凭据** — 面板登录信息在此**无法使用**。

```csharp
using DomainNameApi;

var resellerId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
var apiKey     = "your-api-key";

// Live environment
var dna = new DomainNameApiClient(resellerId, apiKey);

// Test (OTE) environment
var dna = new DomainNameApiClient(resellerId, apiKey, true);
```

两项凭据首尾的空白字符都会被去除。测试模式标志也可以以文本形式传入，与配置文件和面板的存储方式一致：`"true"`、`"1"`、`"on"` 和 `"yes"`（不区分大小写）选择测试环境，其他任何值则选择正式环境。

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey, Environment.GetEnvironmentVariable("DNA_TEST_MODE"));
```

## 🚀 快速入门

```csharp
using System;
using DomainNameApi;

using var dna = new DomainNameApiClient("xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "your-api-key");

var result = dna.GetDetails("example.com");

if ((string)result["result"] == "OK")
{
    Console.WriteLine("Status:  " + (string)result["data"]["Status"]);
    Console.WriteLine("Expires: " + (string)result["data"]["Dates"]["Expiration"]);
}
else
{
    Console.WriteLine("Error " + (string)result["error"]["Code"] + ": " + (string)result["error"]["Message"]);
}
```

### 返回值

每个方法返回的结构都与 php-dna 相同，类型为 `System.Text.Json.Nodes.JsonNode`（通常为 `JsonObject`）。可通过类型转换或 `GetValue<T>()` 读取值，或使用 `ToJsonString()` 打印完整结果：

```csharp
var details = dna.GetDetails("example.com");
string status = (string)details["data"]["Status"];          // "Active"
int    days   = (int)details["data"]["Dates"]["RemainingDays"];
bool   locked = (string)details["data"]["LockStatus"] == "true";   // flags are the strings "true"/"false"
Console.WriteLine(details.ToJsonString());
```

键名、字符串标志（`"true"`/`"false"`）以及格式化的金额字符串（`"11.9900"`）与 php-dna 完全一致。大多数方法使用 `result` = `OK`/`ERROR` 的响应封装；`GetCurrentBalance` 使用旧版的 `OperationResult` 封装，而 `CheckAvailability` 在成功时直接返回一个列表。

联系人、TR 属性和列表筛选条件等输入均为 `IDictionary<string, object>`，键名与 php-dna 相同（`FirstName`、`EMail`、`AddressLine1`、`PhoneCountryCode`、`TRABISCITIZENID`、`PageSize` 等）。名称服务器列表可以是列表（`string[]`、`List<string>`），也可以是字典（使用其值）。

### 使用方法

以下所有示例也可以在 [examples](examples) 文件夹中运行（参见[示例](#示例)）。

#### 域名注册

注意：.tr 域名需要额外参数。`additionalAttributes` 参数用于 .tr 等需要额外信息的域名。

```csharp
var contact = new Dictionary<string, object>
{
    ["FirstName"]        = "John",
    ["LastName"]         = "Doe",
    ["Company"]          = "Example Corp",
    ["EMail"]            = "john.doe@example.com",
    ["AddressLine1"]     = "123 Lorem Street",
    ["AddressLine2"]     = "Suite 456",
    ["AddressLine3"]     = "",
    ["City"]             = "Springfield",
    ["Country"]          = "US",
    ["Fax"]              = "1234567890",
    ["FaxCountryCode"]   = "1",
    ["Phone"]            = "9876543210",
    ["PhoneCountryCode"] = 1,
    ["Type"]             = "Contact",
    ["ZipCode"]          = "12345",
    ["State"]            = "IL",
};

var result = dna.RegisterWithContactInfo(
    "example.com",
    1,
    new Dictionary<string, object>
    {
        ["Administrative"] = contact,
        ["Billing"]        = contact,
        ["Technical"]      = contact,
        ["Registrant"]     = contact,
    },
    new[] { "ns1.example.com", "ns2.example.com" },
    eppLock: true,
    privacyLock: false,
    // Additional attributes are only required for .tr domains
    additionalAttributes: new Dictionary<string, object>
    {
        ["TRABISDOMAINCATEGORY"] = 1,
        ["TRABISCITIZIENID"]     = "12345678901",
        ["TRABISNAMESURNAME"]    = "John Doe",
        ["TRABISCOUNTRYID"]      = "840",
        ["TRABISCITYID"]         = "17",
    });
```

联系人在发送前会被补全：缺失的字段会填入占位值，姓名中的数字和标点会被移除，州/省缺失或为空时回退为城市，国家名称会转换为 ISO 代码，电话号码会拆分为号码和国家代码 — 以 `+` 或 `00` 自带国家代码书写的号码会保留该代码（参见 `ValidateContact`）。如果未提供名称服务器（或仅提供了空值），则使用 `ns1.domainnameapi.com` 和 `ns2.domainnameapi.com`。对于 .tr 域名，TRABIS 属性会被精简为网关所接受的内容（`TRABISDOMAINCATEGORY` = `Owner`/`Corporate`、`TRABISCITIZENID`、`TRABISTAXOFFICE`、`TRABISTAXNUMBER`）；旧版键 `TRABISCITIZIENID` 和数字形式的类别会自动转换。

<details>
<summary>域名注册示例输出</summary>

```json
{
  "data": {
    "ID": 123456,
    "Status": "Active",
    "DomainName": "example.com",
    "AuthCode": "Xy9#mK2$pL5@vN8",
    "LockStatus": "true",
    "PrivacyProtectionStatus": "false",
    "IsChildNameServer": "false",
    "Contacts": {
      "Billing": { "ID": 987654 },
      "Technical": { "ID": 987654 },
      "Administrative": { "ID": 987654 },
      "Registrant": { "ID": 987654 }
    },
    "Dates": { "Start": "2024-03-15T10:00:00", "Expiration": "2025-03-15T10:00:00", "RemainingDays": 365 },
    "NameServers": [ "ns1.example.com", "ns2.example.com" ],
    "Additional": [],
    "ChildNameServers": []
  },
  "result": "OK"
}
```

</details>

如果网关拒绝该订单（返回 `"success": false`），该方法会返回错误封装，其中 `Code` = `REGISTER_DOMAIN` 并附带网关的消息，因此被拒绝的注册绝不会被报告为已完成。`Renew`（`RENEW_DOMAIN`）和 `Transfer`（`TRANSFER_DOMAIN`）的行为相同。

<hr style="border: 4px solid #000; border-style: dashed;">

#### 域名续费

```csharp
var result = dna.Renew("example.com", 1);
```

<details>
<summary>域名续费示例输出</summary>

```json
{ "result": "OK", "data": { "ExpirationDate": "2025-03-15T10:00:00" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 域名转移

```csharp
var result = dna.Transfer("example.com", "Xy9#mK2$", 3);
```

> ℹ️ 转移从不发送联系人：请求始终携带空的联系人列表，由注册商自行接管联系人。因此 `Transfer` 没有联系人参数；如有需要，请在转移完成后使用 `SaveContacts` 更新联系人。

<details>
<summary>域名转移示例输出</summary>

```json
{
  "data": {
    "ID": 0, "Status": "Active", "DomainName": "", "AuthCode": "",
    "LockStatus": "false", "PrivacyProtectionStatus": "false", "IsChildNameServer": "false",
    "Contacts": { "Billing": { "ID": 0 }, "Technical": { "ID": 0 }, "Administrative": { "ID": 0 }, "Registrant": { "ID": 0 } },
    "Dates": { "Start": "", "Expiration": "2027-11-09T12:00:00", "RemainingDays": 365 },
    "NameServers": [], "Additional": [], "ChildNameServers": []
  },
  "result": "OK"
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 取消、批准或拒绝转移

```csharp
dna.CancelTransfer("example.com");   // pending incoming transfer
dna.ApproveTransfer("example.com");  // pending outgoing transfer
dna.RejectTransfer("example.com");   // pending outgoing transfer
```

<details>
<summary>取消转移示例输出</summary>

```json
{ "result": "OK", "data": { "DomainName": "example.com", "Status": "Cancelled" } }
```

`ApproveTransfer` 和 `RejectTransfer` 以相同方式响应，状态分别为 `Approved` / `Rejected`。

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 转移检查

```csharp
var result = dna.CheckTransfer("example.com", "Ab1!xyZ9");
```

仅当注册局报告该域名可转移时，`result` 才为 `OK`；否则为 `ERROR`，`Code` = `TRANSFER_NOT_AVAILABLE`，详细信息仍位于 `data` 中。

<details>
<summary>转移检查示例输出</summary>

```json
{
  "result": "OK",
  "data": {
    "TransferAvailabilityStatus": true,
    "AuthCodeIsRequired": true,
    "AuthCodeIsValid": true,
    "UserTransferRequired": false,
    "TransferLock": false,
    "Message": "Domain can be transferred",
    "MessageKey": "TransferAvailable",
    "Additional": []
  }
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 域名列表

```csharp
var result = dna.GetList(new Dictionary<string, object>
{
    ["OrderColumn"]    = "Id",
    ["OrderDirection"] = "ASC",
    ["PageNumber"]     = 0,
    ["PageSize"]       = 1000,
});
```

`PageNumber` 从零开始。也可以传入网关自身的筛选参数名（`MaxResultCount`、`SkipCount`、`Sorting`、`DomainName` 等）。

<details>
<summary>域名列表示例输出</summary>

```json
{
  "data": {
    "Domains": [
      {
        "ID": 564346,
        "Status": "Active",
        "DomainName": "example.com",
        "AuthCode": "",
        "LockStatus": "false",
        "PrivacyProtectionStatus": "false",
        "IsChildNameServer": "false",
        "Contacts": { "Billing": { "ID": "" }, "Technical": { "ID": "" }, "Administrative": { "ID": "" }, "Registrant": { "ID": "" } },
        "Dates": { "Start": "2024-03-15T10:00:00", "Expiration": "2025-03-15T10:00:00", "RemainingDays": 365 },
        "NameServers": [ "ns1.example.com", "ns2.example.com" ],
        "Additional": [],
        "ChildNameServers": []
      }
    ]
  },
  "result": "OK",
  "TotalCount": 1
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### TLD 列表

```csharp
var result = dna.GetTldList(100);
```

<details>
<summary>TLD 列表示例输出</summary>

```json
{
  "data": [
    {
      "id": 1,
      "status": "Active",
      "maxchar": 63,
      "maxperiod": 10,
      "minchar": 1,
      "minperiod": 1,
      "tld": "com",
      "pricing": {
        "registration": { "1": "10.8100" },
        "renew": { "1": "11.0100" },
        "transfer": { "1": "10.6100" },
        "restore": { "1": "99.9000" }
      },
      "currencies": { "registration": "USD", "renew": "USD", "transfer": "USD", "restore": "USD" }
    }
  ],
  "result": "OK"
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 域名可用性检查

```csharp
var result = dna.CheckAvailability(new[] { "hello", "world123x0" }, new[] { "com", "net" }, 1, "create");
```

<details>
<summary>域名可用性检查示例输出</summary>

```json
[
  {
    "TLD": "com", "DomainName": "hello", "Status": "notavailable", "Command": "create", "Period": 1,
    "IsFee": false, "Price": "10.8100", "Currency": "USD", "Reason": "Domain exists"
  },
  {
    "TLD": "com", "DomainName": "world123x0", "Status": "available", "Command": "create", "Period": 1,
    "IsFee": false, "Price": "10.8100", "Currency": "USD", "Reason": null
  }
]
```

失败时，该方法返回常规的错误封装而不是列表，因此请先检查 `result is JsonArray`。

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 域名详情

```csharp
var result = dna.GetDetails("example.com");
var synced = dna.SyncFromRegistry("example.com"); // same envelope
```

<details>
<summary>域名详情示例输出</summary>

```json
{
  "data": {
    "ID": 564346,
    "Status": "Active",
    "DomainName": "example.com",
    "AuthCode": "DHQ!K52",
    "LockStatus": "true",
    "PrivacyProtectionStatus": "false",
    "IsChildNameServer": "true",
    "Contacts": {
      "Billing": { "ID": "DNA-B-4" },
      "Technical": { "ID": "DNA-T-3" },
      "Administrative": { "ID": "DNA-A-2" },
      "Registrant": { "ID": "DNA-R-1" }
    },
    "Dates": { "Start": "2025-05-26T16:08:37", "Expiration": "2027-05-26T16:08:37", "RemainingDays": 449 },
    "NameServers": [ "ns1.example.com", "ns2.example.com" ],
    "Additional": [],
    "ChildNameServers": [ { "ns": "ns1.example.com", "ip": "1.2.3.4" } ]
  },
  "result": "OK"
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 修改名称服务器

```csharp
var result = dna.ModifyNameServer("example.com", new[] { "ns1.example.com", "ns2.example.com" });
```

主机名会去除首尾空白、转换为小写并去掉末尾的点；空条目和重复条目会被丢弃。

<details>
<summary>修改名称服务器示例输出</summary>

```json
{ "result": "OK", "data": { "NameServers": [ "ns1.example.com", "ns2.example.com" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 启用域名锁定

```csharp
var result = dna.EnableTheftProtectionLock("example.com");
```

<details>
<summary>启用域名锁定示例输出</summary>

```json
{ "result": "OK", "data": { "LockStatus": true } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 禁用域名锁定

```csharp
var result = dna.DisableTheftProtectionLock("example.com");
```

<details>
<summary>禁用域名锁定示例输出</summary>

```json
{ "result": "OK", "data": { "LockStatus": false } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 添加子名称服务器

```csharp
var result = dna.AddChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>添加子名称服务器示例输出</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 删除子名称服务器

```csharp
var result = dna.DeleteChildNameServer("example.com", "ns1.example.com");
```

<details>
<summary>删除子名称服务器示例输出</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 修改子名称服务器

```csharp
var result = dna.ModifyChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>修改子名称服务器示例输出</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 修改隐私保护状态

```csharp
var result = dna.ModifyPrivacyProtectionStatus("example.com", true);

// The flag may also be text, as panels store it: "1", "true", "on" and "yes" enable privacy
var result2 = dna.ModifyPrivacyProtectionStatus("example.com", "on");
```

可选的第三个参数（`reason`）仅为兼容性而保留；网关没有该字段，也不会发送它。

<details>
<summary>修改隐私保护状态示例输出</summary>

```json
{ "data": { "PrivacyProtectionStatus": true }, "result": "OK" }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 获取域名联系人

```csharp
var result = dna.GetContacts("example.com");
```

<details>
<summary>获取域名联系人示例输出</summary>

```json
{
  "data": {
    "contacts": {
      "Administrative": {
        "ID": "DNA-A-2",
        "Status": "Active",
        "AuthCode": "",
        "FirstName": "Grace",
        "LastName": "Hopper",
        "Company": "",
        "EMail": "grace@example.com",
        "Type": "Administrative",
        "Address": {
          "Line1": "Example Street 2", "Line2": "", "Line3": "",
          "State": "DC", "City": "Washington", "Country": "US", "ZipCode": "20001"
        },
        "Phone": {
          "Phone": { "Number": "2025550100", "CountryCode": "1" },
          "Fax": { "Number": "2025550101", "CountryCode": "1" }
        },
        "Additional": []
      },
      "Billing": { "...": "..." },
      "Registrant": { "...": "..." },
      "Technical": { "...": "..." }
    }
  },
  "result": "OK"
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 保存域名联系人

```csharp
var contact = new Dictionary<string, object>
{
    ["FirstName"]        = "John",
    ["LastName"]         = "Doe",
    ["Company"]          = "Example Corp",
    ["EMail"]            = "john@example.com",
    ["AddressLine1"]     = "123 Main Street",
    ["AddressLine2"]     = "",
    ["AddressLine3"]     = "",
    ["City"]             = "Springfield",
    ["Country"]          = "US",
    ["Fax"]              = "5559876543",
    ["FaxCountryCode"]   = "1",
    ["Phone"]            = "5551234567",
    ["PhoneCountryCode"] = 1,
    ["Type"]             = "Contact",
    ["ZipCode"]          = "62701",
    ["State"]            = "IL",
};

var result = dna.SaveContacts("example.com", new Dictionary<string, object>
{
    ["Administrative"] = contact,
    ["Billing"]        = contact,
    ["Technical"]      = contact,
    ["Registrant"]     = contact,
});
```

联系人也可以使用嵌套结构（`Address` → `Line1`/`City`/...，`Phone` → `Phone`/`Fax` → `Number`/`CountryCode`），即 `GetContacts` 返回的结构。

<details>
<summary>保存域名联系人示例输出</summary>

```json
{ "result": "OK", "data": { "contacts": [] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 获取经销商详细信息

```csharp
var result = dna.GetResellerDetails();
```

<details>
<summary>获取经销商详细信息示例输出</summary>

```json
{
  "result": "OK",
  "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "active": true,
  "name": "Example Reseller",
  "balance": "500.00",
  "currency": "USD",
  "symbol": "$",
  "balances": [
    { "balance": "500.00", "currency": "USD", "symbol": "$" },
    { "balance": "1500.00", "currency": "TL", "symbol": "TL" }
  ]
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 当前余额

```csharp
var result = dna.GetCurrentBalance("USD"); // USD, TRY, EUR or GBP
```

<details>
<summary>当前余额示例输出</summary>

```json
{
  "ErrorCode": 0,
  "OperationMessage": "Command completed succesfully.",
  "OperationResult": "SUCCESS",
  "Balance": "601.64",
  "CurrencyId": 2,
  "CurrencyInfo": null,
  "CurrencyName": "USD",
  "CurrencySymbol": "$"
}
```

失败时，`OperationResult` 为 `FAILED`，`ErrorCode` 包含错误代码（例如 `API_429`），`Balance` 为 `"0.00"`。账户仅报告 USD 和 TRY 余额；请求其他货币（EUR、GBP 等）会失败，`ErrorCode` = `BALANCE`，消息为 `No EUR balance is reported for this account. Available: USD, TRY.`

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### DNS 区域管理

网关以记录**集**（名称 + 类型 → 值）的形式存储区域，并在每次写入时替换整个记录集。`*ResourceRecordSet` 和 `*ResourceRecordValue` 方法会替您处理这一点。名称可以是相对名称（`www`）、区域顶点（`@`、`""` 或 `example.com`），也可以是完全限定名称（`www.example.com.`）。支持的类型：`A`、`AAAA`、`ALIAS`、`CAA`、`CNAME`、`DS`、`MX`、`NAPTR`、`PTR`、`SRV`、`TLSA`、`TXT`。除非您传入 `apply: false` 并自行调用 `ApplyResourceRecords`，否则写入会立即生效；如果该自动应用失败，方法会返回应用错误（更改仍处于暂存状态，尚未生效）。连续的区域调用之间至少间隔一秒，因为网关会对突发请求返回 HTTP 429。

```csharp
// List every record (one row per value; names are fully qualified)
var records = dna.GetResourceRecords("example.com");

// Make a set hold exactly these values
dna.SetResourceRecordSet("example.com", "@", "MX", new[] { "10 mx1.example.com", "20 mx2.example.com" }, 3600);

// Add / remove / replace one value, keeping the others
dna.AddResourceRecordValue("example.com", "@", "MX", "30 mx3.example.com");
dna.RemoveResourceRecordValue("example.com", "@", "MX", "20 mx2.example.com.");
dna.ReplaceResourceRecordValue("example.com", "@", "A", "192.0.2.1", "192.0.2.2");

// Remove a whole set
dna.DeleteResourceRecordSet("example.com", "old", "A");

// Low-level calls (one write = one set)
dna.AddResourceRecord("example.com", "www", "A", "192.0.2.5", 300);
dna.EditResourceRecord("example.com", "www", "www", "A", "192.0.2.6", 600);
dna.DeleteResourceRecord("example.com", "www", "192.0.2.6", "A");

// Stage several changes, then commit once
dna.AddResourceRecord("example.com", "api", "A", "192.0.2.7", 300, apply: false);
dna.AddResourceRecord("example.com", "cdn", "CNAME", "cdn.example.net.", 300, apply: false);
dna.ApplyResourceRecords("example.com");
```

<details>
<summary>DNS 区域管理示例输出</summary>

`GetResourceRecords`:

```json
{
  "result": "OK",
  "data": {
    "records": [
      { "Name": "example.com.", "Type": "A", "TTL": 3600, "Content": "192.0.2.1", "Disabled": false },
      { "Name": "www.example.com.", "Type": "CNAME", "TTL": 300, "Content": "example.com.", "Disabled": false },
      { "Name": "example.com.", "Type": "MX", "TTL": 3600, "Content": "10 mx1.example.com.", "Disabled": false }
    ]
  }
}
```

`SetResourceRecordSet` / `AddResourceRecordValue`（类型以大写形式返回，TTL 按发送值返回，范围限制为 1–86400 秒）：

```json
{ "result": "OK", "data": { "Name": "", "Type": "MX", "TTL": 3600, "Contents": [ "10 mx1.example.com", "20 mx2.example.com" ] } }
```

添加已存在的值或删除不存在的值不会产生任何更改，并返回 `{"result":"OK","data":{..., "Changed": false}}`。不支持的类型会在发出任何调用之前被拒绝：

```json
{ "result": "ERROR", "error": { "Code": "ADD_ZONE", "Message": "Record type \"NS\" is not supported by the DNS zone API. Supported types: A, AAAA, ALIAS, CAA, CNAME, DS, MX, NAPTR, PTR, SRV, TLSA, TXT.", "Details": "..." } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### URL 转发

```csharp
dna.SetForwarding("example.com", "https://www.example.org", "Standard"); // or "Frame" (masked)
var forwarding = dna.GetForwarding("example.com");
dna.DeleteForwarding("example.com");
```

<details>
<summary>URL 转发示例输出</summary>

```json
{ "result": "OK", "data": { "Enabled": true, "RedirectAddress": "https://www.example.org/landing", "ForwardType": "Frame" } }
```

未配置转发时（网关返回 `Redirect information not found`），`GetForwarding` 返回 `{"result":"OK","data":{"Enabled":false,"RedirectAddress":"","ForwardType":""}}`。其他任何错误 — 例如域名不存在 — 都会作为错误返回。

`DeleteForwarding` 返回 `{"result":"OK","data":{"DomainName":"example.com","Removed":true}}`；如果原本没有任何配置，则不执行任何操作并返回 `"Removed": false`。

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### 实用工具（无网络调用）

```csharp
bool isTr = dna.IsTrTLD("example.com.tr"); // true

var contact = dna.ValidateContact(new Dictionary<string, object>
{
    ["FirstName"] = " Jo ",
    ["Phone"]     = "+1 (202) 555-0100",
    ["Country"]   = "Turkey",
});
```

<details>
<summary>ValidateContact 示例输出</summary>

```json
{
  "FirstName": "Jo", "Phone": "12025550100", "Country": "TR", "LastName": "Isimyok",
  "AddressLine1": "Addres yok", "City": "ISTANBUL", "State": "ISTANBUL", "ZipCode": "34000",
  "PhoneCountryCode": "90", "Fax": "", "FaxCountryCode": ""
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

### 错误处理

方法在遇到 API 或网络问题时从不抛出异常，而是返回错误封装：

```json
{
  "result": "ERROR",
  "error": {
    "Code": "API_404",
    "Message": "The domain name you requested could not be found. Please check and try again.",
    "Details": "The domain could not be found. Please check and try again.",
    "GatewayCode": "Dna.DomainService:Domain:10007"
  }
}
```

| 情况 | `Code` | `Message` |
|---|---|---|
| 网关返回的 HTTP 错误 | `API_<status>`，例如 `API_400`、`API_429`、`API_500` | 网关的消息；响应体为空时为 `HTTP 429 - Too Many Requests` |
| 凭据错误（HTTP 301/302） | `API_302` | `Invalid API credentials` |
| 验证错误 | `API_400` | 网关的消息；`Details` 列出相关字段，例如 `Admin PostalCode - 'PostalCode' is required` |
| 超时 | `CURL_28` | `Could not reach the DomainNameAPI service: the connection timed out. (...)` |
| 其他网络故障（DNS、连接、TLS、连接重置） | `CURL_6`、`CURL_7`、`CURL_60`、`CURL_56` | `Curl error during request: ...` |
| 注册、续费或转移被拒绝（`"success": false`） | `REGISTER_DOMAIN`、`RENEW_DOMAIN`、`TRANSFER_DOMAIN` | 网关的消息 |
| 成功响应为空或无法使用 | `RESPONSE` | `Invalid response received from server! Response is empty.` |
| 调用网关之前执行的检查 | 例如 `AVAILABILITY`、`ADD_ZONE`、`SET_FORWARD` | 问题描述 |

`API_<n>` 代码始终表示 HTTP 状态。当网关发送了自己的错误代码时，封装中还会以 `GatewayCode` 携带该代码（例如 `Dna.DomainService:Domain:10007`），这样无需解析消息即可区分"域名未找到"和"不支持该 TLD"。网络故障使用 `CURL_<n>`，其中的数字沿用 libcurl 的编号，以便与 php-dna 的代码保持一致。只有编程错误（例如传入不受支持的对象类型）才可能抛出异常。

排查问题时，可以检查最近一次调用：

```csharp
dna.GetLastFunction();       // "request:GET domains/info"
dna.GetRequestData();        // { "url": ..., "method": ..., "payload": ... }
dna.GetResponseData();       // decoded response, or { "raw_response": "..." }
dna.GetResponseHeaders();    // { "url", "http_code", "content_type", "total_time", "headers" }
dna.GetParsedResponseData(); // parsed response or the error built from it
```

### 测试模式

将 `true` 作为构造函数的第三个参数传入，即可使用 OTE 测试环境（`https://ote.domainresellerapi.com/api/v1`）而非正式环境（`https://api.domainresellerapi.com/api/v1`）。`GetServiceUrl()` 显示当前使用的地址，`SetServiceUrl(url)` 可覆盖该地址。

### 错误报告（遥测）

当 API 调用失败时，本库可以向 DomainNameAPI 自己的错误跟踪系统（Sentry）发送错误报告。成功调用中的一小部分随机样本（4%，以及所有耗时超过 1 秒的调用）可能会作为性能记录发送。这有助于 DomainNameAPI 发现并修复问题。错误报告**默认开启**。

**发送的内容：**

- 错误代码和消息
- 调用的端点路径和 HTTP 方法
- 调用耗时
- 应用程序名称（`DomainNameAPI-DotNet`）、库版本和 .NET 版本
- 经销商 ID 和域名
- 服务器的公网 IP 地址（在后台通过 `api.ipify.org` 查询）
- 运行应用程序的操作系统账户名
- API 请求和响应，包括域名的联系人信息；仅对凭据进行掩码处理 — EPP/授权码、API 密钥、密码和令牌会被替换为 `***`

**永不发送的内容：** 可读形式的 EPP/授权码、API 密钥、密码或令牌；堆栈跟踪中的方法参数。

**用途与保留期限：** 这些数据仅用于使用情况统计和问题的早期诊断，在 DomainNameAPI 自己的 Sentry 服务器上保留 30 天，之后自动删除。

报告在后台发送：绝不会阻塞或减慢您的调用，也绝不会抛出异常。预期内的高频错误（域名未找到、余额不足、凭据无效、未配置转发等）不会被报告。

**如何关闭：** 在创建客户端后调用 `SetErrorReportingEnabled(false)`：

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey);
dna.SetErrorReportingEnabled(false);
```

**最小模式：** `SetTelemetryMinimal(true)` 保持报告开启，但仅发送错误代码和消息、端点、耗时、版本信息和经销商 ID — 不包含请求/响应数据、域名、IP 地址或账户名。

可以通过 `TelemetryDsn` 属性更改发送目标（例如改为您自己的 Sentry 项目）。

> 🔒 **安全性：** TLS 证书验证已开启。在 CA 证书包过旧的主机上，可以通过 `dna.SetSslVerification(false);` 将其关闭 — 不建议这样做；更新 CA 证书包才是更好的解决方法。

### 高级用法

- **超时：** 每个请求 30 秒，连接 10 秒；可通过 `Timeout` 和 `ConnectTimeout` 属性修改。
- **自定义传输层：** 向构造函数传入 `IHttpTransport`（`new DomainNameApiClient(id, key, false, transport)`）或设置 `Transport` 属性，即可通过您自己的 HTTP 栈、代理或测试替身（test double）发送请求。实现需要返回所有 HTTP 状态（不跟随重定向），并在网络故障时抛出 `TransportException`。
- **线程：** 客户端会保留最近一次的请求和响应以便诊断，因此不是线程安全的。请为每个线程（或每个请求）使用一个实例，或对访问进行同步。`Dispose()` 会释放底层的 `HttpClient`。
- **User-Agent：** 请求以 `DomainNameAPI-DotNet/1.0.1` 的身份发送。

### 示例

[examples](examples/DomainNameApi.Examples) 项目为每个方法提供一个可运行的文件（`GetDetails`、`RegisterWithContactInfo`、`RegisterWithContactInfo-TR`、DNS 区域和转发相关方法等）。设置好您的凭据后运行其中一个：

```bash
export DNA_RESELLER_ID=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
export DNA_API_KEY=your-api-key
export DNA_TEST_MODE=true   # optional: use the OTE test environment

dotnet run --project examples/DomainNameApi.Examples -- GetDetails
dotnet run --project examples/DomainNameApi.Examples            # lists all examples
```

### 测试

测试无需凭据，也不会发起任何外部网络调用：

```bash
dotnet test
```

如果本地没有 .NET SDK，可以在 Docker 中运行：

```bash
docker run --rm -v "$PWD":/src -w /src mcr.microsoft.com/dotnet/sdk:8.0 dotnet test
```

| 测试文件 | 说明 |
|-----------|-------------|
| `tests/DomainNameApi.Tests/ConformanceTests.cs` | 重放 php-dna 的全部 134 个一致性测试用例（`testdata/conformance/`），并比对请求和结果 |
| `tests/DomainNameApi.Tests/HttpLayerTests.cs` | 请求头、URL、查询字符串、测试模式、超时、区域调用节流 |
| `tests/DomainNameApi.Tests/HttpClientTransportTests.cs` | 针对本地监听器测试默认的 `HttpClient` 传输层 |
| `tests/DomainNameApi.Tests/ErrorMappingTests.cs` | 网关错误映射和验证消息 |
| `tests/DomainNameApi.Tests/TelemetryTests.cs` | 遥测负载、凭据掩码、最小模式 |
| `tests/DomainNameApi.Tests/UtilityTests.cs` | 联系人验证、电话/国家规范化、与 PHP 兼容的格式化 |

<hr style="border: 4px solid #000; border-style: dashed;">

## 返回和错误代码及说明

| 代码 | 描述 | 详细 |
|------|------|------|
| 1000 | 命令成功完成 | 操作成功。 |
| 1001 | 命令成功完成；操作待处理 | 操作成功。但是，操作当前已排队等待完成。 |
| 2003 | 缺少必需参数 | 参数缺失错误。例如：联系信息中没有填写电话。 |
| 2105 | 对象不符合续费条件 | 域名状态不适合续费，已锁定更新操作。状态不应为"clientupdateprohibited"。也可能由其他状态条件导致。 |
| 2200 | 身份验证错误 | 授权错误，安全代码不正确或域名已在另一个注册商处注册。 |
| 2302 | 对象已存在 | 域名或名称服务器信息已存在于数据库中。无法注册。 |
| 2303 | 对象不存在 | 域名或名称服务器信息在数据库中不存在。需要新注册。 |
| 2304 | 对象状态禁止操作 | 域名状态不适合更新，已锁定更新操作。状态不应为"clientupdateprohibited"。也可能由其他状态条件导致。 |

| 代码 | 详细 |
|------|------|
| 101  | 系统错误，详细信息 ({0})！ |
| 102  | 多个错误，详细信息 ({0})！ |
| 103  | 未知错误，详细信息 ({0})！ |
| 200  | 未找到 API 命令 ({0})！ |
| 210  | 未找到 API 服务 ({0})！ |
| 211  | 未设置 API 服务提供商！ |
| 300  | 未找到经销商！ |
| 301  | 您当前的 IP 地址未被授权访问。请确保您从已授权的 IP 地址连接并重试 |
| 310  | 不支持该 TLD！ |
| 320  | 未找到 API！ |
| 321  | 不支持该货币！ |
| 330  | 未设置必需参数 ({0})。 |
| –    | 请确保完整发送所有联系人对象 |
| 340  | 未找到价格定义 ({0}[{1}]-{2}{3})。 |
| 350  | 经销商余额不足。(经销商 Id : {0} - 当前余额 : {1} {2})。 |
| 350  | 会计货币不匹配或余额不足。 |
| 360  | 字段 ({0}) 的 API 请求无效。 |
| 360  | 已超出 API 配额！ |
| 361  | 限流错误！ |
| 362  | 该溢价域名目前无法注册。 |
| 363  | 由于域名处于自动续费期，操作已取消 |
| 364  | 由于注册局出现问题，此域名目前无法进行交易 |

| 代码 | 详细 |
|------|------|
| 400  | 无效联系人 ({0})。 |
| 401  | 联系信息无法同步。 |
| 402  | 无法从注册局访问联系信息。 |
| 403  | 系统缺少信息。请输入您的默认信息或联系支持团队。 |
| 404  | 域名联系人无法更新。需要注册局的许可。 |
| 410  | 未找到联系人。 |
| 410  | 未找到联系人 ({0})。 |
| 420  | 联系人 {0} 的 Api 命令无效。 |
| 430  | 未找到联系人 api。 |
| 440  | 联系人未同步。 |
| 450  | 域名联系人过多！ |
| 451  | 无法继续进行联系人更新。 |

| 代码 | 详细 |
|------|------|
| 500  | 无效的域名 id。 |
| 500  | 无效的域名 id ({0})。 |
| 501  | 域名无法同步({0}) |
| 502  | 内部转移失败 |
| 503  | 域名注册不可用。 |
| 504  | 域名信息不匹配。之前：{0} |
| 505  | 请输入 IP 地址。 |
| 506  | 域名转移已开始，但无法读取联系信息。 |
| 507  | 授权错误。 |
| 510  | 未找到域名。 |
| 511  | 无法找到已过期的域名。 |
| 512  | 域名不可续费。 |
| 513  | 域名不处于可更新状态。必须处于活动状态才能更新 |
| 514  | 应处于赎回期。 |
| 520  | 域名 "{0}" 的 Api 命令无效。 |
| 530  | 无效的域名期限。期限必须为 {0} 到 {1} 年。请求的期限为 {2} 年。 |
| 540  | 域名的有效期不能延长至自当前日期起 {0} 年以后。 |
| 550  | 无效的域名。域名长度必须为 {0} 到 {1} 个字符。请求的域名为 {2} 个字符。 |
| 560  | 无效的名称服务器数量。域名必须有 {0} 到 {1} 个名称服务器。请求的名称服务器数量为 {2}。 |
| 561  | 在传入请求中未找到名称服务器信息 |
| 570  | tld "{0}" 不支持 Idn。 |
| 571  | 期限无效。 |
| 572  | 命令无效。 |
| 573  | 未找到域名。 |
| 574  | 未找到 TLD 名称。 |
| 575  | 域名不处于可更新状态。必须处于活动状态才能更新名称服务器 |
| 576  | 域名只能在到期日前的最后 1 个月内续费 |
| 580  | tld "{0}" 不支持转移。 |
| 581  | 未找到子名称服务器 |
| 582  | 转移已由其他经销商发起。 |
| 583  | 转移未初始化。请联系支持团队。 |
| 584  | 对象状态禁止操作 |
| 590  | 此转移需要授权码。 |
| 591  | 授权码无效。 |
| 592  | 域名上存在转移锁定。 |
| 593  | 域名无法转移，状态信息和转移锁定必须符合所需标准。(状态信息: #ok) |
| 594  | 无法解析域名的名称服务器地址 ({0}) |
| 595  | 无法读取联系信息 (whois.registrar.tld)，请确保隐私保护状态已开启 |
| 596  | 联系信息无法验证 |
| 597  | 未找到 Tld |
| 598  | 发生未知错误 |
| 599  | 未找到域名转发 |

## 许可证

Apache License 2.0 — 参见 [LICENSE](LICENSE)。
