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

# DomainNameAPI .NET-Bibliothek (dotnet-dna)

Der offizielle .NET-Client für die Reseller-REST-API von DomainNameAPI: Domainsuche, Registrierung, Verlängerung, Transfers, Kontakte, Nameserver, Child-Nameserver, DNS-Zonen und URL-Weiterleitung.

dotnet-dna ist eine originalgetreue Portierung von [php-dna](https://github.com/domainreseller/php-dna): dieselben Methoden, dieselben Anfragen auf der Leitung und dieselben Rückgabewerte. Die Bibliothek wird gegen die Konformitäts-Fixtures von php-dna (134 Szenarien) geprüft, sodass sich Code, der für die PHP-Bibliothek geschrieben wurde, eins zu eins übertragen lässt.

## 📦 Download — immer die Releases verwenden!

⬇️ **Die neueste getestete Version erhalten Sie hier: https://github.com/domainreseller/dotnet-dna/releases/latest**

> ⚠️ Verwenden Sie **nicht** den grünen Button **Code → Download ZIP** — damit laden Sie den rohen Entwicklungszweig herunter. Release-Pakete sind versioniert, getestet und produktionsreif.

## Installations- und Integrationsanleitung

### Mindestanforderungen

- .NET 8 oder neuer, **oder** jede Laufzeitumgebung, die .NET Standard 2.0 unterstützt (.NET Framework 4.6.2+, .NET Core 2.0+, Mono, Xamarin, Unity).
- Keine Abhängigkeiten von Drittanbietern. Die Bibliothek verwendet `HttpClient` und `System.Text.Json` (unter .NET Standard 2.0 wird das Paket `System.Text.Json` automatisch eingebunden).

### Installation

**A) NuGet-Paket** — die Paket-ID lautet `DomainNameApi`. Jedes Release enthält die `.nupkg`-Datei; sobald das Paket auf NuGet.org gelistet ist, können Sie es auch direkt installieren:

```bash
dotnet add package DomainNameApi
```

Um eine heruntergeladene `.nupkg` zu installieren, legen Sie sie in einem Ordner ab und verwenden Sie diesen Ordner als Paketquelle:

```bash
dotnet add package DomainNameApi --source ./packages
```

**B) Projektverweis** — laden Sie den Quellcode des Releases herunter und verweisen Sie auf das Projekt:

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

## 🔑 API-Zugangsdaten — Reseller-ID & API-Key

Die Bibliothek kommuniziert mit der DomainNameAPI REST-API und benötigt genau zwei Zugangsdaten, die dem Konstruktor übergeben werden:

| Parameter | Wert |
|---|---|
| Erster | Reseller-ID — UUID wie `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| Zweiter | API-Key |

> 💡 Ihre **Reseller-ID** und Ihren **API-Key** finden Sie in Ihrem DomainNameAPI-Panel unter **API-Einstellungen**.
> ⚠️ Dies sind **API-Zugangsdaten** — Ihre Panel-Anmeldedaten funktionieren hier **nicht**.

```csharp
using DomainNameApi;

var resellerId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
var apiKey     = "your-api-key";

// Live environment
var dna = new DomainNameApiClient(resellerId, apiKey);

// Test (OTE) environment
var dna = new DomainNameApiClient(resellerId, apiKey, true);
```

Führende und nachfolgende Leerzeichen werden aus beiden Zugangsdaten entfernt. Das Testmodus-Flag kann auch als Text übergeben werden, so wie Konfigurationsdateien und Panels es speichern: `"true"`, `"1"`, `"on"` und `"yes"` (Groß-/Kleinschreibung egal) wählen die Testumgebung, jeder andere Wert wählt die Live-Umgebung.

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey, Environment.GetEnvironmentVariable("DNA_TEST_MODE"));
```

## 🚀 Schnellstart

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

### Rückgabewerte

Jede Methode gibt dieselbe Struktur zurück wie php-dna, und zwar als `System.Text.Json.Nodes.JsonNode` (in der Regel ein `JsonObject`). Werte lesen Sie per Cast oder mit `GetValue<T>()` aus; das gesamte Ergebnis geben Sie mit `ToJsonString()` aus:

```csharp
var details = dna.GetDetails("example.com");
string status = (string)details["data"]["Status"];          // "Active"
int    days   = (int)details["data"]["Dates"]["RemainingDays"];
bool   locked = (string)details["data"]["LockStatus"] == "true";   // flags are the strings "true"/"false"
Console.WriteLine(details.ToJsonString());
```

Schlüsselnamen, String-Flags (`"true"`/`"false"`) und formatierte Geldbeträge als Strings (`"11.9900"`) entsprechen exakt php-dna. Die meisten Methoden verwenden die Hülle `result` = `OK`/`ERROR`; `GetCurrentBalance` verwendet die ältere `OperationResult`-Hülle, und `CheckAvailability` gibt im Erfolgsfall eine reine Liste zurück.

Eingaben wie Kontakte, TR-Attribute und Listenfilter sind `IDictionary<string, object>` mit den Schlüsseln von php-dna (`FirstName`, `EMail`, `AddressLine1`, `PhoneCountryCode`, `TRABISCITIZENID`, `PageSize`, ...). Nameserver-Listen können als Liste (`string[]`, `List<string>`) oder als Dictionary (es werden die Werte verwendet) übergeben werden.

### Verwendung

Alle folgenden Beispiele lassen sich auch aus dem Ordner [examples](examples) ausführen (siehe [Beispiele](#beispiele)).

#### Domain-Registrierung

Hinweis: Für .tr-Domains sind zusätzliche Parameter erforderlich. Der Parameter `additionalAttributes` wird für Domainnamen verwendet, die wie .tr zusätzliche Angaben benötigen.

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

Kontakte werden vor dem Senden vervollständigt: Fehlende Felder erhalten Platzhalterwerte, Ziffern und Satzzeichen werden aus Namen entfernt, ein fehlendes oder leeres Bundesland wird durch die Stadt ersetzt, Ländernamen werden in ISO-Codes umgewandelt und Telefonnummern in Nummer und Ländervorwahl aufgeteilt — eine Nummer, die mit eigener Ländervorwahl per `+` oder `00` geschrieben ist, behält diese Vorwahl (siehe `ValidateContact`). Wird kein Nameserver (oder nur leere) angegeben, werden `ns1.domainnameapi.com` und `ns2.domainnameapi.com` verwendet. Bei .tr-Domains werden die TRABIS-Attribute auf das reduziert, was das Gateway akzeptiert (`TRABISDOMAINCATEGORY` = `Owner`/`Corporate`, `TRABISCITIZENID`, `TRABISTAXOFFICE`, `TRABISTAXNUMBER`); der veraltete Schlüssel `TRABISCITIZIENID` und die numerische Kategorie werden automatisch umgewandelt.

<details>
<summary>Beispielausgabe für die Domain-Registrierung</summary>

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

Lehnt das Gateway den Auftrag ab (es antwortet mit `"success": false`), gibt die Methode eine Fehlerhülle mit `Code` = `REGISTER_DOMAIN` und der Meldung des Gateways zurück, sodass eine abgelehnte Registrierung niemals als erledigt gemeldet wird. `Renew` (`RENEW_DOMAIN`) und `Transfer` (`TRANSFER_DOMAIN`) verhalten sich genauso.

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domain-Verlängerung

```csharp
var result = dna.Renew("example.com", 1);
```

<details>
<summary>Beispielausgabe für die Domain-Verlängerung</summary>

```json
{ "result": "OK", "data": { "ExpirationDate": "2025-03-15T10:00:00" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domain-Transfer

```csharp
var result = dna.Transfer("example.com", "Xy9#mK2$", 3);
```

> ℹ️ Ein Transfer sendet niemals Kontakte: Die Anfrage enthält stets eine leere Kontaktliste, und der Registrar übernimmt die Kontakte selbst. `Transfer` hat daher keinen Kontakt-Parameter; aktualisieren Sie die Kontakte bei Bedarf nach Abschluss des Transfers mit `SaveContacts`.

<details>
<summary>Beispielausgabe für den Domain-Transfer</summary>

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

#### Transfer abbrechen, genehmigen oder ablehnen

```csharp
dna.CancelTransfer("example.com");   // pending incoming transfer
dna.ApproveTransfer("example.com");  // pending outgoing transfer
dna.RejectTransfer("example.com");   // pending outgoing transfer
```

<details>
<summary>Beispielausgabe für das Abbrechen eines Transfers</summary>

```json
{ "result": "OK", "data": { "DomainName": "example.com", "Status": "Cancelled" } }
```

`ApproveTransfer` und `RejectTransfer` antworten auf dieselbe Weise mit `Approved` / `Rejected`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Transfer-Prüfung

```csharp
var result = dna.CheckTransfer("example.com", "Ab1!xyZ9");
```

`result` ist nur dann `OK`, wenn die Registry die Domain als transferierbar meldet; andernfalls ist es `ERROR` mit `Code` = `TRANSFER_NOT_AVAILABLE`, und die Details stehen weiterhin unter `data`.

<details>
<summary>Beispielausgabe für die Transfer-Prüfung</summary>

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

#### Domain-Liste

```csharp
var result = dna.GetList(new Dictionary<string, object>
{
    ["OrderColumn"]    = "Id",
    ["OrderDirection"] = "ASC",
    ["PageNumber"]     = 0,
    ["PageSize"]       = 1000,
});
```

`PageNumber` beginnt bei null. Die gatewayeigenen Filternamen (`MaxResultCount`, `SkipCount`, `Sorting`, `DomainName`, ...) können ebenfalls übergeben werden.

<details>
<summary>Beispielausgabe für die Domain-Liste</summary>

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

#### TLD-Liste

```csharp
var result = dna.GetTldList(100);
```

<details>
<summary>Beispielausgabe für die TLD-Liste</summary>

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

#### Verfügbarkeitsprüfung für Domains

```csharp
var result = dna.CheckAvailability(new[] { "hello", "world123x0" }, new[] { "com", "net" }, 1, "create");
```

<details>
<summary>Beispielausgabe für die Verfügbarkeitsprüfung</summary>

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

Im Fehlerfall gibt die Methode statt einer Liste die übliche Fehlerhülle zurück; prüfen Sie daher zuerst `result is JsonArray`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domain-Details

```csharp
var result = dna.GetDetails("example.com");
var synced = dna.SyncFromRegistry("example.com"); // same envelope
```

<details>
<summary>Beispielausgabe für die Domain-Details</summary>

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

#### Nameserver ändern

```csharp
var result = dna.ModifyNameServer("example.com", new[] { "ns1.example.com", "ns2.example.com" });
```

Hostnamen werden von Leerzeichen befreit, in Kleinbuchstaben umgewandelt und um einen abschließenden Punkt gekürzt; leere und doppelte Einträge werden verworfen.

<details>
<summary>Beispielausgabe für das Ändern der Nameserver</summary>

```json
{ "result": "OK", "data": { "NameServers": [ "ns1.example.com", "ns2.example.com" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domain-Sperre aktivieren

```csharp
var result = dna.EnableTheftProtectionLock("example.com");
```

<details>
<summary>Beispielausgabe für das Aktivieren der Domain-Sperre</summary>

```json
{ "result": "OK", "data": { "LockStatus": true } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domain-Sperre deaktivieren

```csharp
var result = dna.DisableTheftProtectionLock("example.com");
```

<details>
<summary>Beispielausgabe für das Deaktivieren der Domain-Sperre</summary>

```json
{ "result": "OK", "data": { "LockStatus": false } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Child-Nameserver hinzufügen

```csharp
var result = dna.AddChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>Beispielausgabe für das Hinzufügen eines Child-Nameservers</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Child-Nameserver löschen

```csharp
var result = dna.DeleteChildNameServer("example.com", "ns1.example.com");
```

<details>
<summary>Beispielausgabe für das Löschen eines Child-Nameservers</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Child-Nameserver aktualisieren

```csharp
var result = dna.ModifyChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>Beispielausgabe für das Aktualisieren eines Child-Nameservers</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domain-Datenschutz ändern

```csharp
var result = dna.ModifyPrivacyProtectionStatus("example.com", true);

// The flag may also be text, as panels store it: "1", "true", "on" and "yes" enable privacy
var result2 = dna.ModifyPrivacyProtectionStatus("example.com", "on");
```

Das optionale dritte Argument (`reason`) existiert nur aus Kompatibilitätsgründen; das Gateway kennt kein solches Feld, und es wird nicht gesendet.

<details>
<summary>Beispielausgabe für das Ändern des Domain-Datenschutzes</summary>

```json
{ "data": { "PrivacyProtectionStatus": true }, "result": "OK" }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domain-Kontakte abrufen

```csharp
var result = dna.GetContacts("example.com");
```

<details>
<summary>Beispielausgabe für das Abrufen der Domain-Kontakte</summary>

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

#### Domain-Kontakte speichern

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

Kontakte können auch in der verschachtelten Form übergeben werden (`Address` → `Line1`/`City`/..., `Phone` → `Phone`/`Fax` → `Number`/`CountryCode`), also in der Form, die `GetContacts` zurückgibt.

<details>
<summary>Beispielausgabe für das Speichern der Domain-Kontakte</summary>

```json
{ "result": "OK", "data": { "contacts": [] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Reseller-Details abrufen

```csharp
var result = dna.GetResellerDetails();
```

<details>
<summary>Beispielausgabe für das Abrufen der Reseller-Details</summary>

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

#### Aktuelles Guthaben

```csharp
var result = dna.GetCurrentBalance("USD"); // USD, TRY, EUR or GBP
```

<details>
<summary>Beispielausgabe für das aktuelle Guthaben</summary>

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

Im Fehlerfall ist `OperationResult` gleich `FAILED`, `ErrorCode` enthält den Fehlercode (zum Beispiel `API_429`) und `Balance` ist `"0.00"`. Das Konto meldet nur Guthaben in USD und TRY; die Abfrage einer anderen Währung (EUR, GBP, ...) schlägt mit `ErrorCode` = `BALANCE` und der Meldung `No EUR balance is reported for this account. Available: USD, TRY.` fehl.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### DNS-Zonenverwaltung

Das Gateway speichert eine Zone als Record-**Sets** (Name + Typ → Werte) und ersetzt bei jedem Schreibvorgang ein komplettes Set. Die Methoden `*ResourceRecordSet` und `*ResourceRecordValue` übernehmen das für Sie. Namen können relativ (`www`), die Zonenspitze (`@`, `""` oder `example.com`) oder vollqualifiziert (`www.example.com.`) sein. Unterstützte Typen: `A`, `AAAA`, `ALIAS`, `CAA`, `CNAME`, `DS`, `MX`, `NAPTR`, `PTR`, `SRV`, `TLSA`, `TXT`. Schreibvorgänge werden sofort übernommen, es sei denn, Sie übergeben `apply: false` und rufen `ApplyResourceRecords` selbst auf; schlägt dieses automatische Übernehmen fehl, gibt die Methode den Fehler des Übernehmens zurück (die Änderung bleibt vorgemerkt und ist nicht live). Aufeinanderfolgende Zonenaufrufe erfolgen im Abstand von mindestens einer Sekunde, da das Gateway auf schnelle Aufrufserien mit HTTP 429 antwortet.

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
<summary>Beispielausgabe für die DNS-Zonenverwaltung</summary>

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

`SetResourceRecordSet` / `AddResourceRecordValue` (der Typ wird in Großbuchstaben gemeldet und die TTL so, wie sie gesendet wurde, begrenzt auf 1–86400 Sekunden):

```json
{ "result": "OK", "data": { "Name": "", "Type": "MX", "TTL": 3600, "Contents": [ "10 mx1.example.com", "20 mx2.example.com" ] } }
```

Das Hinzufügen eines bereits vorhandenen Werts oder das Entfernen eines nicht vorhandenen Werts ändert nichts und liefert `{"result":"OK","data":{..., "Changed": false}}`. Ein nicht unterstützter Typ wird abgewiesen, bevor überhaupt ein Aufruf erfolgt:

```json
{ "result": "ERROR", "error": { "Code": "ADD_ZONE", "Message": "Record type \"NS\" is not supported by the DNS zone API. Supported types: A, AAAA, ALIAS, CAA, CNAME, DS, MX, NAPTR, PTR, SRV, TLSA, TXT.", "Details": "..." } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### URL-Weiterleitung

```csharp
dna.SetForwarding("example.com", "https://www.example.org", "Standard"); // or "Frame" (masked)
var forwarding = dna.GetForwarding("example.com");
dna.DeleteForwarding("example.com");
```

<details>
<summary>Beispielausgabe für die URL-Weiterleitung</summary>

```json
{ "result": "OK", "data": { "Enabled": true, "RedirectAddress": "https://www.example.org/landing", "ForwardType": "Frame" } }
```

Ist keine Weiterleitung eingerichtet (das Gateway antwortet mit `Redirect information not found`), gibt `GetForwarding` den Wert `{"result":"OK","data":{"Enabled":false,"RedirectAddress":"","ForwardType":""}}` zurück. Jeder andere Fehler — zum Beispiel eine nicht existierende Domain — wird als Fehler zurückgegeben.

`DeleteForwarding` gibt `{"result":"OK","data":{"DomainName":"example.com","Removed":true}}` zurück; war nichts eingerichtet, ist der Aufruf wirkungslos und antwortet mit `"Removed": false`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Hilfsfunktionen (ohne Netzwerk)

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
<summary>Beispielausgabe für ValidateContact</summary>

```json
{
  "FirstName": "Jo", "Phone": "12025550100", "Country": "TR", "LastName": "Isimyok",
  "AddressLine1": "Addres yok", "City": "ISTANBUL", "State": "ISTANBUL", "ZipCode": "34000",
  "PhoneCountryCode": "90", "Fax": "", "FaxCountryCode": ""
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

### Fehlerbehandlung

Bei API- oder Netzwerkproblemen werfen die Methoden niemals eine Ausnahme; stattdessen geben sie eine Fehlerhülle zurück:

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
| HTTP-Fehler vom Gateway | `API_<status>`, z. B. `API_400`, `API_429`, `API_500` | die Meldung des Gateways; bei leerem Antwortkörper `HTTP 429 - Too Many Requests` |
| Falsche Zugangsdaten (HTTP 301/302) | `API_302` | `Invalid API credentials` |
| Validierungsfehler | `API_400` | die Meldung des Gateways; `Details` listet die Felder auf, z. B. `Admin PostalCode - 'PostalCode' is required` |
| Zeitüberschreitung | `CURL_28` | `Could not reach the DomainNameAPI service: the connection timed out. (...)` |
| Sonstige Netzwerkfehler (DNS, Verbindungsaufbau, TLS, Verbindungsabbruch) | `CURL_6`, `CURL_7`, `CURL_60`, `CURL_56` | `Curl error during request: ...` |
| Registrierung, Verlängerung oder Transfer abgelehnt (`"success": false`) | `REGISTER_DOMAIN`, `RENEW_DOMAIN`, `TRANSFER_DOMAIN` | die Meldung des Gateways |
| Leere oder unbrauchbare Erfolgsantwort | `RESPONSE` | `Invalid response received from server! Response is empty.` |
| Prüfungen vor dem Aufruf des Gateways | z. B. `AVAILABILITY`, `ADD_ZONE`, `SET_FORWARD` | eine Beschreibung des Problems |

`API_<n>`-Codes bedeuten immer einen HTTP-Status. Hat das Gateway einen eigenen Fehlercode gesendet, enthält die Hülle diesen zusätzlich als `GatewayCode` (zum Beispiel `Dna.DomainService:Domain:10007`), sodass Sie „Domain nicht gefunden“ von „TLD nicht unterstützt“ unterscheiden können, ohne die Meldung auszuwerten. Netzwerkfehler verwenden `CURL_<n>`, wobei die Nummer der Nummerierung von libcurl folgt, damit die Codes mit php-dna übereinstimmen. Nur Programmierfehler (zum Beispiel die Übergabe eines nicht unterstützten Objekttyps) können eine Ausnahme auslösen.

Zur Fehlersuche lässt sich der letzte Aufruf untersuchen:

```csharp
dna.GetLastFunction();       // "request:GET domains/info"
dna.GetRequestData();        // { "url": ..., "method": ..., "payload": ... }
dna.GetResponseData();       // decoded response, or { "raw_response": "..." }
dna.GetResponseHeaders();    // { "url", "http_code", "content_type", "total_time", "headers" }
dna.GetParsedResponseData(); // parsed response or the error built from it
```

### Testmodus

Übergeben Sie `true` als drittes Konstruktorargument, um statt der Live-Umgebung (`https://api.domainresellerapi.com/api/v1`) die OTE-Testumgebung (`https://ote.domainresellerapi.com/api/v1`) zu verwenden. `GetServiceUrl()` zeigt die verwendete Adresse an, und `SetServiceUrl(url)` überschreibt sie.

### Fehlerberichte (Telemetrie)

Wenn ein API-Aufruf fehlschlägt, kann die Bibliothek einen Fehlerbericht an den eigenen Fehler-Tracker von DomainNameAPI (Sentry) senden. Ein kleiner, zufällig gewählter Anteil erfolgreicher Aufrufe (4 %, dazu jeder Aufruf, der länger als 1 Sekunde dauert) kann als Performance-Datensatz gesendet werden. Das hilft DomainNameAPI, Probleme zu finden und zu beheben. Fehlerberichte sind **standardmäßig aktiviert**.

> ℹ️ In Version 1.0.0 ist die Sentry-Adresse (`DefaultSentryDsn`) noch leer, da das Sentry-Projekt für die .NET-Bibliothek noch nicht angelegt wurde. Bis ein Release sie einträgt, wird **nichts gesendet**. Das folgende Verhalten beschreibt, was gesendet wird, sobald sie gesetzt ist.

**Was gesendet wird:**

- Fehlercode und -meldung
- Pfad des aufgerufenen Endpunkts und HTTP-Methode
- Dauer des Aufrufs
- Anwendungsname (`DomainNameAPI-DotNet`), Bibliotheksversion und .NET-Version
- Reseller-ID und Domainname
- die öffentliche IP-Adresse des Servers (im Hintergrund ermittelt über `api.ipify.org`)
- der Name des Betriebssystemkontos, unter dem die Anwendung läuft
- API-Anfrage und -Antwort, einschließlich der Kontaktdaten der Domain; nur Zugangsdaten werden maskiert – EPP-/Auth-Codes, API-Keys, Passwörter und Tokens werden durch `***` ersetzt

**Was nie gesendet wird:** EPP-/Auth-Codes, API-Keys, Passwörter oder Tokens in lesbarer Form; Methodenargumente in Stacktraces.

**Wozu und wie lange:** Die Daten werden ausschließlich zur Nutzungsmessung und zur frühzeitigen Erkennung von Problemen verwendet und 30 Tage lang auf dem eigenen Sentry-Server von DomainNameAPI gespeichert; danach werden sie automatisch gelöscht.

Berichte werden im Hintergrund gesendet: Sie blockieren oder verlangsamen Ihren Aufruf nie und lösen niemals eine Ausnahme aus. Erwartete, häufig auftretende Fehler (Domain nicht gefunden, unzureichendes Guthaben, ungültige Zugangsdaten, keine Weiterleitung eingerichtet, ...) werden nicht gemeldet.

**So schalten Sie es ab:** Rufen Sie nach dem Erstellen des Clients `SetErrorReportingEnabled(false)` auf:

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey);
dna.SetErrorReportingEnabled(false);
```

**Minimalmodus:** `SetTelemetryMinimal(true)` lässt die Fehlerberichte aktiviert, sendet aber nur Fehlercode und -meldung, Endpunkt, Zeitmessung, Versionen und Reseller-ID — keine Anfrage-/Antwortdaten, keine Domain, keine IP-Adresse und keinen Kontonamen.

Das Ziel lässt sich über die Eigenschaft `TelemetryDsn` ändern (zum Beispiel auf Ihr eigenes Sentry-Projekt).

> 🔒 **Sicherheit:** Die TLS-Zertifikatsprüfung ist aktiviert. Auf einem Host mit veraltetem CA-Bundle kann sie mit `dna.SetSslVerification(false);` abgeschaltet werden — nicht empfohlen; besser ist es, das CA-Bundle zu aktualisieren.

### Erweitert

- **Timeouts:** 30 Sekunden pro Anfrage und 10 Sekunden für den Verbindungsaufbau; ändern Sie sie über die Eigenschaften `Timeout` und `ConnectTimeout`.
- **Eigener Transport:** Übergeben Sie dem Konstruktor einen `IHttpTransport` (`new DomainNameApiClient(id, key, false, transport)`) oder setzen Sie die Eigenschaft `Transport`, um Anfragen über Ihren eigenen HTTP-Stack, einen Proxy oder ein Test-Double zu leiten. Implementierungen geben jeden HTTP-Status zurück (Weiterleitungen werden nicht verfolgt) und lösen bei Netzwerkfehlern eine `TransportException` aus.
- **Threads:** Ein Client speichert die letzte Anfrage und Antwort zu Diagnosezwecken und ist nicht threadsicher. Verwenden Sie eine Instanz pro Thread (oder pro Anfrage) oder synchronisieren Sie den Zugriff. `Dispose()` gibt den zugrunde liegenden `HttpClient` frei.
- **User-Agent:** Anfragen werden als `DomainNameAPI-DotNet/1.0.0` gesendet.

### Beispiele

Das Projekt [examples](examples/DomainNameApi.Examples) enthält eine ausführbare Datei pro Methode (`GetDetails`, `RegisterWithContactInfo`, `RegisterWithContactInfo-TR`, die Methoden für DNS-Zonen und Weiterleitungen, ...). Setzen Sie Ihre Zugangsdaten und führen Sie ein Beispiel aus:

```bash
export DNA_RESELLER_ID=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
export DNA_API_KEY=your-api-key
export DNA_TEST_MODE=true   # optional: use the OTE test environment

dotnet run --project examples/DomainNameApi.Examples -- GetDetails
dotnet run --project examples/DomainNameApi.Examples            # lists all examples
```

### Tests

Die Tests benötigen keine Zugangsdaten und führen keine externen Netzwerkaufrufe aus:

```bash
dotnet test
```

Ohne lokales .NET SDK führen Sie sie in Docker aus:

```bash
docker run --rm -v "$PWD":/src -w /src mcr.microsoft.com/dotnet/sdk:8.0 dotnet test
```

| Testdatei | Beschreibung |
|-----------|-------------|
| `tests/DomainNameApi.Tests/ConformanceTests.cs` | Spielt alle 134 Konformitäts-Fixtures von php-dna (`testdata/conformance/`) ab und vergleicht Anfragen und Ergebnisse |
| `tests/DomainNameApi.Tests/HttpLayerTests.cs` | Header, URLs, Query-Strings, Testmodus, Timeouts, Zonen-Taktung |
| `tests/DomainNameApi.Tests/HttpClientTransportTests.cs` | Der Standard-Transport auf Basis von `HttpClient` gegen einen lokalen Listener |
| `tests/DomainNameApi.Tests/ErrorMappingTests.cs` | Zuordnung der Gateway-Fehler und Validierungsmeldungen |
| `tests/DomainNameApi.Tests/TelemetryTests.cs` | Telemetrie-Payloads, Maskierung von Zugangsdaten, Minimalmodus |
| `tests/DomainNameApi.Tests/UtilityTests.cs` | Kontaktvalidierung, Normalisierung von Telefonnummern und Ländern, PHP-kompatible Formatierung |

<hr style="border: 4px solid #000; border-style: dashed;">

## Rückgabe- und Fehlercodes mit Erklärungen

| Code | Beschreibung                                   | Details                                                                                                                                                        |
|------|------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1000 | Befehl erfolgreich abgeschlossen               | Vorgang erfolgreich.                                                                                                                                           |
| 1001 | Befehl erfolgreich abgeschlossen; Aktion ausstehend | Vorgang erfolgreich. Der Vorgang befindet sich jedoch derzeit zur Fertigstellung in der Warteschlange.                                                    |
| 2003 | Erforderlicher Parameter fehlt                 | Fehler wegen fehlendem Parameter. Zum Beispiel: keine Telefonnummer in den Kontaktinformationen angegeben.                                                     |
| 2105 | Objekt ist nicht für eine Verlängerung geeignet | Der Domain-Status erlaubt keine Verlängerung, die Domain ist für Aktualisierungen gesperrt. Der Status darf nicht "clientupdateprohibited" sein. Kann auch an anderen Statusbedingungen liegen. |
| 2200 | Authentifizierungsfehler                       | Autorisierungsfehler, der Sicherheitscode ist falsch oder die Domain ist bei einem anderen Registrar registriert.                                               |
| 2302 | Objekt existiert bereits                       | Domainname oder Nameserver-Information existiert bereits in der Datenbank. Kann nicht registriert werden.                                                      |
| 2303 | Objekt existiert nicht                         | Domainname oder Nameserver-Information existiert nicht in der Datenbank. Eine Neuregistrierung ist erforderlich.                                               |
| 2304 | Objektstatus verhindert den Vorgang            | Der Domain-Status erlaubt keine Aktualisierung, die Domain ist für Aktualisierungen gesperrt. Der Status darf nicht "clientupdateprohibited" sein. Kann auch an anderen Statusbedingungen liegen. |

| CODE | DETAIL                                                                                                                               |
|------|--------------------------------------------------------------------------------------------------------------------------------------|
| 101  | Systemfehler, Details für ({0})!                                                                                                     |
| 102  | Mehrere Fehler, Details für ({0})!                                                                                                   |
| 103  | Unbekannte Fehler, Details für ({0})!                                                                                                |
| 200  | API-Befehl nicht gefunden ({0})!                                                                                                     |
| 210  | API-Dienst nicht gefunden ({0})!                                                                                                     |
| 211  | API-Dienstanbieter nicht festgelegt!                                                                                                 |
| 300  | Reseller nicht gefunden!                                                                                                             |
| 301  | Ihre aktuelle IP-Adresse ist nicht für den Zugriff berechtigt. Stellen Sie sicher, dass Sie sich von einer berechtigten IP-Adresse aus verbinden, und versuchen Sie es erneut |
| 310  | TLD wird nicht unterstützt!                                                                                                          |
| 320  | API nicht gefunden!                                                                                                                  |
| 321  | Währung wird nicht unterstützt!                                                                                                      |
| 330  | Erforderliche(r) Parameter nicht gesetzt ({0}).                                                                                      |
| –    | Stellen Sie sicher, dass Sie alle Kontaktobjekte vollständig senden                                                                  |
| 340  | Preisdefinition nicht gefunden ({0}[{1}]-{2}{3}).                                                                                    |
| 350  | Unzureichendes Reseller-Guthaben. (Reseller Id : {0} - Aktuelles Guthaben : {1} {2}).                                                |
| 350  | Die Buchungswährung stimmt nicht überein oder das Guthaben reicht nicht aus.                                                         |
| 360  | Ungültige API-Anfrage für das Feld ({0}).                                                                                            |
| 360  | API-Kontingent überschritten!                                                                                                        |
| 361  | Drosselungsfehler!                                                                                                                   |
| 362  | Die Premium-Domain kann derzeit nicht registriert werden.                                                                            |
| 363  | Vorgang abgebrochen, da sich die Domain im Zeitraum der automatischen Verlängerung befindet                                          |
| 364  | Diese Domain ist aufgrund eines Problems bei der Registry derzeit nicht für Transaktionen verfügbar                                  |

| CODE | DETAIL                                                                                      |
|------|---------------------------------------------------------------------------------------------|
| 400  | Ungültiger Kontakt ({0}).                                                                   |
| 401  | Kontaktinformationen können nicht synchronisiert werden.                                    |
| 402  | Kein Zugriff auf die Kontaktinformationen der Registry.                                     |
| 403  | IM SYSTEM FEHLEN INFORMATIONEN. GEBEN SIE IHRE STANDARDINFORMATIONEN EIN ODER WENDEN SIE SICH AN DAS SUPPORT-TEAM. |
| 404  | DOMAIN-KONTAKT KANN NICHT AKTUALISIERT WERDEN. BERECHTIGUNG DURCH DIE REGISTRY ERFORDERLICH. |
| 410  | Kontakt nicht gefunden.                                                                     |
| 410  | Kontakt nicht gefunden ({0}).                                                               |
| 420  | Ungültiger API-Befehl für den Kontakt {0}.                                                  |
| 430  | Kontakt-API nicht gefunden.                                                                 |
| 440  | Kontakt ist nicht synchronisiert.                                                           |
| 450  | Zu viele Domain-Kontakte!                                                                   |
| 451  | Die Kontaktaktualisierung konnte nicht durchgeführt werden.                                 |

| CODE | DETAIL                                                                                                                               |
|------|--------------------------------------------------------------------------------------------------------------------------------------|
| 500  | Ungültige Domain-ID.                                                                                                                 |
| 500  | Ungültige Domain-ID ({0}).                                                                                                           |
| 501  | Domain konnte nicht synchronisiert werden ({0})                                                                                      |
| 502  | Interner Transfer fehlgeschlagen                                                                                                     |
| 503  | Domain-Registrierung ist nicht verfügbar.                                                                                            |
| 504  | Die Domain-Informationen stimmen nicht überein. Vorher: {0}                                                                          |
| 505  | Geben Sie die IP-Adresse ein.                                                                                                        |
| 506  | Der Domain-Transfer wurde gestartet, aber die Kontaktinformationen konnten nicht gelesen werden.                                     |
| 507  | Autorisierungsfehler.                                                                                                                |
| 510  | Domain nicht gefunden.                                                                                                               |
| 511  | Abgelaufene Domains können nicht gefunden werden.                                                                                    |
| 512  | Domain ist nicht verlängerbar.                                                                                                       |
| 513  | Die Domain befindet sich nicht in einem aktualisierbaren Status. Sie muss aktiv sein, um aktualisiert werden zu können               |
| 514  | Redemption Period (Wiederherstellungsfrist) erwartet.                                                                                |
| 520  | Ungültiger API-Befehl für die Domain "{0}".                                                                                          |
| 530  | Ungültige Domain-Laufzeit. Die Laufzeit muss {0} bis {1} Jahre betragen. Angeforderte Laufzeit: {2} Jahre.                           |
| 540  | Die Domain kann nicht über {0} Jahre ab dem aktuellen Datum hinaus verlängert werden.                                                |
| 550  | Ungültiger Domainname. Die Domain muss {0} bis {1} Zeichen lang sein. Der angeforderte Domainname hat {2} Zeichen.                   |
| 560  | Ungültige Anzahl an Nameservern. Die Domain muss {0} bis {1} Nameserver haben. Angeforderte Anzahl an Nameservern: {2}.              |
| 561  | In der eingehenden Anfrage wurden keine Nameserver-Informationen gefunden                                                            |
| 570  | IDN wird für die TLD "{0}" nicht unterstützt.                                                                                        |
| 571  | Laufzeit ungültig.                                                                                                                   |
| 572  | Befehl ungültig.                                                                                                                     |
| 573  | Domainnamen nicht gefunden.                                                                                                          |
| 574  | TLD-Namen nicht gefunden.                                                                                                            |
| 575  | Die Domain befindet sich nicht in einem aktualisierbaren Status. Sie muss aktiv sein, damit die Nameserver aktualisiert werden können |
| 576  | Die Domain kann im letzten Monat vor dem Ablaufdatum verlängert werden                                                               |
| 580  | Transfer wird für die TLD "{0}" nicht unterstützt.                                                                                   |
| 581  | Child-Nameserver nicht gefunden                                                                                                      |
| 582  | Transfer wurde von einem anderen Reseller gestartet.                                                                                 |
| 583  | Transfer nicht eingeleitet. Bitte wenden Sie sich an das Support-Team.                                                               |
| 584  | Objektstatus verhindert den Vorgang                                                                                                  |
| 590  | Für diesen Transfer ist ein Auth-Code erforderlich.                                                                                  |
| 591  | Auth-Code ist ungültig.                                                                                                              |
| 592  | Auf der Domain besteht eine Transfersperre.                                                                                          |
| 593  | Die Domain kann nicht transferiert werden; Statusinformationen und Transfersperre müssen die erforderlichen Kriterien erfüllen. (Statusinformationen: #ok) |
| 594  | Die Nameserver-Adresse der Domain konnte nicht aufgelöst werden ({0})                                                                |
| 595  | Kontaktinformationen können nicht gelesen werden (whois.registrar.tld). Bitte stellen Sie sicher, dass der Datenschutzstatus offen ist   |
| 596  | Kontaktinformationen konnten nicht verifiziert werden                                                                                |
| 597  | TLD nicht gefunden                                                                                                                   |
| 598  | Ein unbekannter Fehler ist aufgetreten                                                                                               |
| 599  | Domain-Weiterleitung nicht gefunden                                                                                                  |

## Lizenz

Apache License 2.0 — siehe [LICENSE](LICENSE).
