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

Le SDK .NET officiel de la plateforme revendeur [Domain Name API](https://www.domainnameapi.com). Il couvre les
67 opérations de l'API REST avec des clients et des modèles typés : domaines, contacts, DNS, DNSSEC, certificats SSL,
hébergement, commandes, produits et votre compte.

```csharp
using var client = new DomainNameApiClient(resellerId, apiKey, ApiEnvironment.Ote);

var search = await client.Domains.SearchAsync("example.com");
if (search.Info?.IsAvailable == true)
{
    await client.Domains.RegisterAsync("example.com", contact, ["tr.apiname.com", "eu.apiname.com"]);
}
```

- **Tout est typé.** Chaque opération est une classe de requête et chaque réponse un modèle documenté pour IntelliSense. Pas de dictionnaires, pas de parsing JSON.
- **Vos propres requêtes.** Chaque requête dérive de `BaseRequest`. Créez une sous-classe pour un endpoint que le SDK ne couvre pas encore.
- **Sûr avec l'argent.** Chaque requête déclare si elle lit, écrit, débite votre solde ou supprime quelque chose. Les requêtes facturables ne sont jamais rejouées, et un mode lecture seule bloque toute modification.
- **Résilience intégrée.** Les limites de débit sont respectées automatiquement, les réponses 429 sont rejouées, et les erreurs réseau sont rejouées pour les lectures.
- **Des erreurs claires.** Un seul type d'exception avec une nature stable, le code d'erreur de l'API, les erreurs de validation par champ, l'ID de corrélation et une piste de résolution.
- **.NET moderne.** Async avec annulation, pagination automatique avec `IAsyncEnumerable`, prise en charge d'`IHttpClientFactory`, annotations nullable.
- **Fonctionne partout.** .NET 10, et tout runtime compatible .NET Standard 2.0 (.NET Framework 4.6.2+, .NET 8, Mono, Unity).

C'est la version .NET de [`dna`, la CLI Domain Name API](https://github.com/domainreseller/cli) : le même document
d'API, les mêmes classes de sécurité, les mêmes règles de nouvelle tentative et les mêmes codes d'erreur.

## Sommaire

- [Installation](#install)
- [Identifiants](#credentials)
- [Démarrage rapide](#quick-start)
- [Configuration](#configuration)
- [Utilisation](#usage) : [compte](#account) · [domaines](#domains) · [contacts](#contacts) · [DNS](#dns) · [DNSSEC](#dnssec) · [SSL](#ssl) · [hébergement](#hosting) · [commandes et produits](#orders-and-products)
- [Requêtes et `BaseRequest`](#requests-and-baserequest)
- [Erreurs](#errors)
- [Sécurité, nouvelles tentatives et limites de débit](#safety-retries-and-rate-limits)
- [Injection de dépendances](#dependency-injection)
- [Tester votre code](#testing-your-code)
- [Structure du projet](#project-layout)
- [Développement](#development)
- [Migrer depuis la 1.x](#upgrading-from-1x)

<a id="install"></a>

## Installation

> [!IMPORTANT]
> **Utilisez toujours la [dernière version publiée](https://github.com/domainreseller/dotnet-dna/releases/latest).** Les paquets publiés sont versionnés et testés.
> Le bouton vert **Code → Download ZIP** télécharge à la place la branche de développement.

```bash
dotnet add package DomainNameApi
```

Pour installer un `.nupkg` téléchargé, placez-le dans un dossier et utilisez ce dossier comme source de paquets :

```bash
dotnet add package DomainNameApi --source ./packages
```

Ou référencez le projet depuis les sources d'une version publiée :

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

**Prérequis :** .NET 10 ou version ultérieure, ou tout runtime compatible .NET Standard 2.0. Aucune dépendance
tierce. Sur .NET Standard 2.0, le paquet installe `System.Text.Json` et `Microsoft.Bcl.AsyncInterfaces` de Microsoft,
intégrés à .NET.

<a id="credentials"></a>

## Identifiants

L'API authentifie chaque requête avec deux valeurs, que le SDK envoie pour vous :

| Valeur | En-tête | Où la trouver |
|---|---|---|
| Reseller ID (un UUID) | `__reseller` | Panneau revendeur → **API settings** |
| Clé API | `X-API-KEY` | Panneau revendeur → **API settings** |

> [!NOTE]
> Il s'agit d'**identifiants API** : vos identifiants de connexion au panneau ne fonctionnent pas ici. **La production et l'OTE (l'environnement de test)
> utilisent des clés API différentes.** Utiliser l'une à la place de l'autre échoue avec `INVALID_API_KEY` ou `INVALID_RESELLER`.

<a id="quick-start"></a>

## Démarrage rapide

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

Le [projet d'exemples](examples/DomainNameApi.Examples) exécute des scénarios de ce type sur l'OTE :

```bash
dotnet run --project examples/DomainNameApi.Examples -- search example.com
```

<a id="configuration"></a>

## Configuration

Il y a quatre façons de créer un client :

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

| Option | Par défaut | Signification |
|---|---|---|
| `ResellerId`, `ApiKey` | (obligatoire) | Les identifiants. |
| `Environment` | `Production` | `Production` (`https://api.domainresellerapi.com`) ou `Ote` (`https://ote.domainresellerapi.com`). |
| `BaseAddress` | aucune | Remplace l'adresse de l'environnement, par exemple pour un proxy. |
| `Timeout` | 60 s | Délai maximal d'une tentative. |
| `MaxRetries` | 2 | Combien de fois une requête échouée est rejouée (voir [nouvelles tentatives](#safety-retries-and-rate-limits)). 0 désactive les nouvelles tentatives. |
| `MaxRetryDelay` | 30 s | L'attente la plus longue avant une nouvelle tentative. |
| `PaceRequests` | `true` | Espace les requêtes pour rester sous les limites de débit de la passerelle. |
| `ReadOnly` | `false` | Bloque, avant envoi, toute requête qui n'est pas une lecture. |
| `UserAgent` | `DomainNameAPI-DotNet/<version>` | L'en-tête `User-Agent`. |

`FromEnvironment()` lit :

| Variable | Signification |
|---|---|
| `DNA_RESELLER_ID`, `DNA_API_KEY` | Les identifiants. |
| `DNA_ENV` | `production` ou `ote` (aussi `prod`, `live`, `test`, `sandbox`). |
| `DNA_TEST_MODE` | `true`, `1`, `on` ou `yes` sélectionne l'OTE. Utilisée quand `DNA_ENV` n'est pas définie. |
| `DNA_BASE_URL` | Remplacement facultatif de l'adresse de base. |
| `DNA_READ_ONLY` | `true` active le mode lecture seule. |

Le client est thread-safe. Créez-en un par jeu d'identifiants et réutilisez-le pendant toute la durée de vie de l'application.

<a id="usage"></a>

## Utilisation

Chaque domaine fonctionnel de l'API a son propre client. Les méthodes prennent des arguments simples ; pour plus d'options, chaque méthode a aussi une
surcharge (ou une méthode sœur) qui prend l'objet de requête.

<a id="account"></a>

### Compte

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

### Domaines

**Recherche**

```csharp
var one = await client.Domains.SearchAsync("example.com", currency: "USD");
bool available = one.Info?.IsAvailable == true;

var many = await client.Domains.BulkSearchAsync(["example.com", "example.net", "example.com.tr"]);
foreach (var domain in many.Available)
{
    Console.WriteLine($"{domain.DomainName}: {domain.Price} {domain.Currency}");
}
```

**Liste et détails**

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

**Enregistrer, renouveler, transférer, restaurer** (ces opérations débitent votre solde)

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

Les requêtes sont vérifiées avant d'être envoyées. Un enregistrement sans les quatre rôles de contact, un contact sans adresse e-mail
ou une durée de 0 an lève une `ArgumentException`, et rien n'atteint l'API.

**Paramètres**

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

**Redirection d'URL, serveurs de noms enfants et documents**

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

### Contacts

Les handles de contact sont des contacts réutilisables. Créez-en un une fois et utilisez-le pour plusieurs domaines.

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

Enregistrements de zone des domaines qui utilisent les serveurs de noms de la plateforme. Les enregistrements sont résolus publiquement dès que le domaine utilise ces serveurs de noms,
ce dont `ApplyAsync` se charge.

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

Le SDK vérifie ce que l'API refuserait : un CNAME à l'apex, une valeur CAA sans ses trois parties, un TTL hors de la plage
1–86400. Les appels de zone sont espacés de 1 seconde, la limite de la passerelle.

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

Un domaine peut avoir plusieurs certificats ; un certificat existant est donc désigné par son **identifiant de
certificat** (un `Guid`), jamais par son nom de domaine. `PurchaseAsync` et `RenewAsync` renvoient cet identifiant :
conservez-le. `ListAsync` retrouve les identifiants des certificats que vous possédez déjà.

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

**Lister les certificats et trouver un identifiant**

```csharp
var page = await client.Ssl.ListAsync(new ListCertificatesRequest { DomainName = "example.com" });   // newest first

await foreach (var certificate in client.Ssl.ListAllAsync())
{
    Console.WriteLine($"{certificate.CertificateId} {certificate.DomainName} {certificate.Status} {certificate.SubscriptionExpiresAt:d}");
}

// The approver addresses of a certificate you already have
var approverEmails = await client.Ssl.GetApproverEmailsAsync(certificateId);
```

**Renouveler** (cette opération débite votre solde)

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

Un certificat acheté ou renouvelé doit être configuré dans les 15 jours ; passé ce délai, il est annulé. Pour
renouveler, appelez `RenewAsync`, puis `ConfigureAsync` avec le nouvel identifiant. N'achetez pas de nouveau avec
`PurchaseAsync` : il s'agirait d'un achat distinct au prix d'achat, et l'ancien certificat ne serait jamais marqué comme
renouvelé. Un identifiant de certificat vide (`Guid.Empty`) lève une `ArgumentException` avant tout envoi.

<a id="hosting"></a>

### Hébergement

```csharp
var packages = await client.Hosting.ListPackagesAsync();
var created = await client.Hosting.CreateAsync("example.com", "starter-linux");   // charges the balance
var status = await client.Hosting.GetStatusAsync("example.com");                   // panel access once ready
var hosting = await client.Hosting.GetAsync("example.com");
var plans = await client.Hosting.ListPlansAsync();
```

<a id="orders-and-products"></a>

### Commandes et produits

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

## Requêtes et `BaseRequest`

Chaque opération est une classe de requête, par exemple `RenewDomainRequest`, `ListDomainsRequest` ou `AddDnsRecordRequest`.
Les méthodes du client ci-dessus les construisent pour vous, et vous pouvez aussi les créer et les envoyer vous-même :

```csharp
var request = new RenewDomainRequest("example.com", period: 2);
Console.WriteLine($"{request} is {request.Safety}");   // "POST domains/renew is Billable"

DomainOperationResult result = await client.SendAsync(request);

// The same, with the HTTP details: correlation ID, soft-quota state, attempts, duration
var response = await client.SendWithResponseAsync(request);
Console.WriteLine($"{response.CorrelationId} quota {response.Quota?.Used}/{response.Quota?.Limit}");
```

Toutes les requêtes dérivent de l'une de ces trois classes de base :

| Classe de base | Pour |
|---|---|
| `BaseRequest` | Les opérations qui ne renvoient rien (verrouiller, déverrouiller, ajouter un enregistrement DNS, ...). |
| `BaseRequest<TResponse>` | Les opérations qui renvoient des données. |
| `PagedRequest<TItem>` | Les opérations de liste. Ajoute `SkipCount`, `MaxResultCount` et `Sorting`, et fonctionne avec `ListAllAsync`. |

Une requête se décrit avec quelques redéfinitions (overrides). Pour POST et PUT, l'objet de requête lui-même est le corps JSON (camelCase,
sans les valeurs null). Les paramètres de requête sont ajoutés dans `AddQueryParameters`, et les vérifications avant envoi vont dans `Validate`.
Pour appeler un endpoint que le SDK ne couvre pas encore, écrivez votre propre requête :

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

Votre requête bénéficie de tout ce que reçoivent les requêtes intégrées : identifiants, nouvelles tentatives selon la classe de sécurité, espacement, mode lecture seule et
gestion des erreurs. Redéfinissez `GetBody()` quand l'API attend un corps de forme différente, et `IsIdempotent` quand une écriture
peut être répétée sans risque.

<a id="errors"></a>

## Erreurs

Toute requête échouée lève une `DomainNameApiException`. Les arguments invalides lèvent une `ArgumentException` avant que rien
ne soit envoyé.

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

| Propriété | Signification |
|---|---|
| `Kind` | Une classification stable (tableau ci-dessous). Branchez votre logique dessus, pas sur le message. |
| `Code` | Le code d'erreur de l'API (comme `Dna.DomainService:Domain:10007`), ou l'un des codes du SDK ci-dessous. |
| `Message` | Ce qui s'est passé. Les messages génériques sont remplacés par les détails de l'API. |
| `StatusCode` | Le statut HTTP, ou null si aucune réponse n'est arrivée. |
| `Details`, `ValidationErrors` | Détails supplémentaires et problèmes par champ renvoyés par l'API. |
| `CorrelationId` | Le `X-Correlation-Id` de la passerelle. Indiquez-le en contactant le support. |
| `Hint` | Une courte suggestion sur la suite à donner. |
| `IsRetryable` | Indique si rejouer la même requête plus tard peut réussir. |
| `OutcomeUnknown` | Une requête qui modifie quelque chose a échoué sur le réseau, elle a donc peut-être été exécutée. |
| `Operation`, `Safety` | La requête en échec (`POST domains/renew`) et sa classe de sécurité. |
| `RetryAfter`, `ResponseBody` | Le `Retry-After` de la passerelle et la réponse brute. |

| `Kind` | Quand |
|---|---|
| `Authentication` | Identifiants erronés ou absents, ou revendeur inconnu ou suspendu dans cet environnement (401, redirection). |
| `Forbidden` | Authentifié mais non autorisé (403). |
| `NotFound` | Le domaine, le contact, la commande ou une autre ressource n'existe pas (404). |
| `Validation` | L'API a refusé l'entrée ou une règle métier a échoué : champ invalide, domaine verrouillé, solde insuffisant (400, 422). |
| `Conflict` | L'état de la ressource n'autorise pas l'opération (409). |
| `RateLimited` | Toujours limité en débit après les nouvelles tentatives automatiques (429). |
| `Server` | Une défaillance côté serveur (5xx). Souvent temporaire. |
| `Network`, `Timeout` | Pas de réponse : DNS, TLS, proxy, connexion réinitialisée, ou aucune réponse dans le délai `Timeout`. |
| `ReadOnly` | Bloquée par le mode lecture seule ; rien n'a été envoyé. |
| `Unknown` | Tout le reste, y compris une réponse que le SDK n'a pas pu lire. |

Codes définis par le SDK lui-même :

| Code | Signification |
|---|---|
| `INVALID_RESELLER` | La passerelle a redirigé vers sa page de suspension : le Reseller ID est inconnu, inactif ou suspendu **dans cet environnement**. |
| `INVALID_API_KEY` | La clé ne correspond pas au revendeur. Les clés de production et d'OTE diffèrent. |
| `MISSING_API_KEY`, `MISSING_RESELLER_ID`, `NO_CREDENTIALS` | Un identifiant est vide. |
| `SUBRESELLER_NOT_ALLOWED` | Les comptes de sous-revendeur ne peuvent pas utiliser l'API. Utilisez les identifiants du revendeur principal. |
| `UNAUTHORIZED` | Toute autre 401. |
| `RATE_LIMITED` | 429 après les nouvelles tentatives. |
| `NETWORK`, `TIMEOUT` | Aucune réponse n'est arrivée. |
| `READ_ONLY` | Bloquée par le mode lecture seule. |
| `EMPTY_RESPONSE`, `INVALID_RESPONSE` | Une réponse réussie était vide ou illisible. |
| `HTTP_<status>` | L'API n'a envoyé aucun code d'erreur. |

<a id="safety-retries-and-rate-limits"></a>

## Sécurité, nouvelles tentatives et limites de débit

Chaque requête a une classe de sécurité, la même que celle de la CLI `dna` :

| `OperationSafety` | Signification | Exemples |
|---|---|---|
| `Read` | Aucun effet de bord, même quand l'API utilise POST ou PUT | recherche, détails, listes, vérifications de renouvellement et de transfert, génération de CSR |
| `Write` | Une modification gratuite et réversible | enregistrements DNS, verrouillage, confidentialité, serveurs de noms, contacts |
| `Billable` | Débite votre solde | enregistrement, renouvellement, transfert, restauration, achat et renouvellement de SSL, commande d'hébergement |
| `Destructive` | Supprime quelque chose ou est difficile à annuler | suppression d'un enregistrement DNS, retrait de DNSSEC, révocation d'un certificat |

- **Le mode lecture seule** (`ReadOnly = true` ou `DNA_READ_ONLY=true`) bloque, avant envoi, tout ce qui n'est pas `Read`.
  Utilisez-le pour explorer, dans les rapports et dans les tableaux de bord.
- **Nouvelles tentatives.** Les réponses 429 sont toujours rejouées, en respectant `Retry-After`. C'est sans risque, même pour les requêtes facturables, car
  la passerelle les refuse avant que quoi que ce soit ne s'exécute. Les erreurs réseau, les timeouts et les réponses 502/503/504 ne sont rejoués que pour
  les lectures (et pour les écritures qui peuvent être répétées sans risque, comme le remplacement des serveurs de noms). Un enregistrement ou un renouvellement n'est jamais envoyé deux fois.
- **Espacement.** La passerelle autorise 5 requêtes par seconde et par clé, 2 par seconde pour les recherches de domaines et 20 par seconde pour
  les appels de zone DNS. Le client espace les requêtes pour rester sous ces limites (appels de zone espacés de 1 seconde). Les informations de domaine et la liste
  des TLD ne sont pas limitées en débit.
- **Quota souple.** Les recherches et les consultations d'informations coûtent 1 point, tout le reste 10. Au-delà de 200 points, la passerelle ralentit chaque
  requête jusqu'à 5 secondes ; les points décroissent de 20 % par minute. `SendWithResponseAsync` expose cet état dans `Quota`.

<a id="dependency-injection"></a>

## Injection de dépendances

`DomainNameApiClient` fonctionne comme un `HttpClient` typé :

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

Le SDK ne suit aucune redirection sur son propre `HttpClient`. Un client créé par une factory peut les suivre ; le SDK reconnaît
tout de même la page de suspension de la passerelle et signale `INVALID_RESELLER`.

<a id="testing-your-code"></a>

## Tester votre code

Passez un `HttpClient` avec un handler factice pour tester le code qui utilise le SDK sans appeler l'API :

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

Pour des tests contre la vraie API, utilisez l'OTE avec sa propre clé.

<a id="project-layout"></a>

## Structure du projet

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

Chaque dossier de domaine fonctionnel a la même forme : `XxxClient.cs`, un dossier `Requests/` avec une classe par opération, et un dossier `Models/`
avec les réponses et les enums. Tous les types d'un domaine fonctionnel partagent un même namespace, comme `DomainNameApi.Domains`.

<a id="development"></a>

## Développement

```bash
dotnet build
dotnet test
```

Les tests n'appellent jamais la vraie API : ils utilisent un faux `HttpMessageHandler`. Sous Windows, ils s'exécutent sur .NET 10 et sur
.NET Framework 4.8 ; ce dernier teste la build .NET Standard 2.0.

`tests/DomainNameApi.Tests/testdata/openapi.json` est le document d'API que la CLI `dna` embarque. `SpecCoverageTests` vérifie
que chaque opération qu'il contient a exactement une classe de requête avec la classe de sécurité de la CLI. Quand l'API gagne une opération,
mettez à jour le document : le test échoue tant que la requête n'existe pas.

<a id="upgrading-from-1x"></a>

## Migrer depuis la 1.x

La 2.0 est un nouveau SDK. La 1.x était un portage de la bibliothèque PHP et renvoyait des enveloppes `JsonNode` non typées ; la 2.0 propose des requêtes
et des modèles typés, des méthodes async et des exceptions.

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

La 1.x envoyait des rapports d'erreurs à Sentry. La 2.0 n'envoie rien nulle part, sauf à l'API.

## Liens

- [CLI `dna` et serveur MCP](https://github.com/domainreseller/cli) : la même API depuis le terminal et depuis des agents IA.
- [Documentation de l'API](https://www.domainnameapi.com/docs/)
- [Changelog](CHANGELOG.md)

## Licence

Apache License 2.0. Voir [LICENSE](LICENSE).
