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

# DomainNameAPI .NET Kitabxanası (dotnet-dna)

DomainNameAPI reseller REST API-si üçün rəsmi .NET klienti: domen axtarışı, qeydiyyat, yenilənmə, transferlər, əlaqə məlumatları, nameserverlər, child nameserverlər, DNS zonaları və URL yönləndirmə.

dotnet-dna [php-dna](https://github.com/domainreseller/php-dna) kitabxanasının dəqiq portudur: eyni metodlar, şəbəkəyə göndərilən eyni sorğular və eyni qaytarılan dəyərlər. O, php-dna-nın uyğunluq (conformance) ssenarilərinə (134 ssenari) qarşı yoxlanılır, buna görə də PHP kitabxanası ilə yazılmış kod birə-bir köçürülə bilər.

## 📦 Yükləmə — həmişə Releases istifadə edin!

⬇️ **Ən son test edilmiş versiyanı buradan əldə edin: https://github.com/domainreseller/dotnet-dna/releases/latest**

> ⚠️ Yaşıl **Code → Download ZIP** düyməsindən **istifadə etməyin** — bu, xam inkişaf branch-ını yükləyir. Release paketləri versiyalanmış, test edilmiş və istehsalata hazırdır.

## Quraşdırma və İnteqrasiya Təlimatı

### Minimum Tələblər

- .NET 10 və ya daha yeni versiya, **yaxud** .NET Standard 2.0-ı dəstəkləyən istənilən icra mühiti (.NET Framework 4.6.2+, .NET Core 2.0+, Mono, Xamarin, Unity).
- Üçüncü tərəf asılılıqları yoxdur. Kitabxana `HttpClient` və `System.Text.Json` istifadə edir (.NET Standard 2.0-da `System.Text.Json` paketi avtomatik əlavə olunur).

### Quraşdırma

**A) NuGet paketi** — paketin identifikatoru `DomainNameApi`-dir. Hər buraxılış `.nupkg` faylı ilə birlikdə dərc olunur; paket NuGet.org-da yerləşdirildikdən sonra onu birbaşa da quraşdıra bilərsiniz:

```bash
dotnet add package DomainNameApi
```

Yüklənmiş `.nupkg` faylını quraşdırmaq üçün onu bir qovluğa yerləşdirin və həmin qovluğu paket mənbəyi kimi istifadə edin:

```bash
dotnet add package DomainNameApi --source ./packages
```

**B) Layihə istinadı** — buraxılışın mənbə kodunu yükləyin və layihəyə istinad əlavə edin:

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

## 🔑 API Məlumatları — Reseller ID və API Key

Kitabxana DomainNameAPI REST API ilə əlaqə qurur və constructor-a ötürülən yalnız iki məlumat tələb edir:

| Parametr | Dəyər |
|---|---|
| Birinci | Reseller ID — `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` kimi UUID |
| İkinci | API Key |

> 💡 **Reseller ID** və **API Key** məlumatlarınızı DomainNameAPI panelinizdə **API Parametrləri** bölməsində tapa bilərsiniz.
> ⚠️ Bunlar **API məlumatlarıdır** — panelə giriş məlumatlarınız burada **işləməyəcək**.

```csharp
using DomainNameApi;

var resellerId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
var apiKey     = "your-api-key";

// Live environment
var dna = new DomainNameApiClient(resellerId, apiKey);

// Test (OTE) environment
var dna = new DomainNameApiClient(resellerId, apiKey, true);
```

Hər iki məlumatın əvvəlindəki və sonundakı boşluqlar silinir. Test rejimi bayrağı konfiqurasiya fayllarının və panellərin saxladığı kimi mətn şəklində də ötürülə bilər: `"true"`, `"1"`, `"on"` və `"yes"` (böyük-kiçik hərfdən asılı olmayaraq) test mühitini seçir, qalan bütün dəyərlər canlı mühiti seçir.

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey, Environment.GetEnvironmentVariable("DNA_TEST_MODE"));
```

## 🚀 Sürətli Başlanğıc

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

### Qaytarılan dəyərlər

Hər metod php-dna-nın qaytardığı eyni strukturu `System.Text.Json.Nodes.JsonNode` (adətən `JsonObject`) kimi qaytarır. Dəyərləri tip çevirmələri (cast) və ya `GetValue<T>()` ilə oxuyun, yaxud bütün nəticəni `ToJsonString()` ilə çap edin:

```csharp
var details = dna.GetDetails("example.com");
string status = (string)details["data"]["Status"];          // "Active"
int    days   = (int)details["data"]["Dates"]["RemainingDays"];
bool   locked = (string)details["data"]["LockStatus"] == "true";   // flags are the strings "true"/"false"
Console.WriteLine(details.ToJsonString());
```

Açar adları, mətn şəklində bayraqlar (`"true"`/`"false"`) və formatlanmış məbləğ sətirləri (`"11.9900"`) php-dna-dakı ilə tam eynidir. Metodların əksəriyyəti `result` = `OK`/`ERROR` zərfindən istifadə edir; `GetCurrentBalance` köhnə `OperationResult` zərfindən istifadə edir, `CheckAvailability` isə uğurlu olduqda sadə siyahı qaytarır.

Əlaqə məlumatları, TR atributları və siyahı filtrləri kimi giriş dəyərləri php-dna-nın açarlarından (`FirstName`, `EMail`, `AddressLine1`, `PhoneCountryCode`, `TRABISCITIZENID`, `PageSize`, ...) istifadə edən `IDictionary<string, object>` obyektləridir. Nameserver siyahıları siyahı (`string[]`, `List<string>`) və ya lüğət (dəyərləri istifadə olunur) ola bilər.

### İstifadə

Aşağıdakı bütün nümunələri [examples](examples) qovluğundan da işə salmaq olar (bax: [Nümunələr](#nümunələr)).

#### Domen Qeydiyyatı

Qeyd: .tr domenləri üçün əlavə parametrlər tələb olunur. `additionalAttributes` parametri .tr kimi əlavə məlumat tələb edən domen adları üçün istifadə olunur.

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

Əlaqə məlumatları göndərilməzdən əvvəl tamamlanır: çatışmayan sahələrə yer tutucu dəyərlər verilir, adlardan rəqəmlər və durğu işarələri silinir, çatışmayan və ya boş ştat/region sahəsi əvəzinə şəhər götürülür, ölkə adları ISO kodlarına çevrilir və telefon nömrələri nömrə və ölkə koduna ayrılır — öz `+` və ya `00` ölkə kodu ilə yazılmış nömrə həmin kodu saxlayır (bax: `ValidateContact`). Heç bir nameserver verilmədikdə (və ya yalnız boş dəyərlər verildikdə) `ns1.domainnameapi.com` və `ns2.domainnameapi.com` istifadə olunur. .tr domenləri üçün TRABIS atributları gateway-in qəbul etdiyi sahələrə endirilir (`TRABISDOMAINCATEGORY` = `Owner`/`Corporate`, `TRABISCITIZENID`, `TRABISTAXOFFICE`, `TRABISTAXNUMBER`); köhnə `TRABISCITIZIENID` açarı və rəqəmsal kateqoriya avtomatik çevrilir.

<details>
<summary>Domen Qeydiyyatı üçün Nümunə Çıxış</summary>

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

Gateway sifarişi rədd edərsə (`"success": false` cavabı verərsə), metod `Code` = `REGISTER_DOMAIN` və gateway-in mesajı olan xəta zərfi qaytarır; beləliklə, rədd edilmiş qeydiyyat heç vaxt tamamlanmış kimi göstərilmir. `Renew` (`RENEW_DOMAIN`) və `Transfer` (`TRANSFER_DOMAIN`) də eyni şəkildə davranır.

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domen Yenilənməsi

```csharp
var result = dna.Renew("example.com", 1);
```

<details>
<summary>Domen Yenilənməsi üçün Nümunə Çıxış</summary>

```json
{ "result": "OK", "data": { "ExpirationDate": "2025-03-15T10:00:00" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domen Transferi

```csharp
var result = dna.Transfer("example.com", "Xy9#mK2$", 3);
```

> ℹ️ Transfer zamanı əlaqə məlumatları heç vaxt göndərilmir: sorğu həmişə boş əlaqə siyahısı daşıyır və əlaqə məlumatlarını qeydiyyatçı (registrar) özü təhvil alır. Buna görə də `Transfer` metodunun əlaqə parametri yoxdur; lazım olarsa, transfer tamamlandıqdan sonra əlaqə məlumatlarını `SaveContacts` ilə yeniləyin.

<details>
<summary>Domen Transferi üçün Nümunə Çıxış</summary>

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

#### Transferi Ləğv Etmək, Təsdiqləmək və ya Rədd Etmək

```csharp
dna.CancelTransfer("example.com");   // pending incoming transfer
dna.ApproveTransfer("example.com");  // pending outgoing transfer
dna.RejectTransfer("example.com");   // pending outgoing transfer
```

<details>
<summary>Transferin Ləğvi üçün Nümunə Çıxış</summary>

```json
{ "result": "OK", "data": { "DomainName": "example.com", "Status": "Cancelled" } }
```

`ApproveTransfer` və `RejectTransfer` da eyni şəkildə, müvafiq olaraq `Approved` / `Rejected` ilə cavab verir.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Transfer Yoxlanışı

```csharp
var result = dna.CheckTransfer("example.com", "Ab1!xyZ9");
```

`result` yalnız qeydiyyat orqanı (registry) domenin transfer edilə bilən olduğunu bildirdikdə `OK` olur; əks halda `Code` = `TRANSFER_NOT_AVAILABLE` ilə `ERROR` qaytarılır və təfərrüatlar yenə də `data` altında olur.

<details>
<summary>Transfer Yoxlanışı üçün Nümunə Çıxış</summary>

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

#### Domen Siyahısı

```csharp
var result = dna.GetList(new Dictionary<string, object>
{
    ["OrderColumn"]    = "Id",
    ["OrderDirection"] = "ASC",
    ["PageNumber"]     = 0,
    ["PageSize"]       = 1000,
});
```

`PageNumber` sıfırdan başlayır. Gateway-in öz filtr adları (`MaxResultCount`, `SkipCount`, `Sorting`, `DomainName`, ...) da ötürülə bilər.

<details>
<summary>Domen Siyahısı üçün Nümunə Çıxış</summary>

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

#### TLD Siyahısı

```csharp
var result = dna.GetTldList(100);
```

<details>
<summary>TLD Siyahısı üçün Nümunə Çıxış</summary>

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

#### Domenin Mövcudluğunun Yoxlanılması

```csharp
var result = dna.CheckAvailability(new[] { "hello", "world123x0" }, new[] { "com", "net" }, 1, "create");
```

<details>
<summary>Domenin Mövcudluğunun Yoxlanılması üçün Nümunə Çıxış</summary>

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

Uğursuzluq halında metod siyahı əvəzinə adi xəta zərfini qaytarır, ona görə də əvvəlcə `result is JsonArray` yoxlamasını edin.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domen Məlumatları

```csharp
var result = dna.GetDetails("example.com");
var synced = dna.SyncFromRegistry("example.com"); // same envelope
```

<details>
<summary>Domen Məlumatları üçün Nümunə Çıxış</summary>

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

#### Nameserver Dəyişdirmə

```csharp
var result = dna.ModifyNameServer("example.com", new[] { "ns1.example.com", "ns2.example.com" });
```

Host adlarının kənarlarındakı boşluqlar silinir, kiçik hərflərə çevrilir və sondakı nöqtə götürülür; boş və təkrarlanan qeydlər atılır.

<details>
<summary>Nameserver Dəyişdirmə üçün Nümunə Çıxış</summary>

```json
{ "result": "OK", "data": { "NameServers": [ "ns1.example.com", "ns2.example.com" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domen Kilidini Aktivləşdirmək

```csharp
var result = dna.EnableTheftProtectionLock("example.com");
```

<details>
<summary>Domen Kilidini Aktivləşdirmək üçün Nümunə Çıxış</summary>

```json
{ "result": "OK", "data": { "LockStatus": true } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domen Kilidini Söndürmək

```csharp
var result = dna.DisableTheftProtectionLock("example.com");
```

<details>
<summary>Domen Kilidini Söndürmək üçün Nümunə Çıxış</summary>

```json
{ "result": "OK", "data": { "LockStatus": false } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Child Nameserver Əlavə Etmək

```csharp
var result = dna.AddChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>Child Nameserver Əlavə Etmək üçün Nümunə Çıxış</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Child Nameserver Silmək

```csharp
var result = dna.DeleteChildNameServer("example.com", "ns1.example.com");
```

<details>
<summary>Child Nameserver Silmək üçün Nümunə Çıxış</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Child Nameserver Yeniləmək

```csharp
var result = dna.ModifyChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>Child Nameserver Yeniləmək üçün Nümunə Çıxış</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domen Məxfiliyini Dəyişdirmək

```csharp
var result = dna.ModifyPrivacyProtectionStatus("example.com", true);

// The flag may also be text, as panels store it: "1", "true", "on" and "yes" enable privacy
var result2 = dna.ModifyPrivacyProtectionStatus("example.com", "on");
```

İxtiyari üçüncü arqument (`reason`) yalnız uyğunluq üçün saxlanılıb; gateway-də belə sahə yoxdur və o göndərilmir.

<details>
<summary>Domen Məxfiliyini Dəyişdirmək üçün Nümunə Çıxış</summary>

```json
{ "data": { "PrivacyProtectionStatus": true }, "result": "OK" }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Domenin Əlaqə Məlumatlarını Əldə Etmək

```csharp
var result = dna.GetContacts("example.com");
```

<details>
<summary>Domenin Əlaqə Məlumatlarını Əldə Etmək üçün Nümunə Çıxış</summary>

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

#### Domenin Əlaqə Məlumatlarını Saxlamaq

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

Əlaqə məlumatları `GetContacts`-ın qaytardığı iç-içə strukturdan da (`Address` → `Line1`/`City`/..., `Phone` → `Phone`/`Fax` → `Number`/`CountryCode`) istifadə edə bilər.

<details>
<summary>Domenin Əlaqə Məlumatlarını Saxlamaq üçün Nümunə Çıxış</summary>

```json
{ "result": "OK", "data": { "contacts": [] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Satıcı Təfərrüatlarını Əldə Etmək

```csharp
var result = dna.GetResellerDetails();
```

<details>
<summary>Satıcı Təfərrüatlarını Əldə Etmək üçün Nümunə Çıxış</summary>

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

#### Cari Balans

```csharp
var result = dna.GetCurrentBalance("USD"); // USD, TRY, EUR or GBP
```

<details>
<summary>Cari Balans üçün Nümunə Çıxış</summary>

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

Uğursuzluq halında `OperationResult` dəyəri `FAILED` olur, `ErrorCode` xəta kodunu (məsələn, `API_429`) saxlayır və `Balance` dəyəri `"0.00"` olur. Hesab yalnız USD və TRY balanslarını bildirir; başqa valyuta (EUR, GBP, ...) tələb edildikdə `ErrorCode` = `BALANCE` və `No EUR balance is reported for this account. Available: USD, TRY.` mesajı ilə xəta qaytarılır.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### DNS Zonasının İdarə Edilməsi

Gateway zonanı qeyd **dəstləri** (ad + tip → dəyərlər) kimi saxlayır və hər yazma əməliyyatında bütün dəsti əvəz edir. `*ResourceRecordSet` və `*ResourceRecordValue` metodları bunu sizin əvəzinizə həll edir. Adlar nisbi (`www`), kök domen (`@`, `""` və ya `example.com`) və ya tam ixtisaslı (`www.example.com.`) ola bilər. Dəstəklənən tiplər: `A`, `AAAA`, `ALIAS`, `CAA`, `CNAME`, `DS`, `MX`, `NAPTR`, `PTR`, `SRV`, `TLSA`, `TXT`. `apply: false` ötürüb `ApplyResourceRecords`-u özünüz çağırmadığınız halda yazma əməliyyatları dərhal tətbiq olunur; bu avtomatik tətbiq uğursuz olarsa, metod tətbiq xətasını qaytarır (dəyişiklik hazırlanmış vəziyyətdə qalır, canlıya keçmir). Ardıcıl zona çağırışları arasında ən azı bir saniyə fasilə saxlanılır, çünki gateway sıx gələn sorğulara HTTP 429 ilə cavab verir.

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
<summary>DNS Zonasının İdarə Edilməsi üçün Nümunə Çıxış</summary>

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

`SetResourceRecordSet` / `AddResourceRecordValue` (tip böyük hərflərlə, TTL isə göndərildiyi kimi, 1–86400 saniyə ilə məhdudlaşdırılaraq göstərilir):

```json
{ "result": "OK", "data": { "Name": "", "Type": "MX", "TTL": 3600, "Contents": [ "10 mx1.example.com", "20 mx2.example.com" ] } }
```

Artıq mövcud olan dəyəri əlavə etmək və ya olmayan dəyəri silmək heç nəyi dəyişmir və `{"result":"OK","data":{..., "Changed": false}}` cavabını qaytarır. Dəstəklənməyən tip heç bir çağırış edilmədən rədd olunur:

```json
{ "result": "ERROR", "error": { "Code": "ADD_ZONE", "Message": "Record type \"NS\" is not supported by the DNS zone API. Supported types: A, AAAA, ALIAS, CAA, CNAME, DS, MX, NAPTR, PTR, SRV, TLSA, TXT.", "Details": "..." } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### URL Yönləndirmə

```csharp
dna.SetForwarding("example.com", "https://www.example.org", "Standard"); // or "Frame" (masked)
var forwarding = dna.GetForwarding("example.com");
dna.DeleteForwarding("example.com");
```

<details>
<summary>URL Yönləndirmə üçün Nümunə Çıxış</summary>

```json
{ "result": "OK", "data": { "Enabled": true, "RedirectAddress": "https://www.example.org/landing", "ForwardType": "Frame" } }
```

Heç bir yönləndirmə qurulmadıqda (gateway `Redirect information not found` cavabı verir) `GetForwarding` `{"result":"OK","data":{"Enabled":false,"RedirectAddress":"","ForwardType":""}}` qaytarır. Digər bütün xətalar — məsələn, mövcud olmayan domen — xəta kimi qaytarılır.

`DeleteForwarding` `{"result":"OK","data":{"DomainName":"example.com","Removed":true}}` qaytarır; heç nə qurulmayıbsa, heç bir dəyişiklik etmir və `"Removed": false` cavabını verir.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Köməkçi Metodlar (şəbəkə tələb etmir)

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
<summary>ValidateContact üçün Nümunə Çıxış</summary>

```json
{
  "FirstName": "Jo", "Phone": "12025550100", "Country": "TR", "LastName": "Isimyok",
  "AddressLine1": "Addres yok", "City": "ISTANBUL", "State": "ISTANBUL", "ZipCode": "34000",
  "PhoneCountryCode": "90", "Fax": "", "FaxCountryCode": ""
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

### Xətaların İdarə Edilməsi

Metodlar API və ya şəbəkə problemlərində heç vaxt istisna (exception) atmır; bunun əvəzinə xəta zərfi qaytarır:

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

| Vəziyyət | `Code` | `Message` |
|---|---|---|
| Gateway-dən gələn HTTP xətası | `API_<status>`, məs. `API_400`, `API_429`, `API_500` | gateway-in mesajı; boş cavab gövdəsi üçün `HTTP 429 - Too Many Requests` |
| Yanlış API məlumatları (HTTP 301/302) | `API_302` | `Invalid API credentials` |
| Validasiya xətaları | `API_400` | gateway-in mesajı; `Details` sahələri sadalayır, məs. `Admin PostalCode - 'PostalCode' is required` |
| Vaxt aşımı (timeout) | `CURL_28` | `Could not reach the DomainNameAPI service: the connection timed out. (...)` |
| Digər şəbəkə xətaları (DNS, qoşulma, TLS, bağlantının qırılması) | `CURL_6`, `CURL_7`, `CURL_60`, `CURL_56` | `Curl error during request: ...` |
| Qeydiyyat, yenilənmə və ya transfer rədd edildi (`"success": false`) | `REGISTER_DOMAIN`, `RENEW_DOMAIN`, `TRANSFER_DOMAIN` | gateway-in mesajı |
| Boş və ya yararsız uğurlu cavab | `RESPONSE` | `Invalid response received from server! Response is empty.` |
| Gateway çağırılmazdan əvvəl edilən yoxlamalar | məs. `AVAILABILITY`, `ADD_ZONE`, `SET_FORWARD` | problemin təsviri |

`API_<n>` kodları həmişə HTTP statusunu bildirir. Gateway öz xəta kodunu göndərdikdə zərf onu `GatewayCode` kimi də daşıyır (məsələn, `Dna.DomainService:Domain:10007`), beləliklə mesajı təhlil etmədən "domen tapılmadı" ilə "TLD dəstəklənmir" hallarını bir-birindən ayıra bilərsiniz. Şəbəkə xətaları `CURL_<n>` kodundan istifadə edir; buradakı rəqəm kodların php-dna ilə uyğun gəlməsi üçün libcurl-un nömrələməsinə əsaslanır. Yalnız proqramlaşdırma xətaları (məsələn, dəstəklənməyən obyekt tipinin ötürülməsi) istisna ata bilər.

Problemlərin aradan qaldırılması üçün sonuncu çağırışı yoxlamaq mümkündür:

```csharp
dna.GetLastFunction();       // "request:GET domains/info"
dna.GetRequestData();        // { "url": ..., "method": ..., "payload": ... }
dna.GetResponseData();       // decoded response, or { "raw_response": "..." }
dna.GetResponseHeaders();    // { "url", "http_code", "content_type", "total_time", "headers" }
dna.GetParsedResponseData(); // parsed response or the error built from it
```

### Test Rejimi

Canlı mühit (`https://api.domainresellerapi.com/api/v1`) əvəzinə OTE test mühitindən (`https://ote.domainresellerapi.com/api/v1`) istifadə etmək üçün constructor-un üçüncü arqumenti kimi `true` ötürün. `GetServiceUrl()` istifadə olunan ünvanı göstərir, `SetServiceUrl(url)` isə onu dəyişdirir.

### Xəta hesabatları (telemetriya)

API çağırışı uğursuz olduqda kitabxana DomainNameAPI-nin öz xəta izləmə sisteminə (Sentry) xəta hesabatı göndərə bilər. Uğurlu çağırışların təsadüfi seçilmiş kiçik bir hissəsi (4%, üstəlik 1 saniyədən uzun çəkən hər çağırış) performans qeydi kimi göndərilə bilər. Bu, DomainNameAPI-yə problemləri tapıb düzəltməyə kömək edir. Xəta hesabatları **standart olaraq aktivdir**.

**Göndərilənlər:**

- xəta kodu və mesajı
- çağırılan endpoint yolu və HTTP metodu
- çağırışın müddəti
- tətbiqin adı (`DomainNameAPI-DotNet`), kitabxananın versiyası və .NET versiyası
- reseller ID və domen adı
- serverin ictimai (public) IP ünvanı (arxa planda `api.ipify.org` vasitəsilə müəyyən edilir)
- tətbiqin altında işlədiyi əməliyyat sistemi hesabının adı
- API sorğusu və cavabı, domenin əlaqə məlumatları daxil olmaqla; yalnız giriş məlumatları maskalanır — EPP/auth kodları, API açarları, parollar və tokenlər `***` ilə əvəz olunur

**Heç vaxt göndərilməyənlər:** oxunaqlı formada EPP/auth kodları, API açarları, parollar və ya tokenlər; stack trace-lərdəki metod arqumentləri.

**Niyə və nə qədər müddətə:** məlumatlar yalnız istifadənin ölçülməsi və problemlərin erkən diaqnostikası üçün istifadə olunur, DomainNameAPI-nin öz Sentry serverində 30 gün saxlanılır, sonra avtomatik olaraq silinir.

Hesabatlar arxa planda göndərilir: onlar heç vaxt çağırışınızı bloklamır, yavaşlatmır və istisna atmır. Gözlənilən, yüksək həcmli xətalar (domen tapılmadı, balans kifayət deyil, yanlış API məlumatları, qurulmuş yönləndirmənin olmaması, ...) hesabata daxil edilmir.

**Necə söndürmək olar:** klienti yaratdıqdan sonra `SetErrorReportingEnabled(false)` çağırın:

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey);
dna.SetErrorReportingEnabled(false);
```

**Minimal rejim:** `SetTelemetryMinimal(true)` hesabatları aktiv saxlayır, lakin yalnız xəta kodu və mesajını, endpoint-i, müddəti, versiyaları və reseller ID-ni göndərir — sorğu/cavab məlumatları, domen, IP ünvanı və ya hesab adı göndərilmir.

Təyinat ünvanı `TelemetryDsn` xassəsi ilə dəyişdirilə bilər (məsələn, öz Sentry layihənizə).

> 🔒 **Təhlükəsizlik:** TLS sertifikat yoxlaması aktivdir. CA paketi köhnə olan serverdə `dna.SetSslVerification(false);` ilə söndürülə bilər — tövsiyə edilmir; CA paketini yeniləmək daha düzgün həlldir.

### Təkmil İstifadə

- **Vaxt aşımları:** hər sorğu üçün 30 saniyə, qoşulma üçün 10 saniyə; `Timeout` və `ConnectTimeout` xassələri ilə dəyişdirilə bilər.
- **Fərdi nəqliyyat qatı (transport):** sorğuları öz HTTP stekiniz, proksi və ya test əvəzedicisi (test double) vasitəsilə yönləndirmək üçün constructor-a `IHttpTransport` ötürün (`new DomainNameApiClient(id, key, false, transport)`) və ya `Transport` xassəsini təyin edin. İmplementasiyalar bütün HTTP status kodlarını qaytarır (yönləndirmələrə əməl edilmir) və şəbəkə xətalarında `TransportException` atır.
- **Axınlar (thread):** klient diaqnostika üçün sonuncu sorğu və cavabı saxlayır və thread-safe deyil. Hər axın (və ya hər sorğu) üçün ayrıca nüsxə istifadə edin, yaxud girişi sinxronlaşdırın. `Dispose()` daxili `HttpClient`-i azad edir.
- **User-Agent:** sorğular `DomainNameAPI-DotNet/1.0.1` kimi göndərilir.

### Nümunələr

[examples](examples/DomainNameApi.Examples) layihəsi hər metod üçün işə salına bilən bir fayl ehtiva edir (`GetDetails`, `RegisterWithContactInfo`, `RegisterWithContactInfo-TR`, DNS zona və yönləndirmə metodları, ...). API məlumatlarınızı təyin edin və birini işə salın:

```bash
export DNA_RESELLER_ID=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
export DNA_API_KEY=your-api-key
export DNA_TEST_MODE=true   # optional: use the OTE test environment

dotnet run --project examples/DomainNameApi.Examples -- GetDetails
dotnet run --project examples/DomainNameApi.Examples            # lists all examples
```

### Testlər

Testlər heç bir API məlumatı tələb etmir və xarici şəbəkə çağırışları etmir:

```bash
dotnet test
```

Lokal .NET SDK yoxdursa, testləri Docker-də işə salın:

```bash
docker run --rm -v "$PWD":/src -w /src mcr.microsoft.com/dotnet/sdk:10.0 dotnet test
```

| Test faylı | Açıqlama |
|-----------|-------------|
| `tests/DomainNameApi.Tests/ConformanceTests.cs` | php-dna-nın bütün 134 uyğunluq ssenarisini (`testdata/conformance/`) təkrar icra edir, sorğuları və nəticələri müqayisə edir |
| `tests/DomainNameApi.Tests/HttpLayerTests.cs` | Başlıqlar, URL-lər, sorğu sətirləri (query string), test rejimi, vaxt aşımları, zona çağırışları arasındakı fasilə |
| `tests/DomainNameApi.Tests/HttpClientTransportTests.cs` | Lokal dinləyiciyə qarşı standart `HttpClient` nəqliyyat qatı |
| `tests/DomainNameApi.Tests/ErrorMappingTests.cs` | Gateway xətalarının uyğunlaşdırılması və validasiya mesajları |
| `tests/DomainNameApi.Tests/TelemetryTests.cs` | Telemetriya məzmunu, giriş məlumatlarının maskalanması, minimal rejim |
| `tests/DomainNameApi.Tests/UtilityTests.cs` | Əlaqə məlumatlarının validasiyası, telefon/ölkə normallaşdırılması, PHP ilə uyğun formatlama |

<hr style="border: 4px solid #000; border-style: dashed;">

## Geri Dönüş və Xəta Kodları ilə Açıqlamaları

| Kod  | Açıqlama                                       | Təfərrüat                                                                                                                                                      |
|------|------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1000 | Əmr uğurla tamamlandı                          | Əməliyyat uğurludur.                                                                                                                                           |
| 1001 | Əmr uğurla tamamlandı; əməliyyat gözləmədədir  | Əməliyyat uğurludur. Lakin əməliyyat hazırda tamamlanmaq üçün növbəyə alınıb.                                                                                  |
| 2003 | Tələb olunan parametr çatışmır                 | Parametr çatışmazlığı xətası. Məsələn, əlaqə məlumatlarında telefon nömrəsinin olmaması.                                                                       |
| 2105 | Obyekt yenilənmə üçün uyğun deyil              | Domen statusu yenilənmə üçün uyğun deyil, yeniləmə əməliyyatları üçün kilidlənib. Status "clientupdateprohibited" olmamalıdır. Digər status şərtlərindən qaynaqlana bilər. |
| 2200 | Kimlik doğrulama xətası                        | İcazə xətası; təhlükəsizlik kodu yanlışdır və ya domen başqa qeydiyyatçıda qeydiyyatdadır.                                                                     |
| 2302 | Obyekt artıq mövcuddur                         | Domen adı və ya nameserver məlumatları verilənlər bazasında artıq mövcuddur. Qeydiyyat edilə bilməz.                                                           |
| 2303 | Obyekt mövcud deyil                            | Domen adı və ya nameserver məlumatları verilənlər bazasında mövcud deyil. Yeni qeydiyyat tələb olunur.                                                         |
| 2304 | Obyekt statusu əməliyyatı qadağan edir         | Domen statusu yeniləmələr üçün uyğun deyil, yeniləmə əməliyyatları üçün kilidlənib. Status "clientupdateprohibited" olmamalıdır. Digər status şərtlərindən qaynaqlana bilər. |

| KOD  | TƏFƏRRÜAT                                                                                                                            |
|------|--------------------------------------------------------------------------------------------------------------------------------------|
| 101  | ({0}) üçün təfərrüatlı sistem xətası!                                                                                                |
| 102  | ({0}) üçün təfərrüatlı çoxsaylı xətalar!                                                                                             |
| 103  | ({0}) üçün təfərrüatlı naməlum xətalar!                                                                                              |
| 200  | API əmri tapılmadı ({0})!                                                                                                            |
| 210  | API xidməti tapılmadı ({0})!                                                                                                         |
| 211  | API xidmət provayderi təyin edilməyib!                                                                                               |
| 300  | Satıcı tapılmadı!                                                                                                                    |
| 301  | Hazırkı IP ünvanınızın giriş icazəsi yoxdur. Zəhmət olmasa, icazəli IP ünvanından qoşulduğunuza əmin olun və yenidən cəhd edin       |
| 310  | TLD dəstəklənmir!                                                                                                                    |
| 320  | API tapılmadı!                                                                                                                       |
| 321  | Valyuta dəstəklənmir!                                                                                                                |
| 330  | Tələb olunan parametr(lər) təyin edilməyib ({0}).                                                                                    |
| –    | Bütün əlaqə obyektlərini tam göndərdiyinizə əmin olun                                                                                |
| 340  | Qiymət təyinatı tapılmadı ({0}[{1}]-{2}{3}).                                                                                         |
| 350  | Satıcı balansı kifayət deyil. (Satıcı Id : {0} - Cari Balans : {1} {2}).                                                             |
| 350  | Mühasibat valyutası uyğun gəlmir və ya balans kifayət deyil.                                                                         |
| 360  | ({0}) sahəsi üçün etibarsız API sorğusu.                                                                                             |
| 360  | API kvotası aşılıb!                                                                                                                  |
| 361  | Sorğu məhdudlaşdırma (throttling) xətası!                                                                                            |
| 362  | Premium domen hazırda qeydiyyat üçün əlçatan deyil.                                                                                  |
| 363  | Domen avtomatik yenilənmə dövründə olduğu üçün əməliyyat ləğv edildi                                                                 |
| 364  | Bu domen qeydiyyat orqanı (registry) ilə bağlı problem səbəbindən hazırda əməliyyat üçün əlçatan deyil                               |

| KOD  | TƏFƏRRÜAT                                                                                   |
|------|---------------------------------------------------------------------------------------------|
| 400  | Etibarsız əlaqə ({0}).                                                                      |
| 401  | Əlaqə məlumatı sinxronlaşdırıla bilmir.                                                     |
| 402  | Qeydiyyat orqanından əlaqə məlumatlarına giriş yoxdur.                                      |
| 403  | SİSTEMDƏ ÇATIŞMAYAN MƏLUMAT VAR. STANDART MƏLUMATLARINIZI DAXİL EDİN VƏ YA DƏSTƏK KOMANDASI İLƏ ƏLAQƏ SAXLAYIN. |
| 404  | DOMENİN ƏLAQƏ MƏLUMATLARI YENİLƏNƏ BİLMİR. QEYDİYYAT ORQANINDAN İCAZƏ TƏLƏB OLUNUR.         |
| 410  | Əlaqə tapılmadı.                                                                            |
| 410  | Əlaqə tapılmadı ({0}).                                                                      |
| 420  | {0} əlaqəsi üçün etibarsız API əmri.                                                        |
| 430  | Əlaqə API-si tapılmadı.                                                                     |
| 440  | Əlaqə sinxronlaşdırılmayıb.                                                                 |
| 450  | Domen əlaqələri həddindən artıq çoxdur!                                                     |
| 451  | Əlaqə yeniləməsini davam etdirmək mümkün olmadı.                                            |

| KOD  | TƏFƏRRÜAT                                                                                                                            |
|------|--------------------------------------------------------------------------------------------------------------------------------------|
| 500  | Etibarsız domen id.                                                                                                                  |
| 500  | Etibarsız domen id ({0}).                                                                                                            |
| 501  | Domen sinxronlaşdırıla bilmədi ({0})                                                                                                 |
| 502  | Daxili transfer uğursuz oldu                                                                                                         |
| 503  | Domen qeydiyyatı mümkün deyil.                                                                                                       |
| 504  | Domen məlumatları uyğun gəlmir. Əvvəlki: {0}                                                                                         |
| 505  | IP ünvanını daxil edin.                                                                                                              |
| 506  | Domen transferi başladıldı, lakin əlaqə məlumatları oxuna bilmədi.                                                                   |
| 507  | İcazə xətası.                                                                                                                        |
| 510  | Domen tapılmadı.                                                                                                                     |
| 511  | Müddəti bitmiş domenlər tapıla bilməz.                                                                                               |
| 512  | Domen yenilənə bilməz.                                                                                                               |
| 513  | Domen yenilənə bilən statusda deyil. Yenilənməsi üçün aktiv olmalıdır                                                                |
| 514  | Bərpa (redemption) dövrü gözlənilir.                                                                                                 |
| 520  | "{0}" domeni üçün etibarsız API əmri.                                                                                                |
| 530  | Etibarsız domen müddəti. Müddət {0} ilə {1} il arasında olmalıdır. Tələb olunan müddət {2} ildir.                                    |
| 540  | Domen cari tarixdən {0} ildən artıq uzadıla bilməz.                                                                                  |
| 550  | Etibarsız domen adı. Domen {0} ilə {1} simvol uzunluğunda olmalıdır. Tələb olunan domen adı {2} simvoldur.                           |
| 560  | Etibarsız nameserver sayı. Domenin {0} ilə {1} arasında nameserveri olmalıdır. Tələb olunan nameserver sayı {2}.                     |
| 561  | Daxil olan sorğuda nameserver məlumatı tapılmadı                                                                                     |
| 570  | "{0}" TLD-si üçün IDN dəstəklənmir.                                                                                                  |
| 571  | Müddət etibarsızdır.                                                                                                                 |
| 572  | Əmr etibarsızdır.                                                                                                                    |
| 573  | Domen adları tapılmadı.                                                                                                              |
| 574  | TLD adları tapılmadı.                                                                                                                |
| 575  | Domen yenilənə bilən statusda deyil. Nameserverlərin yenilənməsi üçün aktiv olmalıdır                                                |
| 576  | Domen yalnız bitmə tarixindən əvvəlki son 1 ay ərzində yenilənə bilər                                                                |
| 580  | "{0}" TLD-si üçün transfer dəstəklənmir.                                                                                             |
| 581  | Child nameserver tapılmadı                                                                                                           |
| 582  | Transfer başqa satıcı tərəfindən başladılıb.                                                                                         |
| 583  | Transfer başladılmayıb. Zəhmət olmasa, dəstək komandası ilə əlaqə saxlayın.                                                          |
| 584  | Obyekt statusu əməliyyatı qadağan edir                                                                                               |
| 590  | Bu transfer üçün auth kodu tələb olunur.                                                                                             |
| 591  | Auth kodu etibarsızdır.                                                                                                              |
| 592  | Domen üzərində transfer kilidi mövcuddur.                                                                                            |
| 593  | Domen transfer edilə bilməz; Status Məlumatı və Transfer Kilidi tələb olunan kriteriyalara uyğun olmalıdır. (Status Məlumatı: #ok)   |
| 594  | Domenin nameserver ünvanı həll edilə bilmədi ({0})                                                                                   |
| 595  | Əlaqə məlumatları oxuna bilmir (whois.registrar.tld). Zəhmət olmasa, məxfilik qorumasının açıq olduğuna əmin olun                    |
| 596  | Əlaqə məlumatları təsdiqlənə bilmədi                                                                                                 |
| 597  | TLD tapılmadı                                                                                                                        |
| 598  | Naməlum xəta baş verdi                                                                                                               |
| 599  | Domen yönləndirməsi tapılmadı                                                                                                        |

## Lisenziya

Apache License 2.0 — bax: [LICENSE](LICENSE).
