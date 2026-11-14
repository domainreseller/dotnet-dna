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

# DomainNameAPI .NET Library (dotnet-dna)

The official .NET client for the DomainNameAPI reseller REST API: domain search, registration, renewal, transfers, contacts, nameservers, child nameservers, DNS zones and URL forwarding.

dotnet-dna is a faithful port of [php-dna](https://github.com/domainreseller/php-dna): the same methods, the same requests on the wire and the same return values. It is checked against php-dna's conformance fixtures (134 scenarios), so code written against the PHP library translates one to one.

## 📦 Download — always use Releases!

⬇️ **Get the latest tested version here: https://github.com/domainreseller/dotnet-dna/releases/latest**

> ⚠️ Do **not** use the green **Code → Download ZIP** button — that downloads the raw development branch. Release packages are versioned, tested and production-ready.

## Installation and Integration Guide

### Minimum Requirements

- .NET 8 or later, **or** any runtime that supports .NET Standard 2.0 (.NET Framework 4.6.2+, .NET Core 2.0+, Mono, Xamarin, Unity).
- No third-party dependencies. The library uses `HttpClient` and `System.Text.Json` (on .NET Standard 2.0 the `System.Text.Json` package is pulled in automatically).

### Installation

**A) NuGet package** — the package id is `DomainNameApi`. Each release ships the `.nupkg` file; once the package is listed on NuGet.org you can also install it directly:

```bash
dotnet add package DomainNameApi
```

To install a downloaded `.nupkg`, put it in a folder and use that folder as a package source:

```bash
dotnet add package DomainNameApi --source ./packages
```

**B) Project reference** — download the release source and reference the project:

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

## 🔑 API Credentials — Reseller ID & API Key

The library talks to the DomainNameAPI REST API and needs exactly two credentials, passed to the constructor:

| Parameter | Value |
|---|---|
| First | Reseller ID — UUID like `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| Second | API Key |

> 💡 Find your **Reseller ID** and **API Key** in your DomainNameAPI panel under **API Settings**.
> ⚠️ These are **API credentials** — your panel login details will **not** work here.

```csharp
using DomainNameApi;

var resellerId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
var apiKey     = "your-api-key";

// Live environment
var dna = new DomainNameApiClient(resellerId, apiKey);

// Test (OTE) environment
var dna = new DomainNameApiClient(resellerId, apiKey, true);
```

Leading and trailing whitespace in both credentials is removed. The test-mode flag can also be passed as text, the way configuration files and panels store it: `"true"`, `"1"`, `"on"` and `"yes"` (any case) select the test environment, anything else selects live.

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey, Environment.GetEnvironmentVariable("DNA_TEST_MODE"));
```

## 🚀 Quick Start

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

### Return values

Every method returns the same structure php-dna returns, as a `System.Text.Json.Nodes.JsonNode` (usually a `JsonObject`). Read values with casts or `GetValue<T>()`, or print the whole result with `ToJsonString()`:

```csharp
var details = dna.GetDetails("example.com");
string status = (string)details["data"]["Status"];          // "Active"
int    days   = (int)details["data"]["Dates"]["RemainingDays"];
bool   locked = (string)details["data"]["LockStatus"] == "true";   // flags are the strings "true"/"false"
Console.WriteLine(details.ToJsonString());
```

Key names, string flags (`"true"`/`"false"`) and formatted money strings (`"11.9900"`) are exactly as in php-dna. Most methods use the `result` = `OK`/`ERROR` envelope; `GetCurrentBalance` uses the legacy `OperationResult` envelope and `CheckAvailability` returns a bare list on success.

Inputs such as contacts, TR attributes and list filters are `IDictionary<string, object>` with php-dna's keys (`FirstName`, `EMail`, `AddressLine1`, `PhoneCountryCode`, `TRABISCITIZENID`, `PageSize`, ...). Nameserver lists may be a list (`string[]`, `List<string>`) or a dictionary (the values are used).

### Usage

All examples below can also be run from the [examples](examples) folder (see [Examples](#examples)).

#### Domain Registration

Note: Additional parameters are required for .tr domains. The `additionalAttributes` parameter is used for domain names that require extra information like .tr.

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

Contacts are completed before they are sent: missing fields get placeholder values, digits and punctuation are removed from names, a missing or blank state falls back to the city, country names become ISO codes and phone numbers are split into number and country code — a number written with its own `+` or `00` country code keeps that code (see `ValidateContact`). When no nameserver (or only empty ones) is given, `ns1.domainnameapi.com` and `ns2.domainnameapi.com` are used. For .tr domains the TRABIS attributes are reduced to what the gateway accepts (`TRABISDOMAINCATEGORY` = `Owner`/`Corporate`, `TRABISCITIZENID`, `TRABISTAXOFFICE`, `TRABISTAXNUMBER`); the legacy key `TRABISCITIZIENID` and the numeric category are converted automatically.

<details>
<summary>Sample Output for Domain Registration</summary>

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

If the gateway refuses the order (it answers `"success": false`), the method returns an error envelope with `Code` = `REGISTER_DOMAIN` and the gateway's message, so a refused registration is never reported as done. `Renew` (`RENEW_DOMAIN`) and `Transfer` (`TRANSFER_DOMAIN`) behave the same way.

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domain Renewal

```csharp
var result = dna.Renew("example.com", 1);
```

<details>
<summary>Sample Output for Domain Renewal</summary>

```json
{ "result": "OK", "data": { "ExpirationDate": "2025-03-15T10:00:00" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domain Transfer

```csharp
var result = dna.Transfer("example.com", "Xy9#mK2$", 3);
```

> ℹ️ A transfer never sends contacts: the request always carries an empty contact list and the registrar takes the contacts over itself. `Transfer` therefore has no contacts parameter; update the contacts with `SaveContacts` after the transfer has completed if needed.

<details>
<summary>Sample Output for Domain Transfer</summary>

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

#### Cancel, Approve or Reject a Transfer

```csharp
dna.CancelTransfer("example.com");   // pending incoming transfer
dna.ApproveTransfer("example.com");  // pending outgoing transfer
dna.RejectTransfer("example.com");   // pending outgoing transfer
```

<details>
<summary>Sample Output for Cancel Transfer</summary>

```json
{ "result": "OK", "data": { "DomainName": "example.com", "Status": "Cancelled" } }
```

`ApproveTransfer` and `RejectTransfer` answer the same way with `Approved` / `Rejected`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Transfer Check

```csharp
var result = dna.CheckTransfer("example.com", "Ab1!xyZ9");
```

`result` is `OK` only when the registry reports the domain as transferable; otherwise it is `ERROR` with `Code` = `TRANSFER_NOT_AVAILABLE` and the details are still under `data`.

<details>
<summary>Sample Output for Transfer Check</summary>

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

#### Domain List

```csharp
var result = dna.GetList(new Dictionary<string, object>
{
    ["OrderColumn"]    = "Id",
    ["OrderDirection"] = "ASC",
    ["PageNumber"]     = 0,
    ["PageSize"]       = 1000,
});
```

`PageNumber` is zero-based. The gateway's own filter names (`MaxResultCount`, `SkipCount`, `Sorting`, `DomainName`, ...) can be passed as well.

<details>
<summary>Sample Output for Domain List</summary>

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

#### TLD List

```csharp
var result = dna.GetTldList(100);
```

<details>
<summary>Sample Output for TLD List</summary>

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

#### Domain Availability Check

```csharp
var result = dna.CheckAvailability(new[] { "hello", "world123x0" }, new[] { "com", "net" }, 1, "create");
```

<details>
<summary>Sample Output for Domain Availability Check</summary>

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

On failure the method returns the usual error envelope instead of a list, so check `result is JsonArray` first.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domain Details

```csharp
var result = dna.GetDetails("example.com");
var synced = dna.SyncFromRegistry("example.com"); // same envelope
```

<details>
<summary>Sample Output for Domain Details</summary>

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

#### Nameserver Modification

```csharp
var result = dna.ModifyNameServer("example.com", new[] { "ns1.example.com", "ns2.example.com" });
```

Host names are trimmed, lower-cased and stripped of a trailing dot; empty and duplicate entries are dropped.

<details>
<summary>Sample Output for Nameserver Modification</summary>

```json
{ "result": "OK", "data": { "NameServers": [ "ns1.example.com", "ns2.example.com" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Enable Domain Lock

```csharp
var result = dna.EnableTheftProtectionLock("example.com");
```

<details>
<summary>Sample Output for Enable Domain Lock</summary>

```json
{ "result": "OK", "data": { "LockStatus": true } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Disable Domain Lock

```csharp
var result = dna.DisableTheftProtectionLock("example.com");
```

<details>
<summary>Sample Output for Disable Domain Lock</summary>

```json
{ "result": "OK", "data": { "LockStatus": false } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Add Child Nameserver

```csharp
var result = dna.AddChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>Sample Output for Add Child Nameserver</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Delete Child Nameserver

```csharp
var result = dna.DeleteChildNameServer("example.com", "ns1.example.com");
```

<details>
<summary>Sample Output for Delete Child Nameserver</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Update Child Nameserver

```csharp
var result = dna.ModifyChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>Sample Output for Update Child Nameserver</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Modify Domain Privacy

```csharp
var result = dna.ModifyPrivacyProtectionStatus("example.com", true);

// The flag may also be text, as panels store it: "1", "true", "on" and "yes" enable privacy
var result2 = dna.ModifyPrivacyProtectionStatus("example.com", "on");
```

The optional third argument (`reason`) is kept for compatibility only; the gateway has no such field and it is not sent.

<details>
<summary>Sample Output for Modify Domain Privacy</summary>

```json
{ "data": { "PrivacyProtectionStatus": true }, "result": "OK" }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Get Domain Contacts

```csharp
var result = dna.GetContacts("example.com");
```

<details>
<summary>Sample Output for Get Domain Contacts</summary>

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

#### Save Domain Contacts

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

Contacts may also use the nested shape (`Address` → `Line1`/`City`/..., `Phone` → `Phone`/`Fax` → `Number`/`CountryCode`), the one `GetContacts` returns.

<details>
<summary>Sample Output for Save Domain Contacts</summary>

```json
{ "result": "OK", "data": { "contacts": [] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Get Reseller Details

```csharp
var result = dna.GetResellerDetails();
```

<details>
<summary>Sample Output for Get Reseller Details</summary>

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

#### Current Balance

```csharp
var result = dna.GetCurrentBalance("USD"); // USD, TRY, EUR or GBP
```

<details>
<summary>Sample Output for Current Balance</summary>

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

On failure `OperationResult` is `FAILED`, `ErrorCode` holds the error code (for example `API_429`) and `Balance` is `"0.00"`. The account reports USD and TRY balances only; asking for another currency (EUR, GBP, ...) fails with `ErrorCode` = `BALANCE` and the message `No EUR balance is reported for this account. Available: USD, TRY.`

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### DNS Zone Management

The gateway stores a zone as record **sets** (name + type → values) and replaces a whole set on every write. The `*ResourceRecordSet` and `*ResourceRecordValue` methods take care of that for you. Names may be relative (`www`), the apex (`@`, `""` or `example.com`) or fully qualified (`www.example.com.`). Supported types: `A`, `AAAA`, `ALIAS`, `CAA`, `CNAME`, `DS`, `MX`, `NAPTR`, `PTR`, `SRV`, `TLSA`, `TXT`. Writes are committed immediately unless you pass `apply: false` and call `ApplyResourceRecords` yourself; if that automatic apply fails, the method returns the apply error (the change stays staged, not live). Consecutive zone calls are spaced at least one second apart, because the gateway answers bursts with HTTP 429.

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
<summary>Sample Output for DNS Zone Management</summary>

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

`SetResourceRecordSet` / `AddResourceRecordValue` (the type is reported upper-cased and the TTL as sent, limited to 1–86400 seconds):

```json
{ "result": "OK", "data": { "Name": "", "Type": "MX", "TTL": 3600, "Contents": [ "10 mx1.example.com", "20 mx2.example.com" ] } }
```

Adding a value that is already present, or removing one that is absent, changes nothing and answers `{"result":"OK","data":{..., "Changed": false}}`. An unsupported type is refused before any call is made:

```json
{ "result": "ERROR", "error": { "Code": "ADD_ZONE", "Message": "Record type \"NS\" is not supported by the DNS zone API. Supported types: A, AAAA, ALIAS, CAA, CNAME, DS, MX, NAPTR, PTR, SRV, TLSA, TXT.", "Details": "..." } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### URL Forwarding

```csharp
dna.SetForwarding("example.com", "https://www.example.org", "Standard"); // or "Frame" (masked)
var forwarding = dna.GetForwarding("example.com");
dna.DeleteForwarding("example.com");
```

<details>
<summary>Sample Output for URL Forwarding</summary>

```json
{ "result": "OK", "data": { "Enabled": true, "RedirectAddress": "https://www.example.org/landing", "ForwardType": "Frame" } }
```

When no forwarding is configured (the gateway answers `Redirect information not found`), `GetForwarding` returns `{"result":"OK","data":{"Enabled":false,"RedirectAddress":"","ForwardType":""}}`. Any other error — for example a domain that does not exist — is returned as an error.

`DeleteForwarding` returns `{"result":"OK","data":{"DomainName":"example.com","Removed":true}}`; when nothing was configured it is a no-op that answers `"Removed": false`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Utilities (no network)

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
<summary>Sample Output for ValidateContact</summary>

```json
{
  "FirstName": "Jo", "Phone": "12025550100", "Country": "TR", "LastName": "Isimyok",
  "AddressLine1": "Addres yok", "City": "ISTANBUL", "State": "ISTANBUL", "ZipCode": "34000",
  "PhoneCountryCode": "90", "Fax": "", "FaxCountryCode": ""
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

### Error Handling

Methods never throw for API or network problems; they return an error envelope:

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

| Situation | `Code` | `Message` |
|---|---|---|
| HTTP error from the gateway | `API_<status>`, e.g. `API_400`, `API_429`, `API_500` | the gateway's message; for an empty body `HTTP 429 - Too Many Requests` |
| Wrong credentials (HTTP 301/302) | `API_302` | `Invalid API credentials` |
| Validation errors | `API_400` | the gateway's message; `Details` lists the fields, e.g. `Admin PostalCode - 'PostalCode' is required` |
| Timeout | `CURL_28` | `Could not reach the DomainNameAPI service: the connection timed out. (...)` |
| Other network failures (DNS, connect, TLS, reset) | `CURL_6`, `CURL_7`, `CURL_60`, `CURL_56` | `Curl error during request: ...` |
| Registration, renewal or transfer refused (`"success": false`) | `REGISTER_DOMAIN`, `RENEW_DOMAIN`, `TRANSFER_DOMAIN` | the gateway's message |
| Empty or unusable success response | `RESPONSE` | `Invalid response received from server! Response is empty.` |
| Checks done before calling the gateway | e.g. `AVAILABILITY`, `ADD_ZONE`, `SET_FORWARD` | a description of the problem |

`API_<n>` codes always mean an HTTP status. When the gateway sent its own error code, the envelope also carries it as `GatewayCode` (for example `Dna.DomainService:Domain:10007`), so you can tell "domain not found" from "TLD not supported" without parsing the message. Network failures use `CURL_<n>`, where the number follows libcurl's numbering so that the codes match php-dna. Only programming errors (for example passing an unsupported object type) can throw.

For troubleshooting, the last call can be inspected:

```csharp
dna.GetLastFunction();       // "request:GET domains/info"
dna.GetRequestData();        // { "url": ..., "method": ..., "payload": ... }
dna.GetResponseData();       // decoded response, or { "raw_response": "..." }
dna.GetResponseHeaders();    // { "url", "http_code", "content_type", "total_time", "headers" }
dna.GetParsedResponseData(); // parsed response or the error built from it
```

### Test Mode

Pass `true` as the third constructor argument to use the OTE test environment (`https://ote.domainresellerapi.com/api/v1`) instead of live (`https://api.domainresellerapi.com/api/v1`). `GetServiceUrl()` shows the address in use and `SetServiceUrl(url)` overrides it.

### Error reporting (telemetry)

When an API call fails, the library can send an error report to DomainNameAPI's own error tracker (Sentry). A small random share of successful calls (4%, plus every call slower than 1 second) can be sent as a performance record. This helps DomainNameAPI find and fix problems. Error reporting is **on by default**.

**What is sent:**

- error code and message
- called endpoint path and HTTP method
- call duration
- application name (`DomainNameAPI-DotNet`), library version and .NET version
- reseller ID and domain name
- the server's public IP address (looked up via `api.ipify.org`, in the background)
- the operating-system account name the application runs under
- the API request and response, including the domain's contact details; only credentials are masked — EPP/auth codes, API keys, passwords and tokens are replaced with `***`

**What is never sent:** EPP/auth codes, API keys, passwords or tokens in readable form; method arguments in stack traces.

**Why and how long:** the data is used only for usage measurement and early diagnosis of problems, and is kept for 30 days on DomainNameAPI's own Sentry server, after which it is deleted automatically.

Reports are sent in the background: they never block or slow down your call and never throw. Expected, high-volume errors (domain not found, insufficient balance, invalid credentials, no forwarding configured, ...) are not reported.

**How to turn it off:** call `SetErrorReportingEnabled(false)` after constructing the client:

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey);
dna.SetErrorReportingEnabled(false);
```

**Minimal mode:** `SetTelemetryMinimal(true)` keeps reporting on but sends only the error code and message, endpoint, timing, versions and reseller ID — no request/response data, domain, IP address or account name.

The destination can be changed with the `TelemetryDsn` property (for example to your own Sentry project).

> 🔒 **Security:** TLS certificate verification is on. On a host with an outdated CA bundle it can be switched off with `dna.SetSslVerification(false);` — this is not recommended; updating the CA bundle is the better fix.

### Advanced

- **Timeouts:** 30 seconds per request and 10 seconds to connect; change them with the `Timeout` and `ConnectTimeout` properties.
- **Custom transport:** pass an `IHttpTransport` to the constructor (`new DomainNameApiClient(id, key, false, transport)`) or set the `Transport` property to route requests through your own HTTP stack, a proxy or a test double. Implementations return every HTTP status (redirects are not followed) and throw `TransportException` for network failures.
- **Threads:** a client keeps the last request and response for diagnostics and is not thread-safe. Use one instance per thread (or per request) or synchronise access. `Dispose()` releases the underlying `HttpClient`.
- **User-Agent:** requests are sent as `DomainNameAPI-DotNet/1.0.1`.

### Examples

The [examples](examples/DomainNameApi.Examples) project contains one runnable file per method (`GetDetails`, `RegisterWithContactInfo`, `RegisterWithContactInfo-TR`, the DNS zone and forwarding methods, ...). Set your credentials and run one:

```bash
export DNA_RESELLER_ID=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
export DNA_API_KEY=your-api-key
export DNA_TEST_MODE=true   # optional: use the OTE test environment

dotnet run --project examples/DomainNameApi.Examples -- GetDetails
dotnet run --project examples/DomainNameApi.Examples            # lists all examples
```

### Testing

The tests need no credentials and make no external network calls:

```bash
dotnet test
```

Without a local .NET SDK, run them in Docker:

```bash
docker run --rm -v "$PWD":/src -w /src mcr.microsoft.com/dotnet/sdk:8.0 dotnet test
```

| Test file | Description |
|-----------|-------------|
| `tests/DomainNameApi.Tests/ConformanceTests.cs` | Replays all 134 php-dna conformance fixtures (`testdata/conformance/`) and compares requests and results |
| `tests/DomainNameApi.Tests/HttpLayerTests.cs` | Headers, URLs, query strings, test mode, timeouts, zone pacing |
| `tests/DomainNameApi.Tests/HttpClientTransportTests.cs` | The default `HttpClient` transport against a local listener |
| `tests/DomainNameApi.Tests/ErrorMappingTests.cs` | Gateway error mapping and validation messages |
| `tests/DomainNameApi.Tests/TelemetryTests.cs` | Telemetry payloads, credential masking, minimal mode |
| `tests/DomainNameApi.Tests/UtilityTests.cs` | Contact validation, phone/country normalisation, PHP-compatible formatting |

<hr style="border: 4px solid #000; border-style: dashed;">

## Return and Error Codes with Descriptions

| Code | Description                                    | Detail                                                                                                                                                         |
|------|------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1000 | Command completed successfully                 | Operation successful.                                                                                                                                          |
| 1001 | Command completed successfully; action pending | Operation successful. However, the operation is currently queued for completion.                                                                               |
| 2003 | Required parameter missing                     | Parameter missing error. For example, no phone entry in contact information.                                                                                   |
| 2105 | Object is not eligible for renewal             | Domain status is not suitable for renewal, locked for update operations. Status should not be "clientupdateprohibited". May be due to other status conditions. |
| 2200 | Authentication error                           | Authorization error, security code is incorrect or domain is registered with another registrar.                                                                |
| 2302 | Object exists                                  | Domain name or nameserver information already exists in database. Cannot be registered.                                                                        |
| 2303 | Object does not exist                          | Domain name or nameserver information does not exist in database. New registration required.                                                                   |
| 2304 | Object status prohibits operation              | Domain status is not suitable for updates, locked for update operations. Status should not be "clientupdateprohibited". May be due to other status conditions. |

| CODE | DETAIL                                                                                                                               |
|------|--------------------------------------------------------------------------------------------------------------------------------------|
| 101  | System Error Detailed For ({0})!                                                                                                     |
| 102  | Multiple Errors Detailed For ({0})!                                                                                                  |
| 103  | Unknown Errors Detailed For ({0})!                                                                                                   |
| 200  | API command not found ({0})!                                                                                                         |
| 210  | API service not found ({0})!                                                                                                         |
| 211  | API service provider not set!                                                                                                        |
| 300  | Reseller not found!                                                                                                                  |
| 301  | Your current IP address is not authorized to access. Please make sure you are connecting from an authorized IP address and try again |
| 310  | TLD is not supported!                                                                                                                |
| 320  | API not found!                                                                                                                       |
| 321  | Currency is not supported!                                                                                                           |
| 330  | Required parameter(s) not set ({0}).                                                                                                 |
| –    | Make sure you send all contact objects full                                                                                          |
| 340  | Price definition not found ({0}[{1}]-{2}{3}).                                                                                        |
| 350  | Insufficient reseller balance. (Reseller Id : {0} - Current Balance : {1} {2}).                                                      |
| 350  | Accounting currency does not match or the balance is not sufficient.                                                                 |
| 360  | Invalid API request for field ({0}).                                                                                                 |
| 360  | API quota exceeded!                                                                                                                  |
| 361  | Throttled error!                                                                                                                     |
| 362  | Premium domain is not available to register right now.                                                                               |
| 363  | Operation cancelled because domain in auto-renewal period                                                                            |
| 364  | This domain is currently unavailable for transaction due to a problem with the registry                                              |

| CODE | DETAIL                                                                                      |
|------|---------------------------------------------------------------------------------------------|
| 400  | Invalid contact ({0}).                                                                      |
| 401  | Contact info can not sync.                                                                  |
| 402  | No access to contact information from registry.                                             |
| 403  | THE SYSTEM HAS MISSING INFORMATION. ENTER YOUR DEFAULT INFORMATION OR CONTACT SUPPORT TEAM. |
| 404  | DOMAIN CONTACT CANNOT UPDATE. PERMISSION FROM THE REGISTRY.                                 |
| 410  | Contact not found.                                                                          |
| 410  | Contact not found ({0}).                                                                    |
| 420  | Invalid Api command for contact {0}.                                                        |
| 430  | Contact api not found.                                                                      |
| 440  | Contact is not synced.                                                                      |
| 450  | Too many domain contacts!                                                                   |
| 451  | Failed to proceed with contact update.                                                      |

| CODE | DETAIL                                                                                                                               |
|------|--------------------------------------------------------------------------------------------------------------------------------------|
| 500  | Invalid domain id.                                                                                                                   |
| 500  | Invalid domain id ({0}).                                                                                                             |
| 501  | Domain could not synchronized({0})                                                                                                   |
| 502  | Internal transfer failed                                                                                                             |
| 503  | Domain registration is not available.                                                                                                |
| 504  | Domain information does not match. Before :{0}                                                                                       |
| 505  | Enter the Ip Address.                                                                                                                |
| 506  | Domain Transfer Has Been Started But Contact Info Not Read.                                                                          |
| 507  | Authorization error.                                                                                                                 |
| 510  | Domain not found.                                                                                                                    |
| 511  | Expired domains cannot be found.                                                                                                     |
| 512  | Domain is not renewable.                                                                                                             |
| 513  | Domain is not in updateable status. It must be active for to be updated                                                              |
| 514  | Redemption Period Expected.                                                                                                          |
| 520  | Invalid Api command for domain "{0}".                                                                                                |
| 530  | Invalid domain period. Period must be {0} to {1} years. Requested period is {2} years.                                               |
| 540  | Domain cannot be extended beyond {0} years from current date.                                                                        |
| 550  | Invalid domain name. Domain must be {0} to {1} characters length. Requested domain name is {2} characters.                           |
| 560  | Invalid name server count. Domain must have {0} to {1} name servers. Requested name server count is {2}.                             |
| 561  | No name server information found in the incoming request                                                                             |
| 570  | Idn not supported for tld "{0}".                                                                                                     |
| 571  | Period invalid.                                                                                                                      |
| 572  | Command invalid.                                                                                                                     |
| 573  | Domain names not found.                                                                                                              |
| 574  | TLD names not found.                                                                                                                 |
| 575  | Domain is not in updateable status. It must be active for name servers to be updated                                                 |
| 576  | Domain can be renewed in the last 1 month before the expire date                                                                     |
| 580  | Transfer not supported for tld "{0}".                                                                                                |
| 581  | Child name server not found                                                                                                          |
| 582  | Transfer started by other reseller.                                                                                                  |
| 583  | Transfer not initialized. Please contact the support team.                                                                           |
| 584  | Object status prohibits operation                                                                                                    |
| 590  | Auth code is required for this transfer.                                                                                             |
| 591  | Auth code is not valid.                                                                                                              |
| 592  | Transfer lock exists on domain.                                                                                                      |
| 593  | Domain cannot be transferred, Status Information and Transfer Lock must comply with the required criteria. (Status Information: #ok) |
| 594  | Domain name server address could not resolved ({0})                                                                                  |
| 595  | Contact information can not be read (whois.registrar.tld) Please make sure that privacy protection status open                       |
| 596  | Contact information could not verified                                                                                               |
| 597  | Tld Not Found                                                                                                                        |
| 598  | Unknown error occurred                                                                                                               |
| 599  | Domain Forward Not Found                                                                                                             |

## License

Apache License 2.0 — see [LICENSE](LICENSE).
