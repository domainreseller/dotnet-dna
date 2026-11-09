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

# Libreria .NET DomainNameAPI (dotnet-dna)

Il client .NET ufficiale per l'API REST rivenditori di DomainNameAPI: ricerca domini, registrazione, rinnovo, trasferimenti, contatti, name server, child name server, zone DNS e inoltro URL.

dotnet-dna è un porting fedele di [php-dna](https://github.com/domainreseller/php-dna): gli stessi metodi, le stesse richieste inviate in rete e gli stessi valori di ritorno. È verificato con le fixture di conformità di php-dna (126 scenari), quindi il codice scritto per la libreria PHP si traduce in modo diretto, uno a uno.

## 📦 Download — usa sempre le Releases!

⬇️ **Scarica qui l'ultima versione testata: https://github.com/domainreseller/dotnet-dna/releases/latest**

> ⚠️ **Non** usare il pulsante verde **Code → Download ZIP**: scarica il branch di sviluppo grezzo. I pacchetti di release sono versionati, testati e pronti per la produzione.

## Guida all'Installazione e Integrazione

### Requisiti Minimi

- .NET 8 o superiore, **oppure** qualsiasi runtime che supporti .NET Standard 2.0 (.NET Framework 4.6.2+, .NET Core 2.0+, Mono, Xamarin, Unity).
- Nessuna dipendenza di terze parti. La libreria utilizza `HttpClient` e `System.Text.Json` (su .NET Standard 2.0 il pacchetto `System.Text.Json` viene incluso automaticamente).

### Installazione

**A) Pacchetto NuGet** — l'id del pacchetto è `DomainNameApi`. Ogni release include il file `.nupkg`; una volta che il pacchetto sarà pubblicato su NuGet.org, potrai anche installarlo direttamente:

```bash
dotnet add package DomainNameApi
```

Per installare un `.nupkg` scaricato, mettilo in una cartella e usa quella cartella come origine dei pacchetti:

```bash
dotnet add package DomainNameApi --source ./packages
```

**B) Riferimento al progetto** — scarica il codice sorgente della release e aggiungi un riferimento al progetto:

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

## 🔑 Credenziali API — Reseller ID e API Key

La libreria comunica con l'API REST di DomainNameAPI e richiede esattamente due credenziali, passate al costruttore:

| Parametro | Valore |
|---|---|
| Primo | Reseller ID — UUID nel formato `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| Secondo | API Key |

> 💡 Trovi il tuo **Reseller ID** e la tua **API Key** nel pannello DomainNameAPI, alla voce **Impostazioni API**.
> ⚠️ Si tratta di **credenziali API**: i dati di accesso al pannello **non** funzionano qui.

```csharp
using DomainNameApi;

var resellerId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
var apiKey     = "your-api-key";

// Live environment
var dna = new DomainNameApiClient(resellerId, apiKey);

// Test (OTE) environment
var dna = new DomainNameApiClient(resellerId, apiKey, true);
```

Gli spazi iniziali e finali di entrambe le credenziali vengono rimossi. Il flag della modalità test può essere passato anche come testo, nel modo in cui lo memorizzano i file di configurazione e i pannelli: `"true"`, `"1"`, `"on"` e `"yes"` (senza distinzione tra maiuscole e minuscole) selezionano l'ambiente di test, qualsiasi altro valore seleziona l'ambiente di produzione.

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey, Environment.GetEnvironmentVariable("DNA_TEST_MODE"));
```

## 🚀 Avvio Rapido

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

### Valori di ritorno

Ogni metodo restituisce la stessa struttura di php-dna, sotto forma di `System.Text.Json.Nodes.JsonNode` (di solito un `JsonObject`). Leggi i valori tramite cast o con `GetValue<T>()`, oppure stampa l'intero risultato con `ToJsonString()`:

```csharp
var details = dna.GetDetails("example.com");
string status = (string)details["data"]["Status"];          // "Active"
int    days   = (int)details["data"]["Dates"]["RemainingDays"];
bool   locked = (string)details["data"]["LockStatus"] == "true";   // flags are the strings "true"/"false"
Console.WriteLine(details.ToJsonString());
```

I nomi delle chiavi, i flag sotto forma di stringa (`"true"`/`"false"`) e gli importi formattati come stringa (`"11.9900"`) sono esattamente come in php-dna. La maggior parte dei metodi usa l'envelope `result` = `OK`/`ERROR`; `GetCurrentBalance` usa il vecchio envelope `OperationResult` e `CheckAvailability` restituisce una semplice lista in caso di successo.

Gli input come contatti, attributi TR e filtri delle liste sono `IDictionary<string, object>` con le chiavi di php-dna (`FirstName`, `EMail`, `AddressLine1`, `PhoneCountryCode`, `TRABISCITIZENID`, `PageSize`, ...). Le liste di name server possono essere una lista (`string[]`, `List<string>`) o un dizionario (in tal caso vengono usati i valori).

### Utilizzo

Tutti gli esempi seguenti possono essere eseguiti anche dalla cartella [examples](examples) (vedi [Esempi](#esempi)).

#### Registrazione Dominio

Nota: per i domini .tr sono richiesti parametri aggiuntivi. Il parametro `additionalAttributes` viene utilizzato per i nomi di dominio che richiedono informazioni supplementari, come i .tr.

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

I contatti vengono completati prima dell'invio: i campi mancanti ricevono valori segnaposto, cifre e punteggiatura vengono rimosse dai nomi, uno stato/provincia vuoto viene sostituito dalla città, i nomi dei paesi diventano codici ISO e i numeri di telefono vengono separati in numero e prefisso internazionale (vedi `ValidateContact`). Se non viene indicato alcun name server (o solo valori vuoti), vengono usati `ns1.domainnameapi.com` e `ns2.domainnameapi.com`. Per i domini .tr gli attributi TRABIS vengono ridotti a quelli accettati dal gateway (`TRABISDOMAINCATEGORY` = `Owner`/`Corporate`, `TRABISCITIZENID`, `TRABISTAXOFFICE`, `TRABISTAXNUMBER`); la vecchia chiave `TRABISCITIZIENID` e la categoria numerica vengono convertite automaticamente.

<details>
<summary>Output di Esempio per la Registrazione del Dominio</summary>

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

#### Rinnovo Dominio

```csharp
var result = dna.Renew("example.com", 1);
```

<details>
<summary>Output di Esempio per il Rinnovo del Dominio</summary>

```json
{ "result": "OK", "data": { "ExpirationDate": "2025-03-15T10:00:00" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Trasferimento Dominio

```csharp
var result = dna.Transfer("example.com", "Xy9#mK2$", 3);
```

> ℹ️ Come la facade pubblica di php-dna, `Transfer` invia una lista di contatti vuota; il parametro opzionale `contacts` esiste solo per mantenere la stessa firma.

<details>
<summary>Output di Esempio per il Trasferimento del Dominio</summary>

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

#### Annullare, Approvare o Rifiutare un Trasferimento

```csharp
dna.CancelTransfer("example.com");   // pending incoming transfer
dna.ApproveTransfer("example.com");  // pending outgoing transfer
dna.RejectTransfer("example.com");   // pending outgoing transfer
```

<details>
<summary>Output di Esempio per l'Annullamento del Trasferimento</summary>

```json
{ "result": "OK", "data": { "DomainName": "example.com", "Status": "Cancelled" } }
```

`ApproveTransfer` e `RejectTransfer` rispondono allo stesso modo, con `Approved` / `Rejected`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Verifica del Trasferimento

```csharp
var result = dna.CheckTransfer("example.com", "Ab1!xyZ9");
```

`result` è `OK` solo quando il registro indica che il dominio è trasferibile; altrimenti è `ERROR` con `Code` = `TRANSFER_NOT_AVAILABLE`, e i dettagli restano comunque disponibili in `data`.

<details>
<summary>Output di Esempio per la Verifica del Trasferimento</summary>

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

#### Elenco Domini

```csharp
var result = dna.GetList(new Dictionary<string, object>
{
    ["OrderColumn"]    = "Id",
    ["OrderDirection"] = "ASC",
    ["PageNumber"]     = 0,
    ["PageSize"]       = 1000,
});
```

`PageNumber` parte da zero. È possibile passare anche i nomi dei filtri propri del gateway (`MaxResultCount`, `SkipCount`, `Sorting`, `DomainName`, ...).

<details>
<summary>Output di Esempio per l'Elenco Domini</summary>

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

#### Elenco TLD

```csharp
var result = dna.GetTldList(100);
```

<details>
<summary>Output di Esempio per l'Elenco TLD</summary>

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

#### Verifica Disponibilità Dominio

```csharp
var result = dna.CheckAvailability(new[] { "hello", "world123x0" }, new[] { "com", "net" }, 1, "create");
```

<details>
<summary>Output di Esempio per la Verifica Disponibilità Dominio</summary>

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

In caso di errore il metodo restituisce il consueto envelope di errore anziché una lista, quindi verifica prima `result is JsonArray`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Dettagli Dominio

```csharp
var result = dna.GetDetails("example.com");
var synced = dna.SyncFromRegistry("example.com"); // same envelope
```

<details>
<summary>Output di Esempio per i Dettagli del Dominio</summary>

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

#### Modifica Name Server

```csharp
var result = dna.ModifyNameServer("example.com", new[] { "ns1.example.com", "ns2.example.com" });
```

Ai nomi host vengono rimossi gli spazi superflui e un eventuale punto finale, e vengono convertiti in minuscolo; le voci vuote e i duplicati vengono scartati.

<details>
<summary>Output di Esempio per la Modifica Name Server</summary>

```json
{ "result": "OK", "data": { "NameServers": [ "ns1.example.com", "ns2.example.com" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Attivare il Blocco del Dominio

```csharp
var result = dna.EnableTheftProtectionLock("example.com");
```

<details>
<summary>Output di Esempio per l'Attivazione del Blocco del Dominio</summary>

```json
{ "result": "OK", "data": { "LockStatus": true } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Disattivare il Blocco del Dominio

```csharp
var result = dna.DisableTheftProtectionLock("example.com");
```

<details>
<summary>Output di Esempio per la Disattivazione del Blocco del Dominio</summary>

```json
{ "result": "OK", "data": { "LockStatus": false } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Aggiungere Child Name Server

```csharp
var result = dna.AddChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>Output di Esempio per Aggiungere Child Name Server</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Eliminare Child Name Server

```csharp
var result = dna.DeleteChildNameServer("example.com", "ns1.example.com");
```

<details>
<summary>Output di Esempio per Eliminare Child Name Server</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Modificare Child Name Server

```csharp
var result = dna.ModifyChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>Output di Esempio per Modificare Child Name Server</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Modificare Protezione Privacy

```csharp
var result = dna.ModifyPrivacyProtectionStatus("example.com", true, "Owner request");
```

<details>
<summary>Output di Esempio per Modificare Protezione Privacy</summary>

```json
{ "data": { "PrivacyProtectionStatus": true }, "result": "OK" }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Ottenere i Contatti del Dominio

```csharp
var result = dna.GetContacts("example.com");
```

<details>
<summary>Output di Esempio per Ottenere i Contatti del Dominio</summary>

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

#### Salvare i Contatti del Dominio

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

I contatti possono usare anche la struttura annidata (`Address` → `Line1`/`City`/..., `Phone` → `Phone`/`Fax` → `Number`/`CountryCode`), cioè quella restituita da `GetContacts`.

<details>
<summary>Output di Esempio per Salvare i Contatti del Dominio</summary>

```json
{ "result": "OK", "data": { "contacts": [] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Ottenere Dettagli Rivenditore

```csharp
var result = dna.GetResellerDetails();
```

<details>
<summary>Output di Esempio per Ottenere Dettagli Rivenditore</summary>

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

#### Saldo Attuale

```csharp
var result = dna.GetCurrentBalance("USD"); // USD, TRY, EUR or GBP
```

<details>
<summary>Output di Esempio per il Saldo Attuale</summary>

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

In caso di errore `OperationResult` è `FAILED`, `ErrorCode` contiene il codice di errore (ad esempio `API_429`) e `Balance` è `"0.00"`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Gestione della Zona DNS

Il gateway memorizza una zona come **set** di record (nome + tipo → valori) e sostituisce un intero set a ogni scrittura. I metodi `*ResourceRecordSet` e `*ResourceRecordValue` se ne occupano al posto tuo. I nomi possono essere relativi (`www`), l'apex (`@`, `""` o `example.com`) o completamente qualificati (`www.example.com.`). Tipi supportati: `A`, `AAAA`, `ALIAS`, `CAA`, `CNAME`, `DS`, `MX`, `NAPTR`, `PTR`, `SRV`, `TLSA`, `TXT`. Le scritture vengono applicate immediatamente, a meno che tu non passi `apply: false` e chiami tu stesso `ApplyResourceRecords`. Le chiamate di zona consecutive sono distanziate di almeno un secondo, perché il gateway risponde alle raffiche di richieste con HTTP 429.

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
<summary>Output di Esempio per la Gestione della Zona DNS</summary>

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

Aggiungere un valore già presente, o rimuoverne uno assente, non modifica nulla e restituisce `{"result":"OK","data":{..., "Changed": false}}`. Un tipo non supportato viene rifiutato prima di effettuare qualsiasi chiamata:

```json
{ "result": "ERROR", "error": { "Code": "ADD_ZONE", "Message": "Record type \"NS\" is not supported by the DNS zone API. Supported types: A, AAAA, ALIAS, CAA, CNAME, DS, MX, NAPTR, PTR, SRV, TLSA, TXT.", "Details": "..." } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Inoltro URL

```csharp
dna.SetForwarding("example.com", "https://www.example.org", "Standard"); // or "Frame" (masked)
var forwarding = dna.GetForwarding("example.com");
dna.DeleteForwarding("example.com");
```

<details>
<summary>Output di Esempio per l'Inoltro URL</summary>

```json
{ "result": "OK", "data": { "Enabled": true, "RedirectAddress": "https://www.example.org/landing", "ForwardType": "Frame" } }
```

Se non è configurato alcun inoltro, `GetForwarding` restituisce `{"result":"OK","data":{"Enabled":false,"RedirectAddress":"","ForwardType":""}}`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Utilità (senza rete)

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
<summary>Output di Esempio per ValidateContact</summary>

```json
{
  "FirstName": "Jo", "Phone": "12025550100", "Country": "TR", "LastName": "Isimyok",
  "AddressLine1": "Addres yok", "City": "ISTANBUL", "State": "ISTANBUL", "ZipCode": "34000",
  "PhoneCountryCode": "90", "Fax": "", "FaxCountryCode": ""
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

### Gestione degli Errori

I metodi non generano mai eccezioni per problemi di API o di rete; restituiscono invece un envelope di errore:

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

| Situazione | `Code` | `Message` |
|---|---|---|
| Errore HTTP restituito dal gateway | `API_<status>`, ad es. `API_400`, `API_429`, `API_500` | il messaggio del gateway; con un corpo vuoto, `HTTP 429 - Too Many Requests` |
| Credenziali errate (HTTP 301/302) | `API_302` | `Invalid API credentials` |
| Errori di validazione | `API_400` | il messaggio del gateway; `Details` elenca i campi, ad es. `Admin PostalCode - 'PostalCode' is required` |
| Timeout | `API_28` | `Could not reach the DomainNameAPI service: the connection timed out. (...)` |
| Altri errori di rete (DNS, connessione, TLS, reset) | `API_6`, `API_7`, `API_60`, `API_56` | `Curl error during request: ...` |
| Risposta di successo vuota o inutilizzabile | `RESPONSE` | `Invalid response received from server! Response is empty.` |
| Controlli eseguiti prima di chiamare il gateway | ad es. `AVAILABILITY`, `ADD_ZONE`, `SET_FORWARD` | una descrizione del problema |

I codici di rete numerici seguono la numerazione di libcurl, in modo da corrispondere a quelli di php-dna. Solo gli errori di programmazione (ad esempio il passaggio di un tipo di oggetto non supportato) possono generare eccezioni.

Per la risoluzione dei problemi è possibile ispezionare l'ultima chiamata:

```csharp
dna.GetLastFunction();       // "request:GET domains/info"
dna.GetRequestData();        // { "url": ..., "method": ..., "payload": ... }
dna.GetResponseData();       // decoded response, or { "raw_response": "..." }
dna.GetResponseHeaders();    // { "url", "http_code", "content_type", "total_time", "headers" }
dna.GetParsedResponseData(); // parsed response or the error built from it
```

### Modalità Test

Passa `true` come terzo argomento del costruttore per usare l'ambiente di test OTE (`https://ote.domainresellerapi.com/api/v1`) invece di quello di produzione (`https://api.domainresellerapi.com/api/v1`). `GetServiceUrl()` mostra l'indirizzo in uso e `SetServiceUrl(url)` permette di sovrascriverlo.

### Segnalazione errori (telemetria)

Quando una chiamata API fallisce, la libreria può inviare una segnalazione di errore al sistema di tracciamento errori di DomainNameAPI (Sentry). Una piccola quota casuale delle chiamate riuscite (4%, più ogni chiamata che dura oltre 1 secondo) può essere inviata come record di prestazioni. Questo aiuta DomainNameAPI a individuare e risolvere i problemi. La segnalazione errori è **attiva per impostazione predefinita**.

> ℹ️ Nella versione 1.0.0 l'indirizzo Sentry (`DefaultSentryDsn`) è ancora vuoto, perché il progetto Sentry per la libreria .NET non è ancora stato creato. Finché una release non lo imposta, **non viene inviato nulla**. Il comportamento descritto di seguito indica cosa verrà inviato una volta impostato.

**Cosa viene inviato:**

- codice e messaggio di errore
- percorso dell'endpoint chiamato e metodo HTTP
- durata della chiamata
- nome dell'applicazione (`DomainNameAPI-DotNet`), versione della libreria e versione di .NET
- ID rivenditore e nome di dominio
- l'indirizzo IP pubblico del server (rilevato tramite `api.ipify.org`, in background)
- il nome dell'account del sistema operativo con cui viene eseguita l'applicazione
- la richiesta e la risposta API, inclusi i dati di contatto del dominio; vengono mascherate solo le credenziali — codici EPP/auth, chiavi API, password e token vengono sostituiti con `***`

**Cosa non viene mai inviato:** codici EPP/auth, chiavi API, password o token in forma leggibile; gli argomenti dei metodi negli stack trace.

**Perché e per quanto tempo:** i dati vengono usati esclusivamente per misurare l'utilizzo e diagnosticare tempestivamente i problemi, e sono conservati per 30 giorni sul server Sentry di DomainNameAPI, dopodiché vengono eliminati automaticamente.

Le segnalazioni vengono inviate in background: non bloccano né rallentano mai la tua chiamata e non generano mai eccezioni. Gli errori attesi e frequenti (dominio non trovato, saldo insufficiente, credenziali non valide, ...) non vengono segnalati.

**Come disattivarla:** chiama `SetErrorReportingEnabled(false)` dopo aver creato il client:

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey);
dna.SetErrorReportingEnabled(false);
```

**Modalità minima:** `SetTelemetryMinimal(true)` mantiene attiva la segnalazione ma invia solo codice e messaggio di errore, endpoint, tempi, versioni e ID rivenditore — nessun dato di richiesta/risposta, dominio, indirizzo IP o nome account.

La destinazione può essere modificata con la proprietà `TelemetryDsn` (ad esempio verso il tuo progetto Sentry).

> 🔒 **Sicurezza:** la verifica dei certificati TLS è attiva. Su un server con un bundle CA obsoleto può essere disattivata con `dna.SetSslVerification(false);` — sconsigliato; la soluzione migliore è aggiornare il bundle CA.

### Avanzate

- **Timeout:** 30 secondi per richiesta e 10 secondi per la connessione; modificali con le proprietà `Timeout` e `ConnectTimeout`.
- **Transport personalizzato:** passa un `IHttpTransport` al costruttore (`new DomainNameApiClient(id, key, false, transport)`) o imposta la proprietà `Transport` per instradare le richieste attraverso il tuo stack HTTP, un proxy o un test double. Le implementazioni restituiscono ogni stato HTTP (i redirect non vengono seguiti) e generano `TransportException` in caso di errori di rete.
- **Thread:** un client conserva l'ultima richiesta e l'ultima risposta a fini diagnostici e non è thread-safe. Usa un'istanza per thread (o per richiesta) oppure sincronizza l'accesso. `Dispose()` rilascia l'`HttpClient` sottostante.
- **User-Agent:** le richieste vengono inviate come `DomainNameAPI-DotNet/1.0.0`.

### Esempi

Il progetto [examples](examples/DomainNameApi.Examples) contiene un file eseguibile per ogni metodo (`GetDetails`, `RegisterWithContactInfo`, `RegisterWithContactInfo-TR`, i metodi per la zona DNS e l'inoltro, ...). Imposta le tue credenziali ed eseguine uno:

```bash
export DNA_RESELLER_ID=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
export DNA_API_KEY=your-api-key
export DNA_TEST_MODE=true   # optional: use the OTE test environment

dotnet run --project examples/DomainNameApi.Examples -- GetDetails
dotnet run --project examples/DomainNameApi.Examples            # lists all examples
```

### Test

I test non richiedono credenziali e non effettuano chiamate di rete esterne:

```bash
dotnet test
```

Senza un SDK .NET locale, eseguili in Docker:

```bash
docker run --rm -v "$PWD":/src -w /src mcr.microsoft.com/dotnet/sdk:8.0 dotnet test
```

| File di test | Descrizione |
|-----------|-------------|
| `tests/DomainNameApi.Tests/ConformanceTests.cs` | Riesegue tutte le 126 fixture di conformità di php-dna (`testdata/conformance/`) e confronta richieste e risultati |
| `tests/DomainNameApi.Tests/HttpLayerTests.cs` | Header, URL, query string, modalità test, timeout, cadenzamento delle chiamate di zona |
| `tests/DomainNameApi.Tests/HttpClientTransportTests.cs` | Il transport `HttpClient` predefinito contro un listener locale |
| `tests/DomainNameApi.Tests/ErrorMappingTests.cs` | Mappatura degli errori del gateway e messaggi di validazione |
| `tests/DomainNameApi.Tests/TelemetryTests.cs` | Payload di telemetria, mascheramento delle credenziali, modalità minima |
| `tests/DomainNameApi.Tests/UtilityTests.cs` | Validazione dei contatti, normalizzazione di telefono/paese, formattazione compatibile con PHP |

<hr style="border: 4px solid #000; border-style: dashed;">

## Codici di Ritorno ed Errore con Descrizioni

| Codice | Descrizione                                    | Dettaglio                                                                                                                                                      |
|------|------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1000 | Comando completato con successo | Operazione riuscita. |
| 1001 | Comando completato con successo; azione in sospeso | Operazione riuscita. Tuttavia, l'operazione è attualmente in coda per il completamento. |
| 2003 | Parametro obbligatorio mancante | Errore di parametro mancante. Ad esempio, nessun numero di telefono nelle informazioni di contatto. |
| 2105 | L'oggetto non è idoneo al rinnovo | Lo stato del dominio non consente il rinnovo, è bloccato per le operazioni di aggiornamento. Lo stato non deve essere "clientupdateprohibited". Può dipendere da altre condizioni di stato. |
| 2200 | Errore di autenticazione | Errore di autorizzazione, il codice di sicurezza è errato oppure il dominio è registrato presso un altro registrar. |
| 2302 | L'oggetto esiste | Il nome di dominio o le informazioni del name server esistono già nel database. Non può essere registrato. |
| 2303 | L'oggetto non esiste | Il nome di dominio o le informazioni del name server non esistono nel database. È necessaria una nuova registrazione. |
| 2304 | Lo stato dell'oggetto non consente l'operazione | Lo stato del dominio non consente aggiornamenti, è bloccato per le operazioni di aggiornamento. Lo stato non deve essere "clientupdateprohibited". Può dipendere da altre condizioni di stato. |

| CODICE | DETTAGLIO                                                                                                                            |
|------|--------------------------------------------------------------------------------------------------------------------------------------|
| 101  | Errore di sistema dettagliato per ({0})!                                                                                             |
| 102  | Errori multipli dettagliati per ({0})!                                                                                               |
| 103  | Errori sconosciuti dettagliati per ({0})!                                                                                            |
| 200  | Comando API non trovato ({0})!                                                                                                       |
| 210  | Servizio API non trovato ({0})!                                                                                                      |
| 211  | Provider del servizio API non impostato!                                                                                             |
| 300  | Rivenditore non trovato!                                                                                                             |
| 301  | Il tuo indirizzo IP attuale non è autorizzato ad accedere. Assicurati di connetterti da un indirizzo IP autorizzato e riprova         |
| 310  | TLD non supportato!                                                                                                                  |
| 320  | API non trovata!                                                                                                                     |
| 321  | Valuta non supportata!                                                                                                               |
| 330  | Parametro/i obbligatorio/i non impostato/i ({0}).                                                                                    |
| –    | Assicurati di inviare tutti gli oggetti di contatto completi                                                                         |
| 340  | Definizione di prezzo non trovata ({0}[{1}]-{2}{3}).                                                                                 |
| 350  | Saldo rivenditore insufficiente. (Reseller Id : {0} - Saldo attuale : {1} {2}).                                                      |
| 350  | La valuta contabile non corrisponde o il saldo non è sufficiente.                                                                    |
| 360  | Richiesta API non valida per il campo ({0}).                                                                                         |
| 360  | Quota API superata!                                                                                                                  |
| 361  | Errore di limitazione (throttling)!                                                                                                  |
| 362  | Il dominio premium non è al momento disponibile per la registrazione.                                                                |
| 363  | Operazione annullata perché il dominio è nel periodo di rinnovo automatico                                                           |
| 364  | Questo dominio non è al momento disponibile per operazioni a causa di un problema con il registro                                    |

| CODICE | DETTAGLIO                                                                                   |
|------|---------------------------------------------------------------------------------------------|
| 400  | Contatto non valido ({0}).                                                                  |
| 401  | Impossibile sincronizzare le informazioni di contatto.                                      |
| 402  | Nessun accesso alle informazioni di contatto dal registro.                                  |
| 403  | NEL SISTEMA MANCANO DELLE INFORMAZIONI. INSERISCI LE TUE INFORMAZIONI PREDEFINITE O CONTATTA IL TEAM DI SUPPORTO. |
| 404  | IMPOSSIBILE AGGIORNARE IL CONTATTO DEL DOMINIO. È NECESSARIA L'AUTORIZZAZIONE DEL REGISTRO. |
| 410  | Contatto non trovato.                                                                       |
| 410  | Contatto non trovato ({0}).                                                                 |
| 420  | Comando API non valido per il contatto {0}.                                                 |
| 430  | API dei contatti non trovata.                                                               |
| 440  | Il contatto non è sincronizzato.                                                            |
| 450  | Troppi contatti di dominio!                                                                 |
| 451  | Impossibile procedere con l'aggiornamento del contatto.                                     |

| CODICE | DETTAGLIO                                                                                                                            |
|------|--------------------------------------------------------------------------------------------------------------------------------------|
| 500  | ID dominio non valido.                                                                                                               |
| 500  | ID dominio non valido ({0}).                                                                                                         |
| 501  | Impossibile sincronizzare il dominio ({0})                                                                                           |
| 502  | Trasferimento interno non riuscito                                                                                                   |
| 503  | La registrazione del dominio non è disponibile.                                                                                      |
| 504  | Le informazioni del dominio non corrispondono. Prima: {0}                                                                            |
| 505  | Inserisci l'indirizzo IP.                                                                                                            |
| 506  | Il trasferimento del dominio è stato avviato, ma non è stato possibile leggere le informazioni di contatto.                         |
| 507  | Errore di autorizzazione.                                                                                                            |
| 510  | Dominio non trovato.                                                                                                                 |
| 511  | I domini scaduti non possono essere trovati.                                                                                         |
| 512  | Il dominio non è rinnovabile.                                                                                                        |
| 513  | Il dominio non è in uno stato aggiornabile. Deve essere attivo per poter essere aggiornato                                           |
| 514  | Periodo di redemption previsto.                                                                                                      |
| 520  | Comando API non valido per il dominio "{0}".                                                                                         |
| 530  | Periodo del dominio non valido. Il periodo deve essere compreso tra {0} e {1} anni. Il periodo richiesto è di {2} anni.               |
| 540  | Il dominio non può essere esteso oltre {0} anni dalla data attuale.                                                                  |
| 550  | Nome di dominio non valido. Il dominio deve avere una lunghezza compresa tra {0} e {1} caratteri. Il nome di dominio richiesto ha {2} caratteri. |
| 560  | Numero di name server non valido. Il dominio deve avere da {0} a {1} name server. Il numero di name server richiesto è {2}.          |
| 561  | Nessuna informazione sui name server trovata nella richiesta ricevuta                                                                |
| 570  | IDN non supportato per il TLD "{0}".                                                                                                 |
| 571  | Periodo non valido.                                                                                                                  |
| 572  | Comando non valido.                                                                                                                  |
| 573  | Nomi di dominio non trovati.                                                                                                         |
| 574  | Nomi TLD non trovati.                                                                                                                |
| 575  | Il dominio non è in uno stato aggiornabile. Deve essere attivo perché i name server possano essere aggiornati                        |
| 576  | Il dominio può essere rinnovato solo nell'ultimo mese prima della data di scadenza                                                   |
| 580  | Trasferimento non supportato per il TLD "{0}".                                                                                       |
| 581  | Child name server non trovato                                                                                                        |
| 582  | Trasferimento avviato da un altro rivenditore.                                                                                       |
| 583  | Trasferimento non inizializzato. Contatta il team di supporto.                                                                       |
| 584  | Lo stato dell'oggetto non consente l'operazione                                                                                      |
| 590  | Per questo trasferimento è richiesto il codice di autorizzazione.                                                                    |
| 591  | Il codice di autorizzazione non è valido.                                                                                            |
| 592  | Sul dominio è presente un blocco di trasferimento.                                                                                   |
| 593  | Il dominio non può essere trasferito: le informazioni di stato e il blocco di trasferimento devono rispettare i criteri richiesti. (Informazioni di stato: #ok) |
| 594  | Impossibile risolvere l'indirizzo del name server del dominio ({0})                                                                  |
| 595  | Impossibile leggere le informazioni di contatto (whois.registrar.tld). Assicurati che lo stato di protezione della privacy sia aperto |
| 596  | Impossibile verificare le informazioni di contatto                                                                                   |
| 597  | TLD non trovato                                                                                                                      |
| 598  | Si è verificato un errore sconosciuto                                                                                                |
| 599  | Inoltro del dominio non trovato                                                                                                      |

## Licenza

Apache License 2.0 — vedi [LICENSE](LICENSE).
