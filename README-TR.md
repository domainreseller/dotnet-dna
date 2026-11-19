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

# DomainNameAPI .NET Kütüphanesi (dotnet-dna)

DomainNameAPI bayi (reseller) REST API'si için resmi .NET istemcisi: alan adı sorgulama, kayıt, yenileme, transfer, kişi (contact) bilgileri, nameserver'lar, child nameserver'lar, DNS bölgeleri (zone) ve URL yönlendirme.

dotnet-dna, [php-dna](https://github.com/domainreseller/php-dna) kütüphanesinin birebir karşılığıdır: aynı metotlar, ağa giden aynı istekler ve aynı dönüş değerleri. php-dna'nın uyumluluk (conformance) senaryolarına (134 senaryo) karşı doğrulanır; bu sayede PHP kütüphanesiyle yazılmış kod birebir aktarılabilir.

## 📦 İndirme — her zaman Releases kullanın!

⬇️ **En güncel test edilmiş sürümü buradan indirin: https://github.com/domainreseller/dotnet-dna/releases/latest**

> ⚠️ Yeşil **Code → Download ZIP** butonunu **kullanmayın** — bu buton ham geliştirme dalını (development branch) indirir. Release paketleri sürümlenmiş, test edilmiş ve canlı kullanıma hazırdır.

## Kurulum ve Entegrasyon Rehberi

### Minimum Gereksinimler

- .NET 10 veya üzeri, **ya da** .NET Standard 2.0'ı destekleyen herhangi bir çalışma ortamı (.NET Framework 4.6.2+, .NET Core 2.0+, Mono, Xamarin, Unity).
- Üçüncü taraf bağımlılığı yoktur. Kütüphane `HttpClient` ve `System.Text.Json` kullanır (.NET Standard 2.0'da `System.Text.Json` paketi otomatik olarak eklenir).

### Kurulum

**A) NuGet paketi** — paket kimliği `DomainNameApi`'dir. Her sürüm `.nupkg` dosyasıyla birlikte yayınlanır; paket NuGet.org'da listelendiğinde doğrudan da kurabilirsiniz:

```bash
dotnet add package DomainNameApi
```

İndirdiğiniz bir `.nupkg` dosyasını kurmak için dosyayı bir klasöre koyun ve o klasörü paket kaynağı olarak kullanın:

```bash
dotnet add package DomainNameApi --source ./packages
```

**B) Proje referansı** — sürümün kaynak kodunu indirin ve projeye referans verin:

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

## 🔑 API Bilgileri — Reseller ID ve API Key

Kütüphane DomainNameAPI REST API ile konuşur ve constructor'a verilen yalnızca iki bilgiye ihtiyaç duyar:

| Parametre | Değer |
|---|---|
| Birinci | Reseller ID — `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` biçiminde UUID |
| İkinci | API Key |

> 💡 **Reseller ID** ve **API Key** bilgilerinizi DomainNameAPI panelinizdeki **API Ayarları** bölümünde bulabilirsiniz.
> ⚠️ Bunlar **API bilgileridir** — panel giriş bilgileriniz burada **çalışmaz**.

```csharp
using DomainNameApi;

var resellerId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
var apiKey     = "your-api-key";

// Live environment
var dna = new DomainNameApiClient(resellerId, apiKey);

// Test (OTE) environment
var dna = new DomainNameApiClient(resellerId, apiKey, true);
```

Her iki bilginin başındaki ve sonundaki boşluklar temizlenir. Test modu bayrağı, yapılandırma dosyalarının ve panellerin sakladığı şekilde metin olarak da verilebilir: `"true"`, `"1"`, `"on"` ve `"yes"` (büyük/küçük harf fark etmez) test ortamını seçer, diğer her değer canlı ortamı seçer.

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey, Environment.GetEnvironmentVariable("DNA_TEST_MODE"));
```

## 🚀 Hızlı Başlangıç

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

### Dönüş değerleri

Her metot, php-dna'nın döndürdüğü yapının aynısını bir `System.Text.Json.Nodes.JsonNode` (genellikle bir `JsonObject`) olarak döndürür. Değerleri tür dönüşümü (cast) veya `GetValue<T>()` ile okuyabilir, sonucun tamamını `ToJsonString()` ile yazdırabilirsiniz:

```csharp
var details = dna.GetDetails("example.com");
string status = (string)details["data"]["Status"];          // "Active"
int    days   = (int)details["data"]["Dates"]["RemainingDays"];
bool   locked = (string)details["data"]["LockStatus"] == "true";   // flags are the strings "true"/"false"
Console.WriteLine(details.ToJsonString());
```

Anahtar adları, metin biçimindeki bayraklar (`"true"`/`"false"`) ve biçimlendirilmiş tutar metinleri (`"11.9900"`) php-dna'dakiyle birebir aynıdır. Çoğu metot `result` = `OK`/`ERROR` zarfını kullanır; `GetCurrentBalance` eski tip `OperationResult` zarfını kullanır, `CheckAvailability` ise başarılı olduğunda doğrudan bir liste döndürür.

Kişi bilgileri, TR öznitelikleri ve liste filtreleri gibi girdiler, php-dna'nın anahtarlarını (`FirstName`, `EMail`, `AddressLine1`, `PhoneCountryCode`, `TRABISCITIZENID`, `PageSize`, ...) kullanan `IDictionary<string, object>` nesneleridir. Nameserver listeleri bir liste (`string[]`, `List<string>`) veya bir sözlük (değerleri kullanılır) olabilir.

### Kullanım

Aşağıdaki tüm örnekler [examples](examples) klasöründen de çalıştırılabilir (bkz. [Örnekler](#örnekler)).

#### Alan Adı Kaydı

Not: .tr alan adları için ek parametreler gereklidir. `additionalAttributes` parametresi, .tr gibi ek bilgi gerektiren alan adlarında kullanılır.

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

Kişi bilgileri gönderilmeden önce tamamlanır: eksik alanlara yer tutucu değerler atanır, isimlerden rakamlar ve noktalama işaretleri temizlenir, eksik veya boş bırakılan eyalet/il bilgisi yerine şehir kullanılır, ülke adları ISO kodlarına çevrilir ve telefon numaraları numara ve ülke kodu olarak ayrıştırılır — kendi `+` veya `00` ülke koduyla yazılmış bir numara bu kodu korur (bkz. `ValidateContact`). Hiç nameserver verilmezse (veya yalnızca boş değerler verilirse) `ns1.domainnameapi.com` ve `ns2.domainnameapi.com` kullanılır. .tr alan adlarında TRABIS öznitelikleri gateway'in kabul ettiği alanlara indirgenir (`TRABISDOMAINCATEGORY` = `Owner`/`Corporate`, `TRABISCITIZENID`, `TRABISTAXOFFICE`, `TRABISTAXNUMBER`); eski `TRABISCITIZIENID` anahtarı ve sayısal kategori değeri otomatik olarak dönüştürülür.

<details>
<summary>Alan Adı Kaydı için Örnek Çıktı</summary>

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

Gateway siparişi reddederse (`"success": false` yanıtı verirse), metot `Code` = `REGISTER_DOMAIN` ve gateway'in mesajını içeren bir hata zarfı döndürür; böylece reddedilen bir kayıt asla tamamlanmış gibi bildirilmez. `Renew` (`RENEW_DOMAIN`) ve `Transfer` (`TRANSFER_DOMAIN`) de aynı şekilde davranır.

<hr style="border: 4px solid #000; border-style: dashed;">

#### Alan Adı Yenileme

```csharp
var result = dna.Renew("example.com", 1);
```

<details>
<summary>Alan Adı Yenileme için Örnek Çıktı</summary>

```json
{ "result": "OK", "data": { "ExpirationDate": "2025-03-15T10:00:00" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Alan Adı Transferi

```csharp
var result = dna.Transfer("example.com", "Xy9#mK2$", 3);
```

> ℹ️ Transfer işleminde kişi bilgileri asla gönderilmez: istek her zaman boş bir kişi listesi taşır ve kişi bilgilerini kayıt firması (registrar) kendisi devralır. Bu nedenle `Transfer` metodunun kişi bilgisi parametresi yoktur; gerekirse transfer tamamlandıktan sonra kişi bilgilerini `SaveContacts` ile güncelleyin.

<details>
<summary>Alan Adı Transferi için Örnek Çıktı</summary>

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

#### Transferi İptal Etme, Onaylama veya Reddetme

```csharp
dna.CancelTransfer("example.com");   // pending incoming transfer
dna.ApproveTransfer("example.com");  // pending outgoing transfer
dna.RejectTransfer("example.com");   // pending outgoing transfer
```

<details>
<summary>Transfer İptali için Örnek Çıktı</summary>

```json
{ "result": "OK", "data": { "DomainName": "example.com", "Status": "Cancelled" } }
```

`ApproveTransfer` ve `RejectTransfer` da aynı şekilde, sırasıyla `Approved` / `Rejected` ile yanıt verir.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Transfer Kontrolü

```csharp
var result = dna.CheckTransfer("example.com", "Ab1!xyZ9");
```

`result` yalnızca kayıt otoritesi (registry) alan adının transfer edilebilir olduğunu bildirdiğinde `OK` olur; aksi halde `Code` = `TRANSFER_NOT_AVAILABLE` ile `ERROR` döner ve ayrıntılar yine `data` altında yer alır.

<details>
<summary>Transfer Kontrolü için Örnek Çıktı</summary>

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

#### Alan Adı Listesi

```csharp
var result = dna.GetList(new Dictionary<string, object>
{
    ["OrderColumn"]    = "Id",
    ["OrderDirection"] = "ASC",
    ["PageNumber"]     = 0,
    ["PageSize"]       = 1000,
});
```

`PageNumber` sıfırdan başlar. Gateway'in kendi filtre adları (`MaxResultCount`, `SkipCount`, `Sorting`, `DomainName`, ...) da kullanılabilir.

<details>
<summary>Alan Adı Listesi için Örnek Çıktı</summary>

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

#### TLD Listesi

```csharp
var result = dna.GetTldList(100);
```

<details>
<summary>TLD Listesi için Örnek Çıktı</summary>

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

#### Alan Adı Uygunluk Kontrolü

```csharp
var result = dna.CheckAvailability(new[] { "hello", "world123x0" }, new[] { "com", "net" }, 1, "create");
```

<details>
<summary>Alan Adı Uygunluk Kontrolü için Örnek Çıktı</summary>

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

Hata durumunda metot liste yerine olağan hata zarfını döndürür; bu yüzden önce `result is JsonArray` kontrolü yapın.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Alan Adı Detayları

```csharp
var result = dna.GetDetails("example.com");
var synced = dna.SyncFromRegistry("example.com"); // same envelope
```

<details>
<summary>Alan Adı Detayları için Örnek Çıktı</summary>

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

#### Nameserver Değiştirme

```csharp
var result = dna.ModifyNameServer("example.com", new[] { "ns1.example.com", "ns2.example.com" });
```

Sunucu adlarının baş ve sonundaki boşluklar kırpılır, küçük harfe çevrilir ve sondaki nokta kaldırılır; boş ve tekrarlanan girdiler atılır.

<details>
<summary>Nameserver Değiştirme için Örnek Çıktı</summary>

```json
{ "result": "OK", "data": { "NameServers": [ "ns1.example.com", "ns2.example.com" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Alan Adı Kilidini Etkinleştirme

```csharp
var result = dna.EnableTheftProtectionLock("example.com");
```

<details>
<summary>Alan Adı Kilidini Etkinleştirme için Örnek Çıktı</summary>

```json
{ "result": "OK", "data": { "LockStatus": true } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Alan Adı Kilidini Kaldırma

```csharp
var result = dna.DisableTheftProtectionLock("example.com");
```

<details>
<summary>Alan Adı Kilidini Kaldırma için Örnek Çıktı</summary>

```json
{ "result": "OK", "data": { "LockStatus": false } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Child Nameserver Ekleme

```csharp
var result = dna.AddChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>Child Nameserver Ekleme için Örnek Çıktı</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Child Nameserver Silme

```csharp
var result = dna.DeleteChildNameServer("example.com", "ns1.example.com");
```

<details>
<summary>Child Nameserver Silme için Örnek Çıktı</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Child Nameserver Güncelleme

```csharp
var result = dna.ModifyChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>Child Nameserver Güncelleme için Örnek Çıktı</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Alan Adı Gizliliğini Değiştirme

```csharp
var result = dna.ModifyPrivacyProtectionStatus("example.com", true);

// The flag may also be text, as panels store it: "1", "true", "on" and "yes" enable privacy
var result2 = dna.ModifyPrivacyProtectionStatus("example.com", "on");
```

İsteğe bağlı üçüncü argüman (`reason`) yalnızca uyumluluk için korunmuştur; gateway'de böyle bir alan yoktur ve gönderilmez.

<details>
<summary>Alan Adı Gizliliğini Değiştirme için Örnek Çıktı</summary>

```json
{ "data": { "PrivacyProtectionStatus": true }, "result": "OK" }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Alan Adı Kişi Bilgilerini Alma

```csharp
var result = dna.GetContacts("example.com");
```

<details>
<summary>Alan Adı Kişi Bilgilerini Alma için Örnek Çıktı</summary>

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

#### Alan Adı Kişi Bilgilerini Kaydetme

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

Kişi bilgileri, `GetContacts`'ın döndürdüğü iç içe yapıyı da (`Address` → `Line1`/`City`/..., `Phone` → `Phone`/`Fax` → `Number`/`CountryCode`) kullanabilir.

<details>
<summary>Alan Adı Kişi Bilgilerini Kaydetme için Örnek Çıktı</summary>

```json
{ "result": "OK", "data": { "contacts": [] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Bayi Bilgilerini Alma

```csharp
var result = dna.GetResellerDetails();
```

<details>
<summary>Bayi Bilgilerini Alma için Örnek Çıktı</summary>

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

#### Güncel Bakiye

```csharp
var result = dna.GetCurrentBalance("USD"); // USD, TRY, EUR or GBP
```

<details>
<summary>Güncel Bakiye için Örnek Çıktı</summary>

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

Hata durumunda `OperationResult` değeri `FAILED` olur, `ErrorCode` hata kodunu (örneğin `API_429`) içerir ve `Balance` değeri `"0.00"` olur. Hesap yalnızca USD ve TRY bakiyelerini bildirir; başka bir para birimi (EUR, GBP, ...) istendiğinde `ErrorCode` = `BALANCE` ve `No EUR balance is reported for this account. Available: USD, TRY.` mesajıyla hata döner.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### DNS Bölgesi (Zone) Yönetimi

Gateway bir bölgeyi kayıt **kümeleri** (ad + tür → değerler) olarak saklar ve her yazma işleminde kümenin tamamını değiştirir. `*ResourceRecordSet` ve `*ResourceRecordValue` metotları bunu sizin yerinize halleder. Adlar göreli (`www`), kök alan adı (`@`, `""` veya `example.com`) ya da tam nitelikli (`www.example.com.`) olabilir. Desteklenen türler: `A`, `AAAA`, `ALIAS`, `CAA`, `CNAME`, `DS`, `MX`, `NAPTR`, `PTR`, `SRV`, `TLSA`, `TXT`. `apply: false` verip `ApplyResourceRecords`'u kendiniz çağırmadığınız sürece yazma işlemleri anında uygulanır; bu otomatik uygulama başarısız olursa metot uygulama hatasını döndürür (değişiklik hazırlanmış olarak bekler, yayına alınmaz). Ardışık bölge çağrıları arasında en az bir saniye bırakılır, çünkü gateway art arda gelen isteklere HTTP 429 ile yanıt verir.

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
<summary>DNS Bölgesi Yönetimi için Örnek Çıktı</summary>

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

`SetResourceRecordSet` / `AddResourceRecordValue` (tür büyük harfle, TTL ise gönderildiği şekliyle ve 1–86400 saniye ile sınırlandırılarak bildirilir):

```json
{ "result": "OK", "data": { "Name": "", "Type": "MX", "TTL": 3600, "Contents": [ "10 mx1.example.com", "20 mx2.example.com" ] } }
```

Zaten mevcut olan bir değeri eklemek ya da bulunmayan bir değeri kaldırmak hiçbir şeyi değiştirmez ve `{"result":"OK","data":{..., "Changed": false}}` yanıtını döndürür. Desteklenmeyen bir tür, herhangi bir çağrı yapılmadan reddedilir:

```json
{ "result": "ERROR", "error": { "Code": "ADD_ZONE", "Message": "Record type \"NS\" is not supported by the DNS zone API. Supported types: A, AAAA, ALIAS, CAA, CNAME, DS, MX, NAPTR, PTR, SRV, TLSA, TXT.", "Details": "..." } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### URL Yönlendirme

```csharp
dna.SetForwarding("example.com", "https://www.example.org", "Standard"); // or "Frame" (masked)
var forwarding = dna.GetForwarding("example.com");
dna.DeleteForwarding("example.com");
```

<details>
<summary>URL Yönlendirme için Örnek Çıktı</summary>

```json
{ "result": "OK", "data": { "Enabled": true, "RedirectAddress": "https://www.example.org/landing", "ForwardType": "Frame" } }
```

Herhangi bir yönlendirme tanımlı değilse (gateway `Redirect information not found` yanıtı verir) `GetForwarding`, `{"result":"OK","data":{"Enabled":false,"RedirectAddress":"","ForwardType":""}}` döndürür. Diğer tüm hatalar — örneğin var olmayan bir alan adı — hata olarak döndürülür.

`DeleteForwarding`, `{"result":"OK","data":{"DomainName":"example.com","Removed":true}}` döndürür; hiçbir yönlendirme tanımlı değilse hiçbir şey yapmaz ve `"Removed": false` yanıtını verir.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### Yardımcı Metotlar (ağ bağlantısı gerektirmez)

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
<summary>ValidateContact için Örnek Çıktı</summary>

```json
{
  "FirstName": "Jo", "Phone": "12025550100", "Country": "TR", "LastName": "Isimyok",
  "AddressLine1": "Addres yok", "City": "ISTANBUL", "State": "ISTANBUL", "ZipCode": "34000",
  "PhoneCountryCode": "90", "Fax": "", "FaxCountryCode": ""
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

### Hata Yönetimi

Metotlar API veya ağ sorunlarında asla istisna (exception) fırlatmaz; bunun yerine bir hata zarfı döndürür:

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

| Durum | `Code` | `Message` |
|---|---|---|
| Gateway'den dönen HTTP hatası | `API_<status>`, örn. `API_400`, `API_429`, `API_500` | gateway'in mesajı; gövde boşsa `HTTP 429 - Too Many Requests` |
| Hatalı API bilgileri (HTTP 301/302) | `API_302` | `Invalid API credentials` |
| Doğrulama hataları | `API_400` | gateway'in mesajı; `Details` ilgili alanları listeler, örn. `Admin PostalCode - 'PostalCode' is required` |
| Zaman aşımı | `CURL_28` | `Could not reach the DomainNameAPI service: the connection timed out. (...)` |
| Diğer ağ hataları (DNS, bağlantı, TLS, bağlantı sıfırlanması) | `CURL_6`, `CURL_7`, `CURL_60`, `CURL_56` | `Curl error during request: ...` |
| Kayıt, yenileme veya transfer reddedildi (`"success": false`) | `REGISTER_DOMAIN`, `RENEW_DOMAIN`, `TRANSFER_DOMAIN` | gateway'in mesajı |
| Boş veya kullanılamaz başarılı yanıt | `RESPONSE` | `Invalid response received from server! Response is empty.` |
| Gateway çağrılmadan önce yapılan kontroller | örn. `AVAILABILITY`, `ADD_ZONE`, `SET_FORWARD` | sorunun açıklaması |

`API_<n>` kodları her zaman bir HTTP durum kodunu ifade eder. Gateway kendi hata kodunu gönderdiğinde zarf bu kodu `GatewayCode` olarak da taşır (örneğin `Dna.DomainService:Domain:10007`); böylece mesajı ayrıştırmadan "alan adı bulunamadı" ile "TLD desteklenmiyor" durumlarını birbirinden ayırabilirsiniz. Ağ hataları `CURL_<n>` kodunu kullanır; buradaki sayı, kodların php-dna ile eşleşmesi için libcurl'ün numaralandırmasını izler. Yalnızca programlama hataları (örneğin desteklenmeyen bir nesne türü vermek) istisna fırlatabilir.

Sorun giderme için son çağrı incelenebilir:

```csharp
dna.GetLastFunction();       // "request:GET domains/info"
dna.GetRequestData();        // { "url": ..., "method": ..., "payload": ... }
dna.GetResponseData();       // decoded response, or { "raw_response": "..." }
dna.GetResponseHeaders();    // { "url", "http_code", "content_type", "total_time", "headers" }
dna.GetParsedResponseData(); // parsed response or the error built from it
```

### Test Modu

Canlı ortam (`https://api.domainresellerapi.com/api/v1`) yerine OTE test ortamını (`https://ote.domainresellerapi.com/api/v1`) kullanmak için constructor'ın üçüncü argümanı olarak `true` verin. `GetServiceUrl()` kullanılan adresi gösterir, `SetServiceUrl(url)` ise bu adresi değiştirir.

### Hata raporlama (telemetri)

Bir API çağrısı başarısız olduğunda kütüphane, DomainNameAPI'nin kendi hata takip sistemine (Sentry) bir hata raporu gönderebilir. Başarılı çağrıların rastgele seçilen küçük bir kısmı (%4, ayrıca 1 saniyeden uzun süren her çağrı) performans kaydı olarak gönderilebilir. Bu, DomainNameAPI'nin sorunları bulup düzeltmesine yardımcı olur. Hata raporlama **varsayılan olarak açıktır**.

**Gönderilenler:**

- hata kodu ve mesajı
- çağrılan uç nokta yolu ve HTTP yöntemi
- çağrı süresi
- uygulama adı (`DomainNameAPI-DotNet`), kütüphane sürümü ve .NET sürümü
- bayi kimliği (Reseller ID) ve alan adı
- sunucunun genel (public) IP adresi (arka planda `api.ipify.org` üzerinden öğrenilir)
- uygulamanın altında çalıştığı işletim sistemi hesabının adı
- API isteği ve yanıtı, alan adının kişi bilgileri dahil; yalnızca kimlik bilgileri maskelenir — EPP/transfer kodları, API anahtarları, parolalar ve token'lar `***` ile değiştirilir

**Asla gönderilmeyenler:** okunabilir biçimde EPP/transfer kodları, API anahtarları, parolalar veya token'lar; hata izlerindeki (stack trace) metot argümanları.

**Neden ve ne kadar süre:** veriler yalnızca kullanım ölçümü ve sorunların erken teşhisi için kullanılır; DomainNameAPI'nin kendi Sentry sunucusunda 30 gün saklanır, ardından otomatik olarak silinir.

Raporlar arka planda gönderilir: çağrınızı asla engellemez ya da yavaşlatmaz ve asla istisna fırlatmaz. Beklenen ve yüksek hacimli hatalar (alan adı bulunamadı, yetersiz bakiye, hatalı API bilgileri, tanımlı yönlendirme olmaması, ...) raporlanmaz.

**Nasıl kapatılır:** istemciyi oluşturduktan sonra `SetErrorReportingEnabled(false)` çağırın:

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey);
dna.SetErrorReportingEnabled(false);
```

**Minimal mod:** `SetTelemetryMinimal(true)` raporlamayı açık tutar, ancak yalnızca hata kodu ve mesajını, uç noktayı, süre bilgisini, sürümleri ve bayi kimliğini gönderir — istek/yanıt verisi, alan adı, IP adresi veya hesap adı gönderilmez.

Hedef adres `TelemetryDsn` özelliği ile değiştirilebilir (örneğin kendi Sentry projenize).

> 🔒 **Güvenlik:** TLS sertifika doğrulaması açıktır. CA paketi güncel olmayan bir sunucuda `dna.SetSslVerification(false);` ile kapatılabilir — önerilmez; CA paketini güncellemek daha doğru çözümdür.

### Gelişmiş Kullanım

- **Zaman aşımları:** istek başına 30 saniye, bağlantı için 10 saniye; `Timeout` ve `ConnectTimeout` özellikleriyle değiştirilebilir.
- **Özel taşıma katmanı (transport):** istekleri kendi HTTP altyapınız, bir proxy veya bir test dublörü üzerinden yönlendirmek için constructor'a bir `IHttpTransport` verin (`new DomainNameApiClient(id, key, false, transport)`) ya da `Transport` özelliğini ayarlayın. Uygulamalar her HTTP durum kodunu olduğu gibi döndürür (yönlendirmeler takip edilmez) ve ağ hatalarında `TransportException` fırlatır.
- **İş parçacıkları (thread):** bir istemci, tanılama amacıyla son isteği ve yanıtı saklar ve thread-safe değildir. Her iş parçacığı (veya her istek) için ayrı bir örnek kullanın ya da erişimi senkronize edin. `Dispose()` alttaki `HttpClient`'ı serbest bırakır.
- **User-Agent:** istekler `DomainNameAPI-DotNet/1.0.1` olarak gönderilir.

### Örnekler

[examples](examples/DomainNameApi.Examples) projesi her metot için çalıştırılabilir bir dosya içerir (`GetDetails`, `RegisterWithContactInfo`, `RegisterWithContactInfo-TR`, DNS bölgesi ve yönlendirme metotları, ...). API bilgilerinizi tanımlayın ve birini çalıştırın:

```bash
export DNA_RESELLER_ID=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
export DNA_API_KEY=your-api-key
export DNA_TEST_MODE=true   # optional: use the OTE test environment

dotnet run --project examples/DomainNameApi.Examples -- GetDetails
dotnet run --project examples/DomainNameApi.Examples            # lists all examples
```

### Testler

Testler herhangi bir API bilgisine ihtiyaç duymaz ve dışarıya ağ çağrısı yapmaz:

```bash
dotnet test
```

Yerelde .NET SDK yoksa testleri Docker içinde çalıştırın:

```bash
docker run --rm -v "$PWD":/src -w /src mcr.microsoft.com/dotnet/sdk:10.0 dotnet test
```

| Test dosyası | Açıklama |
|-----------|-------------|
| `tests/DomainNameApi.Tests/ConformanceTests.cs` | php-dna'nın 134 uyumluluk senaryosunun (`testdata/conformance/`) tamamını yeniden oynatır, istekleri ve sonuçları karşılaştırır |
| `tests/DomainNameApi.Tests/HttpLayerTests.cs` | Başlıklar, URL'ler, sorgu metinleri (query string), test modu, zaman aşımları, bölge çağrısı aralıkları |
| `tests/DomainNameApi.Tests/HttpClientTransportTests.cs` | Yerel bir dinleyiciye karşı varsayılan `HttpClient` taşıma katmanı |
| `tests/DomainNameApi.Tests/ErrorMappingTests.cs` | Gateway hata eşlemesi ve doğrulama mesajları |
| `tests/DomainNameApi.Tests/TelemetryTests.cs` | Telemetri içerikleri, kimlik bilgisi maskeleme, minimal mod |
| `tests/DomainNameApi.Tests/UtilityTests.cs` | Kişi bilgisi doğrulama, telefon/ülke normalizasyonu, PHP uyumlu biçimlendirme |

<hr style="border: 4px solid #000; border-style: dashed;">

## Dönüş ve Hata Kodları ile Açıklamaları

| Kod  | Açıklama                                       | Detay                                                                                                                                                          |
|------|------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1000 | Komut başarıyla tamamlandı                     | İşlem başarılı.                                                                                                                                                |
| 1001 | Komut başarıyla tamamlandı; işlem beklemede    | İşlem başarılı. Ancak işlem şu anda tamamlanmak üzere kuyruğa alınmıştır.                                                                                      |
| 2003 | Gerekli parametre eksik                        | Parametre eksik hatası. Örneğin kişi bilgilerinde telefon girilmemiş olması.                                                                                   |
| 2105 | Nesne yenilemeye uygun değil                   | Alan adının durumu yenilemeye uygun değil, güncelleme işlemlerine kilitlenmiş. Durum "clientupdateprohibited" olmamalıdır. Başka durum koşullarından kaynaklanabilir. |
| 2200 | Kimlik doğrulama hatası                        | Yetkilendirme hatası; güvenlik kodu hatalı veya alan adı başka bir kayıt firmasında kayıtlı.                                                                   |
| 2302 | Nesne zaten mevcut                             | Alan adı veya nameserver bilgisi veritabanında zaten mevcut. Kayıt yapılamaz.                                                                                  |
| 2303 | Nesne mevcut değil                             | Alan adı veya nameserver bilgisi veritabanında mevcut değil. Yeni kayıt gereklidir.                                                                            |
| 2304 | Nesne durumu işleme izin vermiyor              | Alan adının durumu güncellemeye uygun değil, güncelleme işlemlerine kilitlenmiş. Durum "clientupdateprohibited" olmamalıdır. Başka durum koşullarından kaynaklanabilir. |

| KOD  | DETAY                                                                                                                                |
|------|--------------------------------------------------------------------------------------------------------------------------------------|
| 101  | ({0}) için ayrıntılı sistem hatası!                                                                                                  |
| 102  | ({0}) için ayrıntılı çoklu hata!                                                                                                     |
| 103  | ({0}) için ayrıntılı bilinmeyen hata!                                                                                                |
| 200  | API komutu bulunamadı ({0})!                                                                                                         |
| 210  | API servisi bulunamadı ({0})!                                                                                                        |
| 211  | API servis sağlayıcısı ayarlanmamış!                                                                                                 |
| 300  | Bayi bulunamadı!                                                                                                                     |
| 301  | Mevcut IP adresinizin erişim yetkisi yok. Lütfen yetkili bir IP adresinden bağlandığınızdan emin olun ve tekrar deneyin              |
| 310  | TLD desteklenmiyor!                                                                                                                  |
| 320  | API bulunamadı!                                                                                                                      |
| 321  | Para birimi desteklenmiyor!                                                                                                          |
| 330  | Gerekli parametre(ler) ayarlanmamış ({0}).                                                                                           |
| –    | Tüm kişi (contact) nesnelerini eksiksiz gönderdiğinizden emin olun                                                                   |
| 340  | Fiyat tanımı bulunamadı ({0}[{1}]-{2}{3}).                                                                                           |
| 350  | Yetersiz bayi bakiyesi. (Bayi Id : {0} - Mevcut Bakiye : {1} {2}).                                                                   |
| 350  | Muhasebe para birimi eşleşmiyor veya bakiye yetersiz.                                                                                |
| 360  | ({0}) alanı için geçersiz API isteği.                                                                                                |
| 360  | API kotası aşıldı!                                                                                                                   |
| 361  | İstek sınırlama (throttling) hatası!                                                                                                 |
| 362  | Premium alan adı şu anda kayıt için uygun değil.                                                                                     |
| 363  | Alan adı otomatik yenileme döneminde olduğu için işlem iptal edildi                                                                  |
| 364  | Bu alan adı, kayıt otoritesindeki (registry) bir sorun nedeniyle şu anda işlem için uygun değil                                      |

| KOD  | DETAY                                                                                       |
|------|---------------------------------------------------------------------------------------------|
| 400  | Geçersiz kişi bilgisi ({0}).                                                                |
| 401  | Kişi bilgisi senkronize edilemiyor.                                                         |
| 402  | Kayıt otoritesinden kişi bilgilerine erişilemiyor.                                          |
| 403  | SİSTEMDE EKSİK BİLGİ VAR. VARSAYILAN BİLGİLERİNİZİ GİRİN VEYA DESTEK EKİBİYLE İLETİŞİME GEÇİN. |
| 404  | ALAN ADI KİŞİ BİLGİLERİ GÜNCELLENEMİYOR. KAYIT OTORİTESİNDEN İZİN GEREKLİ.                  |
| 410  | Kişi bulunamadı.                                                                            |
| 410  | Kişi bulunamadı ({0}).                                                                      |
| 420  | {0} kişisi için geçersiz API komutu.                                                        |
| 430  | Kişi API'si bulunamadı.                                                                     |
| 440  | Kişi bilgisi senkronize edilmemiş.                                                          |
| 450  | Çok fazla alan adı kişi kaydı!                                                              |
| 451  | Kişi bilgisi güncellemesine devam edilemedi.                                                |

| KOD  | DETAY                                                                                                                                |
|------|--------------------------------------------------------------------------------------------------------------------------------------|
| 500  | Geçersiz alan adı kimliği.                                                                                                           |
| 500  | Geçersiz alan adı kimliği ({0}).                                                                                                     |
| 501  | Alan adı senkronize edilemedi ({0})                                                                                                  |
| 502  | Dahili transfer başarısız oldu                                                                                                       |
| 503  | Alan adı kaydı yapılamıyor.                                                                                                          |
| 504  | Alan adı bilgileri eşleşmiyor. Önceki: {0}                                                                                           |
| 505  | IP adresini girin.                                                                                                                   |
| 506  | Alan adı transferi başlatıldı ancak kişi bilgileri okunamadı.                                                                        |
| 507  | Yetkilendirme hatası.                                                                                                                |
| 510  | Alan adı bulunamadı.                                                                                                                 |
| 511  | Süresi dolmuş alan adları bulunamaz.                                                                                                 |
| 512  | Alan adı yenilenemez.                                                                                                                |
| 513  | Alan adı güncellenebilir durumda değil. Güncellenebilmesi için aktif olmalıdır                                                       |
| 514  | Geri alma (redemption) dönemi bekleniyor.                                                                                            |
| 520  | "{0}" alan adı için geçersiz API komutu.                                                                                             |
| 530  | Geçersiz alan adı süresi. Süre {0} ile {1} yıl arasında olmalıdır. İstenen süre {2} yıl.                                             |
| 540  | Alan adı, bugünden itibaren {0} yıldan daha ileriye uzatılamaz.                                                                      |
| 550  | Geçersiz alan adı. Alan adı {0} ile {1} karakter uzunluğunda olmalıdır. İstenen alan adı {2} karakter.                               |
| 560  | Geçersiz nameserver sayısı. Alan adının {0} ile {1} arasında nameserver'ı olmalıdır. İstenen nameserver sayısı {2}.                  |
| 561  | Gelen istekte nameserver bilgisi bulunamadı                                                                                          |
| 570  | "{0}" TLD'si için IDN desteklenmiyor.                                                                                                |
| 571  | Süre geçersiz.                                                                                                                       |
| 572  | Komut geçersiz.                                                                                                                      |
| 573  | Alan adları bulunamadı.                                                                                                              |
| 574  | TLD adları bulunamadı.                                                                                                               |
| 575  | Alan adı güncellenebilir durumda değil. Nameserver'ların güncellenebilmesi için aktif olmalıdır                                      |
| 576  | Alan adı, bitiş tarihinden önceki son 1 ay içinde yenilenebilir                                                                      |
| 580  | "{0}" TLD'si için transfer desteklenmiyor.                                                                                           |
| 581  | Child nameserver bulunamadı                                                                                                          |
| 582  | Transfer başka bir bayi tarafından başlatıldı.                                                                                       |
| 583  | Transfer başlatılamadı. Lütfen destek ekibiyle iletişime geçin.                                                                      |
| 584  | Nesne durumu işleme izin vermiyor                                                                                                    |
| 590  | Bu transfer için auth kodu gereklidir.                                                                                               |
| 591  | Auth kodu geçerli değil.                                                                                                             |
| 592  | Alan adında transfer kilidi bulunuyor.                                                                                               |
| 593  | Alan adı transfer edilemez; Durum Bilgisi ve Transfer Kilidi gerekli kriterlere uymalıdır. (Durum Bilgisi: #ok)                      |
| 594  | Alan adının nameserver adresi çözümlenemedi ({0})                                                                                    |
| 595  | Kişi bilgileri okunamıyor (whois.registrar.tld). Lütfen gizlilik korumasının açık olduğundan emin olun                               |
| 596  | Kişi bilgileri doğrulanamadı                                                                                                         |
| 597  | TLD bulunamadı                                                                                                                       |
| 598  | Bilinmeyen bir hata oluştu                                                                                                           |
| 599  | Alan adı yönlendirmesi bulunamadı                                                                                                    |

## Lisans

Apache License 2.0 — bkz. [LICENSE](LICENSE).
