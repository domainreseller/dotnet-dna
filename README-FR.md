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

# Bibliothèque .NET DomainNameAPI (dotnet-dna)

Le client .NET officiel de l'API REST revendeur de DomainNameAPI : recherche de domaines, enregistrement, renouvellement, transferts, contacts, serveurs de noms, serveurs de noms enfants, zones DNS et redirection d'URL.

dotnet-dna est un portage fidèle de [php-dna](https://github.com/domainreseller/php-dna) : les mêmes méthodes, les mêmes requêtes sur le réseau et les mêmes valeurs de retour. Il est vérifié à l'aide des fixtures de conformité de php-dna (134 scénarios), de sorte que le code écrit pour la bibliothèque PHP se transpose à l'identique.

## 📦 Téléchargement — utilisez toujours les Releases !

⬇️ **Téléchargez la dernière version testée ici : https://github.com/domainreseller/dotnet-dna/releases/latest**

> ⚠️ N'utilisez **pas** le bouton vert **Code → Download ZIP** — il télécharge la branche de développement brute. Les paquets de release sont versionnés, testés et prêts pour la production.

## Guide d'Installation et d'Intégration

### Exigences Minimales

- .NET 8 ou supérieur, **ou** tout runtime prenant en charge .NET Standard 2.0 (.NET Framework 4.6.2+, .NET Core 2.0+, Mono, Xamarin, Unity).
- Aucune dépendance tierce. La bibliothèque utilise `HttpClient` et `System.Text.Json` (sous .NET Standard 2.0, le paquet `System.Text.Json` est ajouté automatiquement).

### Installation

**A) Paquet NuGet** — l'identifiant du paquet est `DomainNameApi`. Chaque release fournit le fichier `.nupkg` ; dès que le paquet est publié sur NuGet.org, vous pouvez aussi l'installer directement :

```bash
dotnet add package DomainNameApi
```

Pour installer un `.nupkg` téléchargé, placez-le dans un dossier et utilisez ce dossier comme source de paquets :

```bash
dotnet add package DomainNameApi --source ./packages
```

**B) Référence de projet** — téléchargez le code source de la release et référencez le projet :

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

## 🔑 Identifiants API — Reseller ID et API Key

La bibliothèque communique avec l'API REST DomainNameAPI et n'a besoin que de deux identifiants, transmis au constructeur :

| Paramètre | Valeur |
|---|---|
| Premier | Reseller ID — UUID de la forme `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| Second | API Key |

> 💡 Vous trouverez votre **Reseller ID** et votre **API Key** dans votre panel DomainNameAPI, sous **Paramètres API**.
> ⚠️ Ce sont des **identifiants API** — vos identifiants de connexion au panel **ne fonctionneront pas** ici.

```csharp
using DomainNameApi;

var resellerId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
var apiKey     = "your-api-key";

// Live environment
var dna = new DomainNameApiClient(resellerId, apiKey);

// Test (OTE) environment
var dna = new DomainNameApiClient(resellerId, apiKey, true);
```

Les espaces en début et en fin des deux identifiants sont supprimés. L'indicateur de mode test peut aussi être transmis sous forme de texte, tel que les fichiers de configuration et les panels le stockent : `"true"`, `"1"`, `"on"` et `"yes"` (quelle que soit la casse) sélectionnent l'environnement de test, toute autre valeur sélectionne la production.

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey, Environment.GetEnvironmentVariable("DNA_TEST_MODE"));
```

## 🚀 Démarrage Rapide

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

### Valeurs de retour

Chaque méthode renvoie la même structure que php-dna, sous la forme d'un `System.Text.Json.Nodes.JsonNode` (généralement un `JsonObject`). Lisez les valeurs par conversion de type (cast) ou avec `GetValue<T>()`, ou affichez le résultat complet avec `ToJsonString()` :

```csharp
var details = dna.GetDetails("example.com");
string status = (string)details["data"]["Status"];          // "Active"
int    days   = (int)details["data"]["Dates"]["RemainingDays"];
bool   locked = (string)details["data"]["LockStatus"] == "true";   // flags are the strings "true"/"false"
Console.WriteLine(details.ToJsonString());
```

Les noms de clés, les indicateurs sous forme de chaîne (`"true"`/`"false"`) et les montants formatés en chaîne (`"11.9900"`) sont exactement identiques à ceux de php-dna. La plupart des méthodes utilisent l'enveloppe `result` = `OK`/`ERROR` ; `GetCurrentBalance` utilise l'ancienne enveloppe `OperationResult` et `CheckAvailability` renvoie une simple liste en cas de succès.

Les entrées telles que les contacts, les attributs TR et les filtres de liste sont des `IDictionary<string, object>` avec les clés de php-dna (`FirstName`, `EMail`, `AddressLine1`, `PhoneCountryCode`, `TRABISCITIZENID`, `PageSize`, ...). Les listes de serveurs de noms peuvent être une liste (`string[]`, `List<string>`) ou un dictionnaire (ce sont alors les valeurs qui sont utilisées).

### Utilisation

Tous les exemples ci-dessous peuvent également être exécutés depuis le dossier [examples](examples) (voir [Exemples](#exemples)).

#### Enregistrement de Domaine

Remarque : des paramètres supplémentaires sont requis pour les domaines .tr. Le paramètre `additionalAttributes` est utilisé pour les noms de domaine qui exigent des informations complémentaires, comme les .tr.

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

Les contacts sont complétés avant l'envoi : les champs manquants reçoivent des valeurs par défaut, les chiffres et la ponctuation sont retirés des noms, un État absent ou vide est remplacé par la ville, les noms de pays sont convertis en codes ISO et les numéros de téléphone sont séparés en numéro et indicatif pays — un numéro écrit avec son propre indicatif `+` ou `00` conserve cet indicatif (voir `ValidateContact`). Si aucun serveur de noms n'est fourni (ou seulement des valeurs vides), `ns1.domainnameapi.com` et `ns2.domainnameapi.com` sont utilisés. Pour les domaines .tr, les attributs TRABIS sont réduits à ceux que la passerelle accepte (`TRABISDOMAINCATEGORY` = `Owner`/`Corporate`, `TRABISCITIZENID`, `TRABISTAXOFFICE`, `TRABISTAXNUMBER`) ; l'ancienne clé `TRABISCITIZIENID` et la catégorie numérique sont converties automatiquement.

<details>
<summary>Exemple de Sortie pour l'Enregistrement de Domaine</summary>

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

Si la passerelle refuse la commande (elle répond `"success": false`), la méthode renvoie une enveloppe d'erreur avec `Code` = `REGISTER_DOMAIN` et le message de la passerelle, de sorte qu'un enregistrement refusé n'est jamais signalé comme effectué. `Renew` (`RENEW_DOMAIN`) et `Transfer` (`TRANSFER_DOMAIN`) se comportent de la même manière.

<hr style="border: 4px solid #000; border-style: dashed;">

#### Renouvellement de Domaine

```csharp
var result = dna.Renew("example.com", 1);
```

<details>
<summary>Exemple de Sortie pour le Renouvellement de Domaine</summary>

```json
{ "result": "OK", "data": { "ExpirationDate": "2025-03-15T10:00:00" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Transfert de Domaine

```csharp
var result = dna.Transfer("example.com", "Xy9#mK2$", 3);
```

> ℹ️ Un transfert n'envoie jamais de contacts : la requête contient toujours une liste de contacts vide et le registraire reprend lui-même les contacts. `Transfer` n'a donc pas de paramètre de contacts ; si nécessaire, mettez à jour les contacts avec `SaveContacts` une fois le transfert terminé.

<details>
<summary>Exemple de Sortie pour le Transfert de Domaine</summary>

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

#### Annuler, Approuver ou Rejeter un Transfert

```csharp
dna.CancelTransfer("example.com");   // pending incoming transfer
dna.ApproveTransfer("example.com");  // pending outgoing transfer
dna.RejectTransfer("example.com");   // pending outgoing transfer
```

<details>
<summary>Exemple de Sortie pour l'Annulation d'un Transfert</summary>

```json
{ "result": "OK", "data": { "DomainName": "example.com", "Status": "Cancelled" } }
```

`ApproveTransfer` et `RejectTransfer` répondent de la même manière, avec `Approved` / `Rejected`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Vérification de Transfert

```csharp
var result = dna.CheckTransfer("example.com", "Ab1!xyZ9");
```

`result` vaut `OK` uniquement lorsque le registre indique que le domaine est transférable ; sinon il vaut `ERROR` avec `Code` = `TRANSFER_NOT_AVAILABLE`, et les détails restent disponibles sous `data`.

<details>
<summary>Exemple de Sortie pour la Vérification de Transfert</summary>

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

#### Liste des Domaines

```csharp
var result = dna.GetList(new Dictionary<string, object>
{
    ["OrderColumn"]    = "Id",
    ["OrderDirection"] = "ASC",
    ["PageNumber"]     = 0,
    ["PageSize"]       = 1000,
});
```

`PageNumber` commence à zéro. Les noms de filtres propres à la passerelle (`MaxResultCount`, `SkipCount`, `Sorting`, `DomainName`, ...) peuvent également être transmis.

<details>
<summary>Exemple de Sortie pour la Liste des Domaines</summary>

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

#### Liste des TLD

```csharp
var result = dna.GetTldList(100);
```

<details>
<summary>Exemple de Sortie pour la Liste des TLD</summary>

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

#### Vérification de Disponibilité de Domaine

```csharp
var result = dna.CheckAvailability(new[] { "hello", "world123x0" }, new[] { "com", "net" }, 1, "create");
```

<details>
<summary>Exemple de Sortie pour la Vérification de Disponibilité de Domaine</summary>

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

En cas d'échec, la méthode renvoie l'enveloppe d'erreur habituelle au lieu d'une liste ; vérifiez donc d'abord `result is JsonArray`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Détails du Domaine

```csharp
var result = dna.GetDetails("example.com");
var synced = dna.SyncFromRegistry("example.com"); // same envelope
```

<details>
<summary>Exemple de Sortie pour les Détails du Domaine</summary>

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

#### Modification des Serveurs de Noms

```csharp
var result = dna.ModifyNameServer("example.com", new[] { "ns1.example.com", "ns2.example.com" });
```

Les noms d'hôte sont débarrassés des espaces superflus, convertis en minuscules et privés d'un éventuel point final ; les entrées vides et les doublons sont ignorés.

<details>
<summary>Exemple de Sortie pour la Modification des Serveurs de Noms</summary>

```json
{ "result": "OK", "data": { "NameServers": [ "ns1.example.com", "ns2.example.com" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Activer le Verrouillage du Domaine

```csharp
var result = dna.EnableTheftProtectionLock("example.com");
```

<details>
<summary>Exemple de Sortie pour l'Activation du Verrouillage du Domaine</summary>

```json
{ "result": "OK", "data": { "LockStatus": true } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Désactiver le Verrouillage du Domaine

```csharp
var result = dna.DisableTheftProtectionLock("example.com");
```

<details>
<summary>Exemple de Sortie pour la Désactivation du Verrouillage du Domaine</summary>

```json
{ "result": "OK", "data": { "LockStatus": false } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Ajouter un Serveur de Noms Enfant

```csharp
var result = dna.AddChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>Exemple de Sortie pour Ajouter un Serveur de Noms Enfant</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Supprimer un Serveur de Noms Enfant

```csharp
var result = dna.DeleteChildNameServer("example.com", "ns1.example.com");
```

<details>
<summary>Exemple de Sortie pour Supprimer un Serveur de Noms Enfant</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Modifier un Serveur de Noms Enfant

```csharp
var result = dna.ModifyChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>Exemple de Sortie pour Modifier un Serveur de Noms Enfant</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Modifier la Protection de la Vie Privée

```csharp
var result = dna.ModifyPrivacyProtectionStatus("example.com", true);

// The flag may also be text, as panels store it: "1", "true", "on" and "yes" enable privacy
var result2 = dna.ModifyPrivacyProtectionStatus("example.com", "on");
```

Le troisième argument optionnel (`reason`) n'est conservé que pour la compatibilité ; la passerelle ne possède pas ce champ et il n'est pas envoyé.

<details>
<summary>Exemple de Sortie pour Modifier la Protection de la Vie Privée</summary>

```json
{ "data": { "PrivacyProtectionStatus": true }, "result": "OK" }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Obtenir les Contacts du Domaine

```csharp
var result = dna.GetContacts("example.com");
```

<details>
<summary>Exemple de Sortie pour Obtenir les Contacts du Domaine</summary>

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

#### Enregistrer les Contacts du Domaine

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

Les contacts peuvent aussi utiliser la forme imbriquée (`Address` → `Line1`/`City`/..., `Phone` → `Phone`/`Fax` → `Number`/`CountryCode`), celle que renvoie `GetContacts`.

<details>
<summary>Exemple de Sortie pour Enregistrer les Contacts du Domaine</summary>

```json
{ "result": "OK", "data": { "contacts": [] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Obtenir les Détails du Revendeur

```csharp
var result = dna.GetResellerDetails();
```

<details>
<summary>Exemple de Sortie pour Obtenir les Détails du Revendeur</summary>

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

#### Solde Actuel

```csharp
var result = dna.GetCurrentBalance("USD"); // USD, TRY, EUR or GBP
```

<details>
<summary>Exemple de Sortie pour le Solde Actuel</summary>

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

En cas d'échec, `OperationResult` vaut `FAILED`, `ErrorCode` contient le code d'erreur (par exemple `API_429`) et `Balance` vaut `"0.00"`. Le compte ne communique que les soldes en USD et en TRY ; demander une autre devise (EUR, GBP, ...) échoue avec `ErrorCode` = `BALANCE` et le message `No EUR balance is reported for this account. Available: USD, TRY.`

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Gestion de la Zone DNS

La passerelle stocke une zone sous forme d'**ensembles** d'enregistrements (nom + type → valeurs) et remplace un ensemble entier à chaque écriture. Les méthodes `*ResourceRecordSet` et `*ResourceRecordValue` s'en chargent pour vous. Les noms peuvent être relatifs (`www`), désigner l'apex (`@`, `""` ou `example.com`) ou être pleinement qualifiés (`www.example.com.`). Types pris en charge : `A`, `AAAA`, `ALIAS`, `CAA`, `CNAME`, `DS`, `MX`, `NAPTR`, `PTR`, `SRV`, `TLSA`, `TXT`. Les écritures sont validées immédiatement, sauf si vous passez `apply: false` et appelez vous-même `ApplyResourceRecords` ; si cette application automatique échoue, la méthode renvoie l'erreur d'application (la modification reste en attente et n'est pas mise en ligne). Les appels de zone consécutifs sont espacés d'au moins une seconde, car la passerelle répond aux rafales de requêtes par un HTTP 429.

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
<summary>Exemple de Sortie pour la Gestion de la Zone DNS</summary>

`GetResourceRecords` :

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

`SetResourceRecordSet` / `AddResourceRecordValue` (le type est renvoyé en majuscules et le TTL tel qu'envoyé, limité à 1–86400 secondes) :

```json
{ "result": "OK", "data": { "Name": "", "Type": "MX", "TTL": 3600, "Contents": [ "10 mx1.example.com", "20 mx2.example.com" ] } }
```

Ajouter une valeur déjà présente, ou supprimer une valeur absente, ne modifie rien et renvoie `{"result":"OK","data":{..., "Changed": false}}`. Un type non pris en charge est refusé avant tout appel :

```json
{ "result": "ERROR", "error": { "Code": "ADD_ZONE", "Message": "Record type \"NS\" is not supported by the DNS zone API. Supported types: A, AAAA, ALIAS, CAA, CNAME, DS, MX, NAPTR, PTR, SRV, TLSA, TXT.", "Details": "..." } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Redirection d'URL

```csharp
dna.SetForwarding("example.com", "https://www.example.org", "Standard"); // or "Frame" (masked)
var forwarding = dna.GetForwarding("example.com");
dna.DeleteForwarding("example.com");
```

<details>
<summary>Exemple de Sortie pour la Redirection d'URL</summary>

```json
{ "result": "OK", "data": { "Enabled": true, "RedirectAddress": "https://www.example.org/landing", "ForwardType": "Frame" } }
```

Lorsqu'aucune redirection n'est configurée (la passerelle répond `Redirect information not found`), `GetForwarding` renvoie `{"result":"OK","data":{"Enabled":false,"RedirectAddress":"","ForwardType":""}}`. Toute autre erreur — par exemple un domaine inexistant — est renvoyée comme une erreur.

`DeleteForwarding` renvoie `{"result":"OK","data":{"DomainName":"example.com","Removed":true}}` ; si rien n'était configuré, l'appel n'a aucun effet et répond `"Removed": false`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Utilitaires (sans réseau)

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
<summary>Exemple de Sortie pour ValidateContact</summary>

```json
{
  "FirstName": "Jo", "Phone": "12025550100", "Country": "TR", "LastName": "Isimyok",
  "AddressLine1": "Addres yok", "City": "ISTANBUL", "State": "ISTANBUL", "ZipCode": "34000",
  "PhoneCountryCode": "90", "Fax": "", "FaxCountryCode": ""
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

### Gestion des Erreurs

Les méthodes ne lèvent jamais d'exception pour les problèmes d'API ou de réseau ; elles renvoient une enveloppe d'erreur :

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
| Erreur HTTP renvoyée par la passerelle | `API_<status>`, par ex. `API_400`, `API_429`, `API_500` | le message de la passerelle ; pour un corps vide, `HTTP 429 - Too Many Requests` |
| Identifiants incorrects (HTTP 301/302) | `API_302` | `Invalid API credentials` |
| Erreurs de validation | `API_400` | le message de la passerelle ; `Details` liste les champs, par ex. `Admin PostalCode - 'PostalCode' is required` |
| Délai d'attente dépassé | `CURL_28` | `Could not reach the DomainNameAPI service: the connection timed out. (...)` |
| Autres défaillances réseau (DNS, connexion, TLS, réinitialisation) | `CURL_6`, `CURL_7`, `CURL_60`, `CURL_56` | `Curl error during request: ...` |
| Enregistrement, renouvellement ou transfert refusé (`"success": false`) | `REGISTER_DOMAIN`, `RENEW_DOMAIN`, `TRANSFER_DOMAIN` | le message de la passerelle |
| Réponse de succès vide ou inexploitable | `RESPONSE` | `Invalid response received from server! Response is empty.` |
| Vérifications effectuées avant l'appel à la passerelle | par ex. `AVAILABILITY`, `ADD_ZONE`, `SET_FORWARD` | une description du problème |

Les codes `API_<n>` désignent toujours un statut HTTP. Lorsque la passerelle a envoyé son propre code d'erreur, l'enveloppe le contient également sous `GatewayCode` (par exemple `Dna.DomainService:Domain:10007`), ce qui permet de distinguer « domaine introuvable » de « TLD non pris en charge » sans analyser le message. Les défaillances réseau utilisent `CURL_<n>`, dont le numéro suit la numérotation de libcurl afin que les codes correspondent à ceux de php-dna. Seules les erreurs de programmation (par exemple le passage d'un type d'objet non pris en charge) peuvent lever une exception.

Pour le dépannage, le dernier appel peut être inspecté :

```csharp
dna.GetLastFunction();       // "request:GET domains/info"
dna.GetRequestData();        // { "url": ..., "method": ..., "payload": ... }
dna.GetResponseData();       // decoded response, or { "raw_response": "..." }
dna.GetResponseHeaders();    // { "url", "http_code", "content_type", "total_time", "headers" }
dna.GetParsedResponseData(); // parsed response or the error built from it
```

### Mode Test

Passez `true` comme troisième argument du constructeur pour utiliser l'environnement de test OTE (`https://ote.domainresellerapi.com/api/v1`) au lieu de la production (`https://api.domainresellerapi.com/api/v1`). `GetServiceUrl()` indique l'adresse utilisée et `SetServiceUrl(url)` permet de la remplacer.

### Rapports d'erreurs (télémétrie)

Lorsqu'un appel API échoue, la bibliothèque peut envoyer un rapport d'erreur au système de suivi des erreurs propre à DomainNameAPI (Sentry). Une petite part aléatoire des appels réussis (4 %, plus chaque appel de plus d'une seconde) peut être envoyée comme enregistrement de performance. Cela aide DomainNameAPI à trouver et corriger les problèmes. Les rapports d'erreurs sont **activés par défaut**.

> ℹ️ Dans la version 1.0.0, l'adresse Sentry (`DefaultSentryDsn`) est encore vide, car le projet Sentry de la bibliothèque .NET n'a pas encore été créé. Tant qu'une release ne l'a pas renseignée, **rien n'est envoyé**. Le comportement décrit ci-dessous correspond à ce qui sera envoyé une fois l'adresse définie.

**Ce qui est envoyé :**

- le code et le message d'erreur
- le chemin du point de terminaison appelé et la méthode HTTP
- la durée de l'appel
- le nom de l'application (`DomainNameAPI-DotNet`), la version de la bibliothèque et la version de .NET
- l'ID revendeur et le nom de domaine
- l'adresse IP publique du serveur (obtenue via `api.ipify.org`, en arrière-plan)
- le nom du compte du système d'exploitation sous lequel l'application s'exécute
- la requête et la réponse API, y compris les coordonnées de contact du domaine ; seuls les identifiants sont masqués — les codes EPP/auth, clés API, mots de passe et jetons sont remplacés par `***`

**Ce qui n'est jamais envoyé :** les codes EPP/auth, clés API, mots de passe ou jetons sous forme lisible ; les arguments des méthodes dans les traces de pile (stack traces).

**Pourquoi et combien de temps :** les données servent uniquement à mesurer l'utilisation et à diagnostiquer les problèmes au plus tôt ; elles sont conservées 30 jours sur le serveur Sentry propre à DomainNameAPI, puis supprimées automatiquement.

Les rapports sont envoyés en arrière-plan : ils ne bloquent ni ne ralentissent jamais votre appel et ne lèvent jamais d'exception. Les erreurs attendues et fréquentes (domaine introuvable, solde insuffisant, identifiants invalides, aucune redirection configurée, ...) ne sont pas signalées.

**Comment le désactiver :** appelez `SetErrorReportingEnabled(false)` après avoir créé le client :

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey);
dna.SetErrorReportingEnabled(false);
```

**Mode minimal :** `SetTelemetryMinimal(true)` laisse les rapports activés mais n'envoie que le code et le message d'erreur, le point de terminaison, la durée, les versions et l'ID revendeur — aucune donnée de requête/réponse, ni domaine, ni adresse IP, ni nom de compte.

La destination peut être modifiée avec la propriété `TelemetryDsn` (par exemple vers votre propre projet Sentry).

> 🔒 **Sécurité :** la vérification des certificats TLS est activée. Sur un serveur dont le bundle CA est obsolète, elle peut être désactivée avec `dna.SetSslVerification(false);` — déconseillé ; mettre à jour le bundle CA est la meilleure solution.

### Avancé

- **Délais d'attente :** 30 secondes par requête et 10 secondes pour la connexion ; modifiez-les avec les propriétés `Timeout` et `ConnectTimeout`.
- **Transport personnalisé :** transmettez un `IHttpTransport` au constructeur (`new DomainNameApiClient(id, key, false, transport)`) ou définissez la propriété `Transport` pour faire passer les requêtes par votre propre pile HTTP, un proxy ou un double de test. Les implémentations renvoient tous les statuts HTTP (les redirections ne sont pas suivies) et lèvent `TransportException` en cas de défaillance réseau.
- **Threads :** un client conserve la dernière requête et la dernière réponse à des fins de diagnostic et n'est pas thread-safe. Utilisez une instance par thread (ou par requête) ou synchronisez les accès. `Dispose()` libère le `HttpClient` sous-jacent.
- **User-Agent :** les requêtes sont envoyées avec `DomainNameAPI-DotNet/1.0.0`.

### Exemples

Le projet [examples](examples/DomainNameApi.Examples) contient un fichier exécutable par méthode (`GetDetails`, `RegisterWithContactInfo`, `RegisterWithContactInfo-TR`, les méthodes de zone DNS et de redirection, ...). Définissez vos identifiants et lancez-en un :

```bash
export DNA_RESELLER_ID=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
export DNA_API_KEY=your-api-key
export DNA_TEST_MODE=true   # optional: use the OTE test environment

dotnet run --project examples/DomainNameApi.Examples -- GetDetails
dotnet run --project examples/DomainNameApi.Examples            # lists all examples
```

### Tests

Les tests ne nécessitent aucun identifiant et n'effectuent aucun appel réseau externe :

```bash
dotnet test
```

Sans SDK .NET local, exécutez-les dans Docker :

```bash
docker run --rm -v "$PWD":/src -w /src mcr.microsoft.com/dotnet/sdk:8.0 dotnet test
```

| Fichier de test | Description |
|-----------|-------------|
| `tests/DomainNameApi.Tests/ConformanceTests.cs` | Rejoue les 134 fixtures de conformité de php-dna (`testdata/conformance/`) et compare les requêtes et les résultats |
| `tests/DomainNameApi.Tests/HttpLayerTests.cs` | En-têtes, URL, chaînes de requête, mode test, délais d'attente, espacement des appels de zone |
| `tests/DomainNameApi.Tests/HttpClientTransportTests.cs` | Le transport `HttpClient` par défaut face à un écouteur local |
| `tests/DomainNameApi.Tests/ErrorMappingTests.cs` | Correspondance des erreurs de la passerelle et messages de validation |
| `tests/DomainNameApi.Tests/TelemetryTests.cs` | Contenu de la télémétrie, masquage des identifiants, mode minimal |
| `tests/DomainNameApi.Tests/UtilityTests.cs` | Validation des contacts, normalisation des téléphones/pays, formatage compatible PHP |

<hr style="border: 4px solid #000; border-style: dashed;">

## Codes de Retour et d'Erreur avec Descriptions

| Code | Description                                    | Détail                                                                                                                                                         |
|------|------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1000 | Commande exécutée avec succès | Opération réussie. |
| 1001 | Commande exécutée avec succès ; action en attente | Opération réussie. Cependant, l'opération est actuellement en file d'attente pour être finalisée. |
| 2003 | Paramètre requis manquant | Erreur de paramètre manquant. Par exemple : aucun numéro de téléphone dans les informations de contact. |
| 2105 | L'objet n'est pas éligible au renouvellement | Le statut du domaine ne permet pas le renouvellement, il est verrouillé pour les opérations de mise à jour. Le statut ne doit pas être "clientupdateprohibited". Peut être dû à d'autres conditions de statut. |
| 2200 | Erreur d'authentification | Erreur d'autorisation, le code de sécurité est incorrect ou le domaine est enregistré auprès d'un autre registraire. |
| 2302 | L'objet existe | Le nom de domaine ou les informations de serveur de noms existent déjà dans la base de données. Enregistrement impossible. |
| 2303 | L'objet n'existe pas | Le nom de domaine ou les informations de serveur de noms n'existent pas dans la base de données. Un nouvel enregistrement est nécessaire. |
| 2304 | Le statut de l'objet interdit l'opération | Le statut du domaine ne permet pas les mises à jour, il est verrouillé pour les opérations de mise à jour. Le statut ne doit pas être "clientupdateprohibited". Peut être dû à d'autres conditions de statut. |

| CODE | DÉTAIL                                                                                                                               |
|------|--------------------------------------------------------------------------------------------------------------------------------------|
| 101  | Erreur système détaillée pour ({0}) !                                                                                                |
| 102  | Erreurs multiples détaillées pour ({0}) !                                                                                            |
| 103  | Erreurs inconnues détaillées pour ({0}) !                                                                                            |
| 200  | Commande API introuvable ({0}) !                                                                                                     |
| 210  | Service API introuvable ({0}) !                                                                                                      |
| 211  | Fournisseur de service API non défini !                                                                                              |
| 300  | Revendeur introuvable !                                                                                                              |
| 301  | Votre adresse IP actuelle n'est pas autorisée à accéder au service. Veuillez vous assurer que vous vous connectez depuis une adresse IP autorisée et réessayez |
| 310  | TLD non pris en charge !                                                                                                             |
| 320  | API introuvable !                                                                                                                    |
| 321  | Devise non prise en charge !                                                                                                         |
| 330  | Paramètre(s) requis non défini(s) ({0}).                                                                                             |
| –    | Assurez-vous d'envoyer tous les objets de contact de manière complète                                                                |
| 340  | Définition de prix introuvable ({0}[{1}]-{2}{3}).                                                                                    |
| 350  | Solde revendeur insuffisant. (Reseller Id : {0} - Solde actuel : {1} {2}).                                                           |
| 350  | La devise comptable ne correspond pas ou le solde est insuffisant.                                                                   |
| 360  | Requête API invalide pour le champ ({0}).                                                                                            |
| 360  | Quota API dépassé !                                                                                                                  |
| 361  | Erreur de limitation de débit !                                                                                                      |
| 362  | Le domaine premium ne peut pas être enregistré pour le moment.                                                                       |
| 363  | Opération annulée car le domaine est en période de renouvellement automatique                                                        |
| 364  | Ce domaine est actuellement indisponible pour toute transaction en raison d'un problème au niveau du registre                        |

| CODE | DÉTAIL                                                                                      |
|------|---------------------------------------------------------------------------------------------|
| 400  | Contact invalide ({0}).                                                                     |
| 401  | Les informations de contact ne peuvent pas être synchronisées.                              |
| 402  | Aucun accès aux informations de contact depuis le registre.                                 |
| 403  | DES INFORMATIONS MANQUENT DANS LE SYSTÈME. SAISISSEZ VOS INFORMATIONS PAR DÉFAUT OU CONTACTEZ L'ÉQUIPE DE SUPPORT. |
| 404  | LE CONTACT DU DOMAINE NE PEUT PAS ÊTRE MIS À JOUR. AUTORISATION DU REGISTRE REQUISE.        |
| 410  | Contact introuvable.                                                                        |
| 410  | Contact introuvable ({0}).                                                                  |
| 420  | Commande API invalide pour le contact {0}.                                                  |
| 430  | API de contact introuvable.                                                                 |
| 440  | Le contact n'est pas synchronisé.                                                           |
| 450  | Trop de contacts de domaine !                                                               |
| 451  | Impossible de poursuivre la mise à jour du contact.                                         |

| CODE | DÉTAIL                                                                                                                               |
|------|--------------------------------------------------------------------------------------------------------------------------------------|
| 500  | ID de domaine invalide.                                                                                                              |
| 500  | ID de domaine invalide ({0}).                                                                                                        |
| 501  | Le domaine n'a pas pu être synchronisé ({0})                                                                                         |
| 502  | Échec du transfert interne                                                                                                           |
| 503  | L'enregistrement du domaine n'est pas disponible.                                                                                    |
| 504  | Les informations du domaine ne correspondent pas. Avant : {0}                                                                        |
| 505  | Saisissez l'adresse IP.                                                                                                              |
| 506  | Le transfert du domaine a démarré, mais les informations de contact n'ont pas pu être lues.                                          |
| 507  | Erreur d'autorisation.                                                                                                               |
| 510  | Domaine introuvable.                                                                                                                 |
| 511  | Les domaines expirés sont introuvables.                                                                                              |
| 512  | Le domaine ne peut pas être renouvelé.                                                                                               |
| 513  | Le domaine n'est pas dans un état modifiable. Il doit être actif pour pouvoir être mis à jour                                        |
| 514  | Période de rédemption attendue.                                                                                                      |
| 520  | Commande API invalide pour le domaine "{0}".                                                                                         |
| 530  | Période de domaine invalide. La période doit être comprise entre {0} et {1} ans. La période demandée est de {2} ans.                 |
| 540  | Le domaine ne peut pas être prolongé au-delà de {0} ans à compter de la date actuelle.                                               |
| 550  | Nom de domaine invalide. Le domaine doit comporter entre {0} et {1} caractères. Le nom de domaine demandé comporte {2} caractères.    |
| 560  | Nombre de serveurs de noms invalide. Le domaine doit avoir entre {0} et {1} serveurs de noms. Le nombre demandé est de {2}.           |
| 561  | Aucune information de serveur de noms trouvée dans la requête reçue                                                                  |
| 570  | IDN non pris en charge pour le TLD "{0}".                                                                                            |
| 571  | Période invalide.                                                                                                                    |
| 572  | Commande invalide.                                                                                                                   |
| 573  | Noms de domaine introuvables.                                                                                                        |
| 574  | Noms de TLD introuvables.                                                                                                            |
| 575  | Le domaine n'est pas dans un état modifiable. Il doit être actif pour que ses serveurs de noms puissent être mis à jour              |
| 576  | Le domaine ne peut être renouvelé qu'au cours du dernier mois précédant la date d'expiration                                         |
| 580  | Transfert non pris en charge pour le TLD "{0}".                                                                                      |
| 581  | Serveur de noms enfant introuvable                                                                                                   |
| 582  | Transfert initié par un autre revendeur.                                                                                             |
| 583  | Transfert non initialisé. Veuillez contacter l'équipe de support.                                                                    |
| 584  | Le statut de l'objet interdit l'opération                                                                                            |
| 590  | Le code d'autorisation est requis pour ce transfert.                                                                                 |
| 591  | Le code d'autorisation n'est pas valide.                                                                                             |
| 592  | Un verrou de transfert est actif sur le domaine.                                                                                     |
| 593  | Le domaine ne peut pas être transféré : les informations de statut et le verrou de transfert doivent respecter les critères requis. (Informations de statut : #ok) |
| 594  | L'adresse du serveur de noms du domaine n'a pas pu être résolue ({0})                                                                |
| 595  | Les informations de contact ne peuvent pas être lues (whois.registrar.tld). Veuillez vous assurer que le statut de protection de la vie privée est ouvert |
| 596  | Les informations de contact n'ont pas pu être vérifiées                                                                              |
| 597  | TLD introuvable                                                                                                                      |
| 598  | Une erreur inconnue s'est produite                                                                                                   |
| 599  | Redirection de domaine introuvable                                                                                                   |

## Licence

Apache License 2.0 — voir [LICENSE](LICENSE).
