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

# DomainNameAPI .NET-bibliotheek (dotnet-dna)

De officiële .NET-client voor de reseller-REST-API van DomainNameAPI: domeinen zoeken, registreren, verlengen, transfers, contacten, nameservers, child nameservers, DNS-zones en URL-doorsturing.

dotnet-dna is een getrouwe port van [php-dna](https://github.com/domainreseller/php-dna): dezelfde methoden, dezelfde verzoeken over de lijn en dezelfde retourwaarden. De bibliotheek wordt getoetst aan de conformiteitsfixtures van php-dna (126 scenario's), zodat code die voor de PHP-bibliotheek is geschreven één op één over te zetten is.

## 📦 Download — gebruik altijd de Releases!

⬇️ **Download hier de nieuwste geteste versie: https://github.com/domainreseller/dotnet-dna/releases/latest**

> ⚠️ Gebruik **niet** de groene knop **Code → Download ZIP** — daarmee downloadt u de ruwe ontwikkelbranch. Releasepakketten zijn voorzien van een versienummer, getest en klaar voor productie.

## Installatie- en integratiehandleiding

### Minimale vereisten

- .NET 8 of hoger, **of** elke runtime die .NET Standard 2.0 ondersteunt (.NET Framework 4.6.2+, .NET Core 2.0+, Mono, Xamarin, Unity).
- Geen afhankelijkheden van derden. De bibliotheek gebruikt `HttpClient` en `System.Text.Json` (onder .NET Standard 2.0 wordt het pakket `System.Text.Json` automatisch meegenomen).

### Installatie

**A) NuGet-pakket** — de pakket-id is `DomainNameApi`. Elke release bevat het `.nupkg`-bestand; zodra het pakket op NuGet.org staat, kunt u het ook rechtstreeks installeren:

```bash
dotnet add package DomainNameApi
```

Om een gedownload `.nupkg`-bestand te installeren, plaatst u het in een map en gebruikt u die map als pakketbron:

```bash
dotnet add package DomainNameApi --source ./packages
```

**B) Projectverwijzing** — download de broncode van de release en verwijs naar het project:

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

## 🔑 API-gegevens — Reseller ID & API-sleutel

De bibliotheek communiceert met de REST-API van DomainNameAPI en heeft precies twee inloggegevens nodig, die aan de constructor worden meegegeven:

| Parameter | Waarde |
|---|---|
| Eerste | Reseller ID — UUID zoals `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| Tweede | API-sleutel |

> 💡 Uw **Reseller ID** en **API-sleutel** vindt u in uw DomainNameAPI-paneel onder **API-instellingen**.
> ⚠️ Dit zijn **API-gegevens** — uw inloggegevens voor het paneel werken hier **niet**.

```csharp
using DomainNameApi;

var resellerId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
var apiKey     = "your-api-key";

// Live environment
var dna = new DomainNameApiClient(resellerId, apiKey);

// Test (OTE) environment
var dna = new DomainNameApiClient(resellerId, apiKey, true);
```

Spaties aan het begin en einde van beide gegevens worden verwijderd. De testmodusvlag kan ook als tekst worden meegegeven, zoals configuratiebestanden en panelen die opslaan: `"true"`, `"1"`, `"on"` en `"yes"` (hoofdletterongevoelig) kiezen de testomgeving, elke andere waarde kiest de live-omgeving.

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey, Environment.GetEnvironmentVariable("DNA_TEST_MODE"));
```

## 🚀 Snel aan de slag

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

### Retourwaarden

Elke methode geeft dezelfde structuur terug als php-dna, in de vorm van een `System.Text.Json.Nodes.JsonNode` (meestal een `JsonObject`). Lees waarden uit met casts of `GetValue<T>()`, of print het volledige resultaat met `ToJsonString()`:

```csharp
var details = dna.GetDetails("example.com");
string status = (string)details["data"]["Status"];          // "Active"
int    days   = (int)details["data"]["Dates"]["RemainingDays"];
bool   locked = (string)details["data"]["LockStatus"] == "true";   // flags are the strings "true"/"false"
Console.WriteLine(details.ToJsonString());
```

Sleutelnamen, stringvlaggen (`"true"`/`"false"`) en geformatteerde geldbedragen als strings (`"11.9900"`) zijn exact gelijk aan php-dna. De meeste methoden gebruiken de envelop `result` = `OK`/`ERROR`; `GetCurrentBalance` gebruikt de oudere `OperationResult`-envelop en `CheckAvailability` geeft bij succes een kale lijst terug.

Invoer zoals contacten, TR-attributen en lijstfilters zijn `IDictionary<string, object>` met de sleutels van php-dna (`FirstName`, `EMail`, `AddressLine1`, `PhoneCountryCode`, `TRABISCITIZENID`, `PageSize`, ...). Nameserverlijsten kunnen een lijst (`string[]`, `List<string>`) of een dictionary (de waarden worden gebruikt) zijn.

### Gebruik

Alle onderstaande voorbeelden kunnen ook vanuit de map [examples](examples) worden uitgevoerd (zie [Voorbeelden](#voorbeelden)).

#### Domeinregistratie

Let op: voor .tr-domeinen zijn extra parameters vereist. De parameter `additionalAttributes` wordt gebruikt voor domeinnamen die extra informatie vereisen, zoals .tr.

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

Contacten worden aangevuld voordat ze worden verzonden: ontbrekende velden krijgen plaatsaanduidingswaarden, cijfers en leestekens worden uit namen verwijderd, een lege provincie/staat wordt vervangen door de plaats, landnamen worden omgezet naar ISO-codes en telefoonnummers worden opgesplitst in nummer en landcode (zie `ValidateContact`). Wanneer er geen nameserver (of alleen lege) wordt opgegeven, worden `ns1.domainnameapi.com` en `ns2.domainnameapi.com` gebruikt. Voor .tr-domeinen worden de TRABIS-attributen teruggebracht tot wat de gateway accepteert (`TRABISDOMAINCATEGORY` = `Owner`/`Corporate`, `TRABISCITIZENID`, `TRABISTAXOFFICE`, `TRABISTAXNUMBER`); de verouderde sleutel `TRABISCITIZIENID` en de numerieke categorie worden automatisch omgezet.

<details>
<summary>Voorbeelduitvoer voor domeinregistratie</summary>

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

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domeinverlenging

```csharp
var result = dna.Renew("example.com", 1);
```

<details>
<summary>Voorbeelduitvoer voor domeinverlenging</summary>

```json
{ "result": "OK", "data": { "ExpirationDate": "2025-03-15T10:00:00" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domeintransfer

```csharp
var result = dna.Transfer("example.com", "Xy9#mK2$", 3);
```

> ℹ️ Net als de publieke facade van php-dna verstuurt `Transfer` een lege contactenlijst; de optionele parameter `contacts` bestaat alleen om de signatuur gelijk te houden.

<details>
<summary>Voorbeelduitvoer voor domeintransfer</summary>

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

#### Een transfer annuleren, goedkeuren of weigeren

```csharp
dna.CancelTransfer("example.com");   // pending incoming transfer
dna.ApproveTransfer("example.com");  // pending outgoing transfer
dna.RejectTransfer("example.com");   // pending outgoing transfer
```

<details>
<summary>Voorbeelduitvoer voor het annuleren van een transfer</summary>

```json
{ "result": "OK", "data": { "DomainName": "example.com", "Status": "Cancelled" } }
```

`ApproveTransfer` en `RejectTransfer` antwoorden op dezelfde manier met `Approved` / `Rejected`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Transfercontrole

```csharp
var result = dna.CheckTransfer("example.com", "Ab1!xyZ9");
```

`result` is alleen `OK` wanneer het register het domein als overdraagbaar meldt; anders is het `ERROR` met `Code` = `TRANSFER_NOT_AVAILABLE` en staan de details nog steeds onder `data`.

<details>
<summary>Voorbeelduitvoer voor transfercontrole</summary>

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

#### Domeinlijst

```csharp
var result = dna.GetList(new Dictionary<string, object>
{
    ["OrderColumn"]    = "Id",
    ["OrderDirection"] = "ASC",
    ["PageNumber"]     = 0,
    ["PageSize"]       = 1000,
});
```

`PageNumber` begint bij nul. De eigen filternamen van de gateway (`MaxResultCount`, `SkipCount`, `Sorting`, `DomainName`, ...) kunnen ook worden meegegeven.

<details>
<summary>Voorbeelduitvoer voor domeinlijst</summary>

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

#### TLD-lijst

```csharp
var result = dna.GetTldList(100);
```

<details>
<summary>Voorbeelduitvoer voor TLD-lijst</summary>

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

#### Beschikbaarheidscontrole van domeinen

```csharp
var result = dna.CheckAvailability(new[] { "hello", "world123x0" }, new[] { "com", "net" }, 1, "create");
```

<details>
<summary>Voorbeelduitvoer voor beschikbaarheidscontrole van domeinen</summary>

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

Bij een fout geeft de methode in plaats van een lijst de gebruikelijke foutenvelop terug; controleer dus eerst `result is JsonArray`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domeindetails

```csharp
var result = dna.GetDetails("example.com");
var synced = dna.SyncFromRegistry("example.com"); // same envelope
```

<details>
<summary>Voorbeelduitvoer voor domeindetails</summary>

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

#### Nameservers wijzigen

```csharp
var result = dna.ModifyNameServer("example.com", new[] { "ns1.example.com", "ns2.example.com" });
```

Hostnamen worden ontdaan van spaties, naar kleine letters omgezet en van een afsluitende punt ontdaan; lege en dubbele items worden weggelaten.

<details>
<summary>Voorbeelduitvoer voor het wijzigen van nameservers</summary>

```json
{ "result": "OK", "data": { "NameServers": [ "ns1.example.com", "ns2.example.com" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domeinvergrendeling inschakelen

```csharp
var result = dna.EnableTheftProtectionLock("example.com");
```

<details>
<summary>Voorbeelduitvoer voor het inschakelen van domeinvergrendeling</summary>

```json
{ "result": "OK", "data": { "LockStatus": true } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domeinvergrendeling uitschakelen

```csharp
var result = dna.DisableTheftProtectionLock("example.com");
```

<details>
<summary>Voorbeelduitvoer voor het uitschakelen van domeinvergrendeling</summary>

```json
{ "result": "OK", "data": { "LockStatus": false } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Child nameserver toevoegen

```csharp
var result = dna.AddChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>Voorbeelduitvoer voor het toevoegen van een child nameserver</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Child nameserver verwijderen

```csharp
var result = dna.DeleteChildNameServer("example.com", "ns1.example.com");
```

<details>
<summary>Voorbeelduitvoer voor het verwijderen van een child nameserver</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Child nameserver bijwerken

```csharp
var result = dna.ModifyChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>Voorbeelduitvoer voor het bijwerken van een child nameserver</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domeinprivacy wijzigen

```csharp
var result = dna.ModifyPrivacyProtectionStatus("example.com", true, "Owner request");
```

<details>
<summary>Voorbeelduitvoer voor het wijzigen van domeinprivacy</summary>

```json
{ "data": { "PrivacyProtectionStatus": true }, "result": "OK" }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domeincontacten ophalen

```csharp
var result = dna.GetContacts("example.com");
```

<details>
<summary>Voorbeelduitvoer voor het ophalen van domeincontacten</summary>

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

#### Domeincontacten opslaan

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

Contacten kunnen ook in de geneste vorm worden meegegeven (`Address` → `Line1`/`City`/..., `Phone` → `Phone`/`Fax` → `Number`/`CountryCode`), de vorm die `GetContacts` teruggeeft.

<details>
<summary>Voorbeelduitvoer voor het opslaan van domeincontacten</summary>

```json
{ "result": "OK", "data": { "contacts": [] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Resellerdetails ophalen

```csharp
var result = dna.GetResellerDetails();
```

<details>
<summary>Voorbeelduitvoer voor het ophalen van resellerdetails</summary>

```json
{
  "result": "OK",
  "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "active": true,
  "name": "Example Reseller",
  "balance": "500",
  "currency": "USD",
  "symbol": "$",
  "balances": [
    { "balance": "500", "currency": "USD", "symbol": "$" },
    { "balance": "1500", "currency": "TL", "symbol": "TL" }
  ]
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Huidig saldo

```csharp
var result = dna.GetCurrentBalance("USD"); // USD, TRY, EUR or GBP
```

<details>
<summary>Voorbeelduitvoer voor huidig saldo</summary>

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

Bij een fout is `OperationResult` gelijk aan `FAILED`, bevat `ErrorCode` de foutcode (bijvoorbeeld `API_429`) en is `Balance` gelijk aan `"0.00"`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### DNS-zonebeheer

De gateway slaat een zone op als record-**sets** (naam + type → waarden) en vervangt bij elke schrijfactie een volledige set. De methoden `*ResourceRecordSet` en `*ResourceRecordValue` regelen dat voor u. Namen kunnen relatief zijn (`www`), de apex (`@`, `""` of `example.com`) of volledig gekwalificeerd (`www.example.com.`). Ondersteunde typen: `A`, `AAAA`, `ALIAS`, `CAA`, `CNAME`, `DS`, `MX`, `NAPTR`, `PTR`, `SRV`, `TLSA`, `TXT`. Schrijfacties worden direct doorgevoerd, tenzij u `apply: false` meegeeft en zelf `ApplyResourceRecords` aanroept. Opeenvolgende zone-aanroepen worden minstens één seconde uit elkaar gehouden, omdat de gateway op snelle reeksen aanroepen antwoordt met HTTP 429.

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
<summary>Voorbeelduitvoer voor DNS-zonebeheer</summary>

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

`SetResourceRecordSet` / `AddResourceRecordValue`:

```json
{ "result": "OK", "data": { "Name": "", "Type": "MX", "TTL": 3600, "Contents": [ "10 mx1.example.com", "20 mx2.example.com" ] } }
```

Het toevoegen van een waarde die al aanwezig is, of het verwijderen van een waarde die ontbreekt, verandert niets en geeft `{"result":"OK","data":{..., "Changed": false}}` terug. Een niet-ondersteund type wordt geweigerd voordat er een aanroep wordt gedaan:

```json
{ "result": "ERROR", "error": { "Code": "ADD_ZONE", "Message": "Record type \"NS\" is not supported by the DNS zone API. Supported types: A, AAAA, ALIAS, CAA, CNAME, DS, MX, NAPTR, PTR, SRV, TLSA, TXT.", "Details": "..." } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### URL-doorsturing

```csharp
dna.SetForwarding("example.com", "https://www.example.org", "Standard"); // or "Frame" (masked)
var forwarding = dna.GetForwarding("example.com");
dna.DeleteForwarding("example.com");
```

<details>
<summary>Voorbeelduitvoer voor URL-doorsturing</summary>

```json
{ "result": "OK", "data": { "Enabled": true, "RedirectAddress": "https://www.example.org/landing", "ForwardType": "Frame" } }
```

Als er geen doorsturing is ingesteld, geeft `GetForwarding` het volgende terug: `{"result":"OK","data":{"Enabled":false,"RedirectAddress":"","ForwardType":""}}`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Hulpfuncties (zonder netwerk)

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
<summary>Voorbeelduitvoer voor ValidateContact</summary>

```json
{
  "FirstName": "Jo", "Phone": "12025550100", "Country": "TR", "LastName": "Isimyok",
  "AddressLine1": "Addres yok", "City": "ISTANBUL", "State": "ISTANBUL", "ZipCode": "34000",
  "PhoneCountryCode": "90", "Fax": "", "FaxCountryCode": ""
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

### Foutafhandeling

Bij API- of netwerkproblemen gooien de methoden nooit een exceptie; in plaats daarvan geven ze een foutenvelop terug:

```json
{
  "result": "ERROR",
  "error": {
    "Code": "API_404",
    "Message": "The domain name you requested could not be found. Please check and try again.",
    "Details": "The domain could not be found. Please check and try again."
  }
}
```

| Situatie | `Code` | `Message` |
|---|---|---|
| HTTP-fout van de gateway | `API_<status>`, bijv. `API_400`, `API_429`, `API_500` | de melding van de gateway; bij een lege body `HTTP 429 - Too Many Requests` |
| Onjuiste inloggegevens (HTTP 301/302) | `API_302` | `Invalid API credentials` |
| Validatiefouten | `API_400` | de melding van de gateway; `Details` somt de velden op, bijv. `Admin PostalCode - 'PostalCode' is required` |
| Time-out | `API_28` | `Could not reach the DomainNameAPI service: the connection timed out. (...)` |
| Overige netwerkfouten (DNS, verbinden, TLS, verbroken verbinding) | `API_6`, `API_7`, `API_60`, `API_56` | `Curl error during request: ...` |
| Leeg of onbruikbaar succesantwoord | `RESPONSE` | `Invalid response received from server! Response is empty.` |
| Controles vóór de aanroep van de gateway | bijv. `AVAILABILITY`, `ADD_ZONE`, `SET_FORWARD` | een beschrijving van het probleem |

De numerieke netwerkcodes volgen de nummering van libcurl, zodat ze overeenkomen met php-dna. Alleen programmeerfouten (bijvoorbeeld het meegeven van een niet-ondersteund objecttype) kunnen een exceptie veroorzaken.

Voor het oplossen van problemen kan de laatste aanroep worden bekeken:

```csharp
dna.GetLastFunction();       // "request:GET domains/info"
dna.GetRequestData();        // { "url": ..., "method": ..., "payload": ... }
dna.GetResponseData();       // decoded response, or { "raw_response": "..." }
dna.GetResponseHeaders();    // { "url", "http_code", "content_type", "total_time", "headers" }
dna.GetParsedResponseData(); // parsed response or the error built from it
```

### Testmodus

Geef `true` mee als derde constructorargument om de OTE-testomgeving (`https://ote.domainresellerapi.com/api/v1`) te gebruiken in plaats van de live-omgeving (`https://api.domainresellerapi.com/api/v1`). `GetServiceUrl()` toont het gebruikte adres en `SetServiceUrl(url)` overschrijft het.

### Foutrapportage (telemetrie)

Wanneer een API-aanroep mislukt, kan de bibliotheek een foutrapport sturen naar de eigen fouttracker van DomainNameAPI (Sentry). Een klein, willekeurig gekozen deel van de geslaagde aanroepen (4%, plus elke aanroep die langer dan 1 seconde duurt) kan als prestatierecord worden verzonden. Dit helpt DomainNameAPI problemen op te sporen en op te lossen. Foutrapportage staat **standaard aan**.

> ℹ️ In versie 1.0.0 is het Sentry-adres (`DefaultSentryDsn`) nog leeg, omdat het Sentry-project voor de .NET-bibliotheek nog niet is aangemaakt. Totdat een release het invult, wordt er **niets verzonden**. Het onderstaande gedrag beschrijft wat er wordt verzonden zodra het is ingesteld.

**Wat er wordt verzonden:**

- foutcode en -melding
- pad van het aangeroepen endpoint en de HTTP-methode
- duur van de aanroep
- applicatienaam (`DomainNameAPI-DotNet`), bibliotheekversie en .NET-versie
- reseller ID en domeinnaam
- het publieke IP-adres van de server (op de achtergrond opgevraagd via `api.ipify.org`)
- de naam van het besturingssysteemaccount waaronder de applicatie draait
- het API-verzoek en -antwoord, inclusief de contactgegevens van het domein; alleen inloggegevens worden gemaskeerd — EPP-/auth-codes, API-sleutels, wachtwoorden en tokens worden vervangen door `***`

**Wat nooit wordt verzonden:** EPP-/auth-codes, API-sleutels, wachtwoorden of tokens in leesbare vorm; methodeargumenten in stacktraces.

**Waarom en hoe lang:** de gegevens worden uitsluitend gebruikt voor gebruiksmeting en het vroegtijdig opsporen van problemen, en worden 30 dagen bewaard op de eigen Sentry-server van DomainNameAPI, waarna ze automatisch worden verwijderd.

Rapporten worden op de achtergrond verzonden: ze blokkeren of vertragen uw aanroep nooit en veroorzaken nooit een exceptie. Verwachte fouten die vaak voorkomen (domein niet gevonden, onvoldoende saldo, ongeldige inloggegevens, ...) worden niet gerapporteerd.

**Zo schakelt u het uit:** roep na het aanmaken van de client `SetErrorReportingEnabled(false)` aan:

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey);
dna.SetErrorReportingEnabled(false);
```

**Minimale modus:** `SetTelemetryMinimal(true)` laat de rapportage aan, maar verzendt alleen de foutcode en -melding, het endpoint, de tijdsduur, de versies en de reseller ID — geen verzoek-/antwoordgegevens, domein, IP-adres of accountnaam.

De bestemming kan worden gewijzigd met de eigenschap `TelemetryDsn` (bijvoorbeeld naar uw eigen Sentry-project).

> 🔒 **Beveiliging:** TLS-certificaatverificatie staat aan. Op een host met een verouderde CA-bundel kan deze worden uitgeschakeld met `dna.SetSslVerification(false);` — dit wordt niet aanbevolen; de CA-bundel bijwerken is de betere oplossing.

### Geavanceerd

- **Time-outs:** 30 seconden per verzoek en 10 seconden om verbinding te maken; wijzig ze met de eigenschappen `Timeout` en `ConnectTimeout`.
- **Eigen transport:** geef een `IHttpTransport` mee aan de constructor (`new DomainNameApiClient(id, key, false, transport)`) of stel de eigenschap `Transport` in om verzoeken via uw eigen HTTP-stack, een proxy of een test double te laten lopen. Implementaties geven elke HTTP-status terug (redirects worden niet gevolgd) en gooien een `TransportException` bij netwerkfouten.
- **Threads:** een client bewaart het laatste verzoek en antwoord voor diagnose en is niet thread-safe. Gebruik één instantie per thread (of per verzoek) of synchroniseer de toegang. `Dispose()` geeft de onderliggende `HttpClient` vrij.
- **User-Agent:** verzoeken worden verzonden als `DomainNameAPI-DotNet/1.0.0`.

### Voorbeelden

Het project [examples](examples/DomainNameApi.Examples) bevat één uitvoerbaar bestand per methode (`GetDetails`, `RegisterWithContactInfo`, `RegisterWithContactInfo-TR`, de methoden voor DNS-zones en doorsturing, ...). Stel uw inloggegevens in en voer er een uit:

```bash
export DNA_RESELLER_ID=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
export DNA_API_KEY=your-api-key
export DNA_TEST_MODE=true   # optional: use the OTE test environment

dotnet run --project examples/DomainNameApi.Examples -- GetDetails
dotnet run --project examples/DomainNameApi.Examples            # lists all examples
```

### Testen

De tests hebben geen inloggegevens nodig en doen geen externe netwerkaanroepen:

```bash
dotnet test
```

Zonder lokale .NET SDK voert u ze uit in Docker:

```bash
docker run --rm -v "$PWD":/src -w /src mcr.microsoft.com/dotnet/sdk:8.0 dotnet test
```

| Testbestand | Beschrijving |
|-----------|-------------|
| `tests/DomainNameApi.Tests/ConformanceTests.cs` | Speelt alle 126 conformiteitsfixtures van php-dna (`testdata/conformance/`) af en vergelijkt verzoeken en resultaten |
| `tests/DomainNameApi.Tests/HttpLayerTests.cs` | Headers, URL's, querystrings, testmodus, time-outs, tempo van zone-aanroepen |
| `tests/DomainNameApi.Tests/HttpClientTransportTests.cs` | Het standaardtransport op basis van `HttpClient` tegen een lokale listener |
| `tests/DomainNameApi.Tests/ErrorMappingTests.cs` | Vertaling van gatewayfouten en validatiemeldingen |
| `tests/DomainNameApi.Tests/TelemetryTests.cs` | Telemetrie-payloads, maskering van inloggegevens, minimale modus |
| `tests/DomainNameApi.Tests/UtilityTests.cs` | Contactvalidatie, normalisatie van telefoonnummers en landen, PHP-compatibele opmaak |

<hr style="border: 4px solid #000; border-style: dashed;">

## Retour- en foutcodes met beschrijvingen

| Code | Beschrijving                                   | Detail                                                                                                                                                         |
|------|------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1000 | Commando succesvol voltooid                    | Bewerking geslaagd.                                                                                                                                            |
| 1001 | Commando succesvol voltooid; actie in behandeling | Bewerking geslaagd. De bewerking staat momenteel echter in de wachtrij om te worden voltooid.                                                               |
| 2003 | Vereiste parameter ontbreekt                   | Fout door ontbrekende parameter. Bijvoorbeeld: geen telefoonnummer in de contactgegevens.                                                                      |
| 2105 | Object komt niet in aanmerking voor verlenging | De domeinstatus staat geen verlenging toe; het domein is vergrendeld voor wijzigingen. De status mag niet "clientupdateprohibited" zijn. Kan ook door andere statusvoorwaarden komen. |
| 2200 | Authenticatiefout                              | Autorisatiefout: de beveiligingscode is onjuist of het domein is geregistreerd bij een andere registrar.                                                       |
| 2302 | Object bestaat al                              | Domeinnaam of nameserverinformatie bestaat al in de database. Kan niet worden geregistreerd.                                                                   |
| 2303 | Object bestaat niet                            | Domeinnaam of nameserverinformatie bestaat niet in de database. Nieuwe registratie vereist.                                                                    |
| 2304 | Objectstatus verbiedt bewerking                | De domeinstatus staat geen wijzigingen toe; het domein is vergrendeld voor wijzigingen. De status mag niet "clientupdateprohibited" zijn. Kan ook door andere statusvoorwaarden komen. |

| CODE | DETAIL                                                                                                                               |
|------|--------------------------------------------------------------------------------------------------------------------------------------|
| 101  | Systeemfout, details voor ({0})!                                                                                                     |
| 102  | Meerdere fouten, details voor ({0})!                                                                                                 |
| 103  | Onbekende fouten, details voor ({0})!                                                                                                |
| 200  | API-commando niet gevonden ({0})!                                                                                                    |
| 210  | API-service niet gevonden ({0})!                                                                                                     |
| 211  | API-serviceprovider niet ingesteld!                                                                                                  |
| 300  | Reseller niet gevonden!                                                                                                              |
| 301  | Uw huidige IP-adres heeft geen toegang. Zorg ervoor dat u verbinding maakt vanaf een geautoriseerd IP-adres en probeer het opnieuw  |
| 310  | TLD wordt niet ondersteund!                                                                                                          |
| 320  | API niet gevonden!                                                                                                                   |
| 321  | Valuta wordt niet ondersteund!                                                                                                       |
| 330  | Vereiste parameter(s) niet ingesteld ({0}).                                                                                          |
| –    | Zorg ervoor dat u alle contactobjecten volledig verzendt                                                                             |
| 340  | Prijsdefinitie niet gevonden ({0}[{1}]-{2}{3}).                                                                                      |
| 350  | Onvoldoende resellersaldo. (Reseller Id : {0} - Huidig saldo : {1} {2}).                                                             |
| 350  | De boekhoudvaluta komt niet overeen of het saldo is onvoldoende.                                                                     |
| 360  | Ongeldig API-verzoek voor veld ({0}).                                                                                                |
| 360  | API-quotum overschreden!                                                                                                             |
| 361  | Fout door throttling (te veel verzoeken)!                                                                                            |
| 362  | Premiumdomein kan op dit moment niet worden geregistreerd.                                                                           |
| 363  | Bewerking geannuleerd omdat het domein zich in de periode voor automatische verlenging bevindt                                       |
| 364  | Dit domein is momenteel niet beschikbaar voor transacties vanwege een probleem bij het register                                      |

| CODE | DETAIL                                                                                      |
|------|---------------------------------------------------------------------------------------------|
| 400  | Ongeldig contact ({0}).                                                                     |
| 401  | Contactgegevens kunnen niet worden gesynchroniseerd.                                        |
| 402  | Geen toegang tot contactgegevens bij het register.                                          |
| 403  | ER ONTBREEKT INFORMATIE IN HET SYSTEEM. VOER UW STANDAARDGEGEVENS IN OF NEEM CONTACT OP MET HET SUPPORTTEAM. |
| 404  | DOMEINCONTACT KAN NIET WORDEN BIJGEWERKT. TOESTEMMING VAN HET REGISTER VEREIST.             |
| 410  | Contact niet gevonden.                                                                      |
| 410  | Contact niet gevonden ({0}).                                                                |
| 420  | Ongeldig API-commando voor contact {0}.                                                     |
| 430  | Contact-API niet gevonden.                                                                  |
| 440  | Contact is niet gesynchroniseerd.                                                           |
| 450  | Te veel domeincontacten!                                                                    |
| 451  | Het bijwerken van het contact kon niet worden uitgevoerd.                                   |

| CODE | DETAIL                                                                                                                               |
|------|--------------------------------------------------------------------------------------------------------------------------------------|
| 500  | Ongeldige domein-id.                                                                                                                 |
| 500  | Ongeldige domein-id ({0}).                                                                                                           |
| 501  | Domein kon niet worden gesynchroniseerd ({0})                                                                                        |
| 502  | Interne transfer mislukt                                                                                                             |
| 503  | Domeinregistratie is niet beschikbaar.                                                                                               |
| 504  | Domeingegevens komen niet overeen. Voorheen: {0}                                                                                     |
| 505  | Voer het IP-adres in.                                                                                                                |
| 506  | De domeintransfer is gestart, maar de contactgegevens konden niet worden gelezen.                                                    |
| 507  | Autorisatiefout.                                                                                                                     |
| 510  | Domein niet gevonden.                                                                                                                |
| 511  | Verlopen domeinen kunnen niet worden gevonden.                                                                                       |
| 512  | Domein kan niet worden verlengd.                                                                                                     |
| 513  | Het domein heeft geen status waarin het kan worden bijgewerkt. Het moet actief zijn om te kunnen worden bijgewerkt                   |
| 514  | Redemption Period (herstelperiode) verwacht.                                                                                         |
| 520  | Ongeldig API-commando voor domein "{0}".                                                                                             |
| 530  | Ongeldige domeinperiode. De periode moet {0} tot {1} jaar zijn. Gevraagde periode is {2} jaar.                                       |
| 540  | Het domein kan niet verder dan {0} jaar vanaf de huidige datum worden verlengd.                                                      |
| 550  | Ongeldige domeinnaam. Het domein moet {0} tot {1} tekens lang zijn. De gevraagde domeinnaam is {2} tekens lang.                      |
| 560  | Ongeldig aantal nameservers. Het domein moet {0} tot {1} nameservers hebben. Gevraagd aantal nameservers is {2}.                     |
| 561  | Geen nameserverinformatie gevonden in het binnenkomende verzoek                                                                      |
| 570  | IDN wordt niet ondersteund voor tld "{0}".                                                                                           |
| 571  | Periode ongeldig.                                                                                                                    |
| 572  | Commando ongeldig.                                                                                                                   |
| 573  | Domeinnamen niet gevonden.                                                                                                           |
| 574  | TLD-namen niet gevonden.                                                                                                             |
| 575  | Het domein heeft geen status waarin het kan worden bijgewerkt. Het moet actief zijn om de nameservers te kunnen bijwerken            |
| 576  | Het domein kan in de laatste maand vóór de vervaldatum worden verlengd                                                               |
| 580  | Transfer wordt niet ondersteund voor tld "{0}".                                                                                      |
| 581  | Child nameserver niet gevonden                                                                                                       |
| 582  | Transfer is gestart door een andere reseller.                                                                                        |
| 583  | Transfer niet geïnitieerd. Neem contact op met het supportteam.                                                                      |
| 584  | Objectstatus verbiedt bewerking                                                                                                      |
| 590  | Voor deze transfer is een auth-code vereist.                                                                                         |
| 591  | Auth-code is ongeldig.                                                                                                               |
| 592  | Er staat een transferlock op het domein.                                                                                             |
| 593  | Domein kan niet worden overgedragen; statusinformatie en transferlock moeten aan de vereiste criteria voldoen. (Statusinformatie: #ok) |
| 594  | Nameserveradres van het domein kon niet worden opgelost ({0})                                                                        |
| 595  | Contactgegevens kunnen niet worden gelezen (whois.registrar.tld). Zorg ervoor dat de status van de privacybescherming open is          |
| 596  | Contactgegevens konden niet worden geverifieerd                                                                                      |
| 597  | TLD niet gevonden                                                                                                                    |
| 598  | Er is een onbekende fout opgetreden                                                                                                  |
| 599  | Domeindoorsturing niet gevonden                                                                                                      |

## Licentie

Apache License 2.0 — zie [LICENSE](LICENSE).
