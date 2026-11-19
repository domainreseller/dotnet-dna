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

# مكتبة DomainNameAPI لـ .NET ‏(dotnet-dna)

عميل .NET الرسمي لواجهة DomainNameAPI REST API الخاصة بالموزعين: البحث عن النطاقات، والتسجيل، والتجديد، والنقل، وجهات الاتصال، وخوادم الأسماء، وخوادم الأسماء الفرعية، ومناطق DNS، وإعادة توجيه عناوين URL.

تُعدّ dotnet-dna نقلًا أمينًا لمكتبة [php-dna](https://github.com/domainreseller/php-dna): الدوال نفسها، والطلبات نفسها على مستوى الشبكة، والقيم المُعادة نفسها. ويتم التحقق منها مقابل ملفات المطابقة (conformance fixtures) الخاصة بـ php-dna ‏(134 سيناريو)، لذا فإن الشيفرة المكتوبة لمكتبة PHP تنتقل إليها سطرًا بسطر.

## 📦 التنزيل — استخدم دائمًا صفحة الإصدارات (Releases)!

⬇️ **احصل على أحدث نسخة مختبَرة من هنا: https://github.com/domainreseller/dotnet-dna/releases/latest**

> ⚠️ **لا** تستخدم الزر الأخضر **Code → Download ZIP** — فهو ينزّل فرع التطوير الخام. حزم الإصدارات مرقّمة الإصدار ومختبَرة وجاهزة لبيئة الإنتاج.

## دليل التثبيت والتكامل

### الحد الأدنى من المتطلبات

- ‏.NET 10 أو أحدث، **أو** أي بيئة تشغيل تدعم .NET Standard 2.0 ‏(.NET Framework 4.6.2+‎، .NET Core 2.0+‎، Mono، Xamarin، Unity).
- لا توجد اعتماديات من أطراف ثالثة. تستخدم المكتبة `HttpClient` و `System.Text.Json` (على .NET Standard 2.0 تُجلب حزمة `System.Text.Json` تلقائيًا).

### التثبيت

**أ) حزمة NuGet** — معرّف الحزمة هو `DomainNameApi`. يتضمن كل إصدار ملف `.nupkg`؛ وبمجرد إدراج الحزمة على NuGet.org يمكنك أيضًا تثبيتها مباشرة:

```bash
dotnet add package DomainNameApi
```

لتثبيت ملف `.nupkg` تم تنزيله، ضعه في مجلد واستخدم ذلك المجلد كمصدر للحزم:

```bash
dotnet add package DomainNameApi --source ./packages
```

**ب) مرجع مشروع** — نزّل الشيفرة المصدرية للإصدار وأضف مرجعًا إلى المشروع:

```bash
dotnet add reference path/to/dotnet-dna/src/DomainNameApi/DomainNameApi.csproj
```

## 🔑 بيانات اعتماد API — Reseller ID و API Key

تتواصل المكتبة مع DomainNameAPI REST API وتحتاج فقط إلى بيانات اعتماد اثنتين تُمرَّران إلى الدالة الإنشائية (constructor):

| المُعامِل | القيمة |
|---|---|
| الأول | Reseller ID — بصيغة UUID مثل `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| الثاني | API Key |

> 💡 ستجد **Reseller ID** و **API Key** في لوحة DomainNameAPI الخاصة بك ضمن **إعدادات API**.
> ⚠️ هذه **بيانات اعتماد API** — بيانات تسجيل الدخول إلى اللوحة **لن تعمل** هنا.

```csharp
using DomainNameApi;

var resellerId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
var apiKey     = "your-api-key";

// Live environment
var dna = new DomainNameApiClient(resellerId, apiKey);

// Test (OTE) environment
var dna = new DomainNameApiClient(resellerId, apiKey, true);
```

تُزال المسافات البيضاء في بداية ونهاية بيانات الاعتماد كلتيهما. يمكن أيضًا تمرير علامة وضع الاختبار كنص، بالطريقة التي تخزنها بها ملفات الإعدادات ولوحات التحكم: القيم `"true"` و `"1"` و `"on"` و `"yes"` (بأي حالة أحرف) تختار بيئة الاختبار، وأي قيمة أخرى تختار البيئة الحية.

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey, Environment.GetEnvironmentVariable("DNA_TEST_MODE"));
```

## 🚀 البدء السريع

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

### القيم المُعادة

تُعيد كل دالة البنية نفسها التي تُعيدها php-dna، على هيئة `System.Text.Json.Nodes.JsonNode` (غالبًا `JsonObject`). اقرأ القيم باستخدام التحويلات (casts) أو `GetValue<T>()`، أو اطبع النتيجة كاملة باستخدام `ToJsonString()`:

```csharp
var details = dna.GetDetails("example.com");
string status = (string)details["data"]["Status"];          // "Active"
int    days   = (int)details["data"]["Dates"]["RemainingDays"];
bool   locked = (string)details["data"]["LockStatus"] == "true";   // flags are the strings "true"/"false"
Console.WriteLine(details.ToJsonString());
```

أسماء المفاتيح، والعلامات النصية (`"true"`/`"false"`)، والسلاسل المالية المنسّقة (`"11.9900"`) مطابقة تمامًا لما في php-dna. تستخدم معظم الدوال الغلاف `result` = `OK`/`ERROR`؛ بينما تستخدم `GetCurrentBalance` الغلاف القديم `OperationResult`، وتُعيد `CheckAvailability` قائمة مجردة عند النجاح.

المُدخلات مثل جهات الاتصال وسمات TR ومرشحات القوائم هي من النوع `IDictionary<string, object>` بمفاتيح php-dna نفسها (`FirstName`، `EMail`، `AddressLine1`، `PhoneCountryCode`، `TRABISCITIZENID`، `PageSize`، ...). يمكن أن تكون قوائم خوادم الأسماء قائمة (`string[]`، `List<string>`) أو قاموسًا (تُستخدم القيم).

### الاستخدام

يمكن أيضًا تشغيل جميع الأمثلة أدناه من مجلد [examples](examples) (انظر [الأمثلة](#الأمثلة)).

#### تسجيل النطاق

ملاحظة: تتطلب نطاقات .tr مُعامِلات إضافية. يُستخدم المُعامِل `additionalAttributes` لأسماء النطاقات التي تتطلب معلومات إضافية مثل .tr.

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

تُستكمل جهات الاتصال قبل إرسالها: الحقول الناقصة تحصل على قيم بديلة، وتُزال الأرقام وعلامات الترقيم من الأسماء، وتُستخدم المدينة عندما تكون الولاية مفقودة أو فارغة، وتتحول أسماء الدول إلى رموز ISO، وتُقسَّم أرقام الهواتف إلى رقم ورمز دولة — والرقم المكتوب برمز دولته الخاص بصيغة `+` أو `00` يحتفظ بذلك الرمز (انظر `ValidateContact`). عند عدم تحديد أي خادم أسماء (أو تحديد خوادم فارغة فقط)، يُستخدم `ns1.domainnameapi.com` و `ns2.domainnameapi.com`. بالنسبة لنطاقات .tr تُختصر سمات TRABIS إلى ما تقبله البوابة (`TRABISDOMAINCATEGORY` = `Owner`/`Corporate`، `TRABISCITIZENID`، `TRABISTAXOFFICE`، `TRABISTAXNUMBER`)؛ ويتم تحويل المفتاح القديم `TRABISCITIZIENID` والفئة الرقمية تلقائيًا.

<details>
<summary>مثال على مخرجات تسجيل النطاق</summary>

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

إذا رفضت البوابة الطلب (أي أجابت بـ `"success": false`)، تُعيد الدالة غلاف خطأ مع `Code` = `REGISTER_DOMAIN` ورسالة البوابة، وبذلك لا يُبلَّغ أبدًا عن تسجيل مرفوض على أنه تم. وتتصرف `Renew` ‏(`RENEW_DOMAIN`) و `Transfer` ‏(`TRANSFER_DOMAIN`) بالطريقة نفسها.

<hr style="border: 4px solid #000; border-style: dashed;">

#### تجديد النطاق

```csharp
var result = dna.Renew("example.com", 1);
```

<details>
<summary>مثال على مخرجات تجديد النطاق</summary>

```json
{ "result": "OK", "data": { "ExpirationDate": "2025-03-15T10:00:00" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### نقل النطاق

```csharp
var result = dna.Transfer("example.com", "Xy9#mK2$", 3);
```

> ℹ️ لا يُرسل النقل جهات اتصال أبدًا: يحمل الطلب دائمًا قائمة جهات اتصال فارغة، ويتولى المسجّل نقل جهات الاتصال بنفسه. لذلك لا تحتوي `Transfer` على مُعامِل لجهات الاتصال؛ وعند الحاجة، حدّث جهات الاتصال باستخدام `SaveContacts` بعد اكتمال النقل.

<details>
<summary>مثال على مخرجات نقل النطاق</summary>

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

#### إلغاء النقل أو الموافقة عليه أو رفضه

```csharp
dna.CancelTransfer("example.com");   // pending incoming transfer
dna.ApproveTransfer("example.com");  // pending outgoing transfer
dna.RejectTransfer("example.com");   // pending outgoing transfer
```

<details>
<summary>مثال على مخرجات إلغاء النقل</summary>

```json
{ "result": "OK", "data": { "DomainName": "example.com", "Status": "Cancelled" } }
```

تستجيب `ApproveTransfer` و `RejectTransfer` بالطريقة نفسها مع `Approved` / `Rejected`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### التحقق من إمكانية النقل

```csharp
var result = dna.CheckTransfer("example.com", "Ab1!xyZ9");
```

تكون قيمة `result` هي `OK` فقط عندما يُفيد السجل (registry) بأن النطاق قابل للنقل؛ وإلا فهي `ERROR` مع `Code` = `TRANSFER_NOT_AVAILABLE`، وتظل التفاصيل متاحة ضمن `data`.

<details>
<summary>مثال على مخرجات التحقق من إمكانية النقل</summary>

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

#### قائمة النطاقات

```csharp
var result = dna.GetList(new Dictionary<string, object>
{
    ["OrderColumn"]    = "Id",
    ["OrderDirection"] = "ASC",
    ["PageNumber"]     = 0,
    ["PageSize"]       = 1000,
});
```

يبدأ `PageNumber` من الصفر. يمكن أيضًا تمرير أسماء المرشحات الخاصة بالبوابة (`MaxResultCount`، `SkipCount`، `Sorting`، `DomainName`، ...).

<details>
<summary>مثال على مخرجات قائمة النطاقات</summary>

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

#### قائمة امتدادات النطاقات (TLD)

```csharp
var result = dna.GetTldList(100);
```

<details>
<summary>مثال على مخرجات قائمة امتدادات النطاقات</summary>

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

#### التحقق من توفر النطاق

```csharp
var result = dna.CheckAvailability(new[] { "hello", "world123x0" }, new[] { "com", "net" }, 1, "create");
```

<details>
<summary>مثال على مخرجات التحقق من توفر النطاق</summary>

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

عند الفشل تُعيد الدالة غلاف الخطأ المعتاد بدلًا من قائمة، لذا تحقق أولًا من `result is JsonArray`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### تفاصيل النطاق

```csharp
var result = dna.GetDetails("example.com");
var synced = dna.SyncFromRegistry("example.com"); // same envelope
```

<details>
<summary>مثال على مخرجات تفاصيل النطاق</summary>

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

#### تعديل خوادم الأسماء

```csharp
var result = dna.ModifyNameServer("example.com", new[] { "ns1.example.com", "ns2.example.com" });
```

تُزال المسافات من أسماء المضيفين، وتُحوَّل إلى أحرف صغيرة، وتُحذف النقطة في نهايتها؛ كما تُستبعد الإدخالات الفارغة والمكررة.

<details>
<summary>مثال على مخرجات تعديل خوادم الأسماء</summary>

```json
{ "result": "OK", "data": { "NameServers": [ "ns1.example.com", "ns2.example.com" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### تفعيل قفل النطاق

```csharp
var result = dna.EnableTheftProtectionLock("example.com");
```

<details>
<summary>مثال على مخرجات تفعيل قفل النطاق</summary>

```json
{ "result": "OK", "data": { "LockStatus": true } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### إلغاء قفل النطاق

```csharp
var result = dna.DisableTheftProtectionLock("example.com");
```

<details>
<summary>مثال على مخرجات إلغاء قفل النطاق</summary>

```json
{ "result": "OK", "data": { "LockStatus": false } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### إضافة خادم أسماء فرعي

```csharp
var result = dna.AddChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>مثال على مخرجات إضافة خادم أسماء فرعي</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### حذف خادم أسماء فرعي

```csharp
var result = dna.DeleteChildNameServer("example.com", "ns1.example.com");
```

<details>
<summary>مثال على مخرجات حذف خادم أسماء فرعي</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com" } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### تحديث خادم الأسماء الفرعي

```csharp
var result = dna.ModifyChildNameServer("example.com", "ns1.example.com", "1.2.3.4");
```

<details>
<summary>مثال على مخرجات تحديث خادم الأسماء الفرعي</summary>

```json
{ "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "1.2.3.4" ] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### تعديل حالة حماية الخصوصية

```csharp
var result = dna.ModifyPrivacyProtectionStatus("example.com", true);

// The flag may also be text, as panels store it: "1", "true", "on" and "yes" enable privacy
var result2 = dna.ModifyPrivacyProtectionStatus("example.com", "on");
```

المُعامِل الثالث الاختياري (`reason`) محفوظ لأغراض التوافق فقط؛ إذ لا يوجد مثل هذا الحقل لدى البوابة ولا يتم إرساله.

<details>
<summary>مثال على مخرجات تعديل حالة حماية الخصوصية</summary>

```json
{ "data": { "PrivacyProtectionStatus": true }, "result": "OK" }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### الحصول على جهات اتصال النطاق

```csharp
var result = dna.GetContacts("example.com");
```

<details>
<summary>مثال على مخرجات الحصول على جهات اتصال النطاق</summary>

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

#### حفظ جهات اتصال النطاق

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

يمكن أيضًا تمرير جهات الاتصال بالبنية المتداخلة (`Address` ← `Line1`/`City`/...، `Phone` ← `Phone`/`Fax` ← `Number`/`CountryCode`)، وهي البنية التي تُعيدها `GetContacts`.

<details>
<summary>مثال على مخرجات حفظ جهات اتصال النطاق</summary>

```json
{ "result": "OK", "data": { "contacts": [] } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### الحصول على تفاصيل الموزع

```csharp
var result = dna.GetResellerDetails();
```

<details>
<summary>مثال على مخرجات تفاصيل الموزع</summary>

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

#### الرصيد الحالي

```csharp
var result = dna.GetCurrentBalance("USD"); // USD, TRY, EUR or GBP
```

<details>
<summary>مثال على مخرجات الرصيد الحالي</summary>

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

عند الفشل تكون قيمة `OperationResult` هي `FAILED`، ويحتوي `ErrorCode` على رمز الخطأ (مثل `API_429`)، وتكون قيمة `Balance` هي `"0.00"`. يُبلغ الحساب عن أرصدة USD و TRY فقط؛ وطلب عملة أخرى (EUR، GBP، ...) يفشل مع `ErrorCode` = `BALANCE` والرسالة `No EUR balance is reported for this account. Available: USD, TRY.`

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### إدارة منطقة DNS

تخزّن البوابة المنطقة على شكل **مجموعات** سجلات (الاسم + النوع ← القيم) وتستبدل المجموعة كاملة عند كل عملية كتابة. تتولى الدوال `*ResourceRecordSet` و `*ResourceRecordValue` ذلك نيابة عنك. يمكن أن تكون الأسماء نسبية (`www`)، أو الجذر (`@` أو `""` أو `example.com`)، أو مؤهلة بالكامل (`www.example.com.`). الأنواع المدعومة: `A`، `AAAA`، `ALIAS`، `CAA`، `CNAME`، `DS`، `MX`، `NAPTR`، `PTR`، `SRV`، `TLSA`، `TXT`. تُعتمد عمليات الكتابة فورًا ما لم تمرر `apply: false` وتستدعِ `ApplyResourceRecords` بنفسك؛ وإذا فشل ذلك الاعتماد التلقائي، تُعيد الدالة خطأ الاعتماد (ويبقى التغيير مُجهَّزًا وغير مُفعَّل). يُفصل بين استدعاءات المنطقة المتتالية بثانية واحدة على الأقل، لأن البوابة ترد على الطلبات المتلاحقة بـ HTTP 429.

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
<summary>مثال على مخرجات إدارة منطقة DNS</summary>

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

`SetResourceRecordSet` / `AddResourceRecordValue` (يُعرض النوع بأحرف كبيرة و TTL كما أُرسل، ضمن النطاق 1–86400 ثانية):

```json
{ "result": "OK", "data": { "Name": "", "Type": "MX", "TTL": 3600, "Contents": [ "10 mx1.example.com", "20 mx2.example.com" ] } }
```

إضافة قيمة موجودة مسبقًا، أو حذف قيمة غير موجودة، لا تُغيّر شيئًا وتكون الاستجابة `{"result":"OK","data":{..., "Changed": false}}`. ويُرفض النوع غير المدعوم قبل إجراء أي استدعاء:

```json
{ "result": "ERROR", "error": { "Code": "ADD_ZONE", "Message": "Record type \"NS\" is not supported by the DNS zone API. Supported types: A, AAAA, ALIAS, CAA, CNAME, DS, MX, NAPTR, PTR, SRV, TLSA, TXT.", "Details": "..." } }
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### إعادة توجيه عناوين URL

```csharp
dna.SetForwarding("example.com", "https://www.example.org", "Standard"); // or "Frame" (masked)
var forwarding = dna.GetForwarding("example.com");
dna.DeleteForwarding("example.com");
```

<details>
<summary>مثال على مخرجات إعادة توجيه عناوين URL</summary>

```json
{ "result": "OK", "data": { "Enabled": true, "RedirectAddress": "https://www.example.org/landing", "ForwardType": "Frame" } }
```

عند عدم تهيئة أي إعادة توجيه (تجيب البوابة بـ `Redirect information not found`)، تُعيد `GetForwarding` القيمة `{"result":"OK","data":{"Enabled":false,"RedirectAddress":"","ForwardType":""}}`. أما أي خطأ آخر — على سبيل المثال نطاق غير موجود — فيُعاد كخطأ.

تُعيد `DeleteForwarding` القيمة `{"result":"OK","data":{"DomainName":"example.com","Removed":true}}`؛ وعندما لا يكون هناك شيء مُهيَّأ، لا تُجري أي تغيير وتجيب بـ `"Removed": false`.

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

#### أدوات مساعدة (دون اتصال بالشبكة)

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
<summary>مثال على مخرجات ValidateContact</summary>

```json
{
  "FirstName": "Jo", "Phone": "12025550100", "Country": "TR", "LastName": "Isimyok",
  "AddressLine1": "Addres yok", "City": "ISTANBUL", "State": "ISTANBUL", "ZipCode": "34000",
  "PhoneCountryCode": "90", "Fax": "", "FaxCountryCode": ""
}
```

</details>

<hr style="border: 4px solid #000; border-style: dashed;">

### معالجة الأخطاء

لا تُطلق الدوال استثناءات أبدًا عند حدوث مشكلات في API أو الشبكة؛ بل تُعيد غلاف خطأ:

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

| الحالة | `Code` | `Message` |
|---|---|---|
| خطأ HTTP من البوابة | `API_<status>`، مثل `API_400`، `API_429`، `API_500` | رسالة البوابة؛ وعند استجابة فارغة `HTTP 429 - Too Many Requests` |
| بيانات اعتماد خاطئة (HTTP 301/302) | `API_302` | `Invalid API credentials` |
| أخطاء التحقق من الصحة | `API_400` | رسالة البوابة؛ ويسرد `Details` الحقول، مثل `Admin PostalCode - 'PostalCode' is required` |
| انتهاء المهلة | `CURL_28` | `Could not reach the DomainNameAPI service: the connection timed out. (...)` |
| أعطال الشبكة الأخرى (DNS، الاتصال، TLS، إعادة التعيين) | `CURL_6`، `CURL_7`، `CURL_60`، `CURL_56` | `Curl error during request: ...` |
| رفض التسجيل أو التجديد أو النقل (`"success": false`) | `REGISTER_DOMAIN`، `RENEW_DOMAIN`، `TRANSFER_DOMAIN` | رسالة البوابة |
| استجابة نجاح فارغة أو غير قابلة للاستخدام | `RESPONSE` | `Invalid response received from server! Response is empty.` |
| فحوصات تُجرى قبل استدعاء البوابة | مثل `AVAILABILITY`، `ADD_ZONE`، `SET_FORWARD` | وصف للمشكلة |

تشير رموز `API_<n>` دائمًا إلى حالة HTTP. وعندما ترسل البوابة رمز الخطأ الخاص بها، يحمله الغلاف أيضًا في `GatewayCode` (مثل `Dna.DomainService:Domain:10007`)، بحيث يمكنك التمييز بين "النطاق غير موجود" و"امتداد النطاق غير مدعوم" دون تحليل الرسالة. تستخدم أعطال الشبكة `CURL_<n>`، حيث يتبع الرقم ترقيم libcurl لكي تتطابق الرموز مع php-dna. فقط الأخطاء البرمجية (مثل تمرير نوع كائن غير مدعوم) يمكن أن تُطلق استثناءات.

لأغراض استكشاف الأخطاء وإصلاحها، يمكن فحص آخر استدعاء:

```csharp
dna.GetLastFunction();       // "request:GET domains/info"
dna.GetRequestData();        // { "url": ..., "method": ..., "payload": ... }
dna.GetResponseData();       // decoded response, or { "raw_response": "..." }
dna.GetResponseHeaders();    // { "url", "http_code", "content_type", "total_time", "headers" }
dna.GetParsedResponseData(); // parsed response or the error built from it
```

### وضع الاختبار

مرّر `true` كمُعامِل ثالث للدالة الإنشائية لاستخدام بيئة اختبار OTE ‏(`https://ote.domainresellerapi.com/api/v1`) بدلًا من البيئة الحية (`https://api.domainresellerapi.com/api/v1`). تعرض `GetServiceUrl()` العنوان المستخدم، وتتيح `SetServiceUrl(url)` تجاوزه.

### تقارير الأخطاء (القياس عن بُعد)

عندما يفشل استدعاء API، يمكن للمكتبة إرسال تقرير خطأ إلى نظام تتبع الأخطاء الخاص بـ DomainNameAPI ‏(Sentry). كما يمكن إرسال جزء صغير عشوائي من الاستدعاءات الناجحة (4%، بالإضافة إلى كل استدعاء تتجاوز مدته ثانية واحدة) كسجل أداء. يساعد ذلك DomainNameAPI على اكتشاف المشكلات وإصلاحها. تقارير الأخطاء **مفعّلة افتراضيًا**.

**ما يتم إرساله:**

- رمز الخطأ ورسالته
- مسار نقطة النهاية المستدعاة وطريقة HTTP
- مدة الاستدعاء
- اسم التطبيق (`DomainNameAPI-DotNet`) وإصدار المكتبة وإصدار .NET
- معرّف الموزع (Reseller ID) واسم النطاق
- عنوان IP العام للخادم (يتم الحصول عليه عبر `api.ipify.org` في الخلفية)
- اسم حساب نظام التشغيل الذي يعمل التطبيق تحته
- طلب API واستجابته، بما في ذلك بيانات جهات اتصال النطاق؛ ولا تُحجب إلا بيانات الاعتماد — إذ تُستبدل رموز EPP/التفويض ومفاتيح API وكلمات المرور والرموز المميزة (tokens) بـ `***`

**ما لا يُرسَل أبدًا:** رموز EPP/التفويض ومفاتيح API وكلمات المرور والرموز المميزة بصيغة مقروءة؛ ووسائط الدوال في تتبعات المكدس (stack traces).

**لماذا وإلى متى:** تُستخدم البيانات فقط لقياس الاستخدام والتشخيص المبكر للمشكلات، وتُحفظ لمدة 30 يومًا على خادم Sentry الخاص بـ DomainNameAPI، ثم تُحذف تلقائيًا.

تُرسَل التقارير في الخلفية: فهي لا تحجب استدعاءك ولا تبطئه أبدًا، ولا تُطلق استثناءات أبدًا. ولا يتم الإبلاغ عن الأخطاء المتوقعة كثيرة التكرار (نطاق غير موجود، رصيد غير كافٍ، بيانات اعتماد غير صالحة، عدم تهيئة إعادة توجيه، ...).

**كيفية إيقافها:** استدعِ `SetErrorReportingEnabled(false)` بعد إنشاء العميل:

```csharp
var dna = new DomainNameApiClient(resellerId, apiKey);
dna.SetErrorReportingEnabled(false);
```

**الوضع المصغّر:** تُبقي `SetTelemetryMinimal(true)` الإبلاغ مفعّلًا لكنها لا ترسل سوى رمز الخطأ ورسالته، ونقطة النهاية، والتوقيت، والإصدارات، ومعرّف الموزع — دون بيانات الطلب/الاستجابة أو النطاق أو عنوان IP أو اسم الحساب.

يمكن تغيير الوجهة عبر الخاصية `TelemetryDsn` (على سبيل المثال إلى مشروع Sentry الخاص بك).

> 🔒 **الأمان:** التحقق من شهادة TLS مفعّل. على خادم يحتوي على حزمة CA قديمة يمكن إيقافه باستخدام `dna.SetSslVerification(false);` — وهذا غير مستحسن؛ فتحديث حزمة CA هو الحل الأفضل.

### إعدادات متقدمة

- **المهلات الزمنية:** ‏30 ثانية لكل طلب و10 ثوانٍ للاتصال؛ ويمكن تغييرها عبر الخاصيتين `Timeout` و `ConnectTimeout`.
- **طبقة نقل مخصصة:** مرّر `IHttpTransport` إلى الدالة الإنشائية (`new DomainNameApiClient(id, key, false, transport)`) أو عيّن الخاصية `Transport` لتوجيه الطلبات عبر حزمة HTTP الخاصة بك أو عبر وكيل (proxy) أو بديل اختباري (test double). يجب أن تُعيد التطبيقات كل حالات HTTP (دون اتباع عمليات إعادة التوجيه) وأن تُطلق `TransportException` عند أعطال الشبكة.
- **الخيوط (Threads):** يحتفظ العميل بآخر طلب واستجابة لأغراض التشخيص، وهو غير آمن للاستخدام المتزامن بين الخيوط. استخدم نسخة واحدة لكل خيط (أو لكل طلب) أو قم بمزامنة الوصول. تُحرر `Dispose()` كائن `HttpClient` الأساسي.
- **User-Agent:** تُرسَل الطلبات بالقيمة `DomainNameAPI-DotNet/1.0.1`.

### الأمثلة

يحتوي مشروع [examples](examples/DomainNameApi.Examples) على ملف قابل للتشغيل لكل دالة (`GetDetails`، `RegisterWithContactInfo`، `RegisterWithContactInfo-TR`، ودوال منطقة DNS وإعادة التوجيه، ...). عيّن بيانات الاعتماد الخاصة بك وشغّل أحدها:

```bash
export DNA_RESELLER_ID=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
export DNA_API_KEY=your-api-key
export DNA_TEST_MODE=true   # optional: use the OTE test environment

dotnet run --project examples/DomainNameApi.Examples -- GetDetails
dotnet run --project examples/DomainNameApi.Examples            # lists all examples
```

### الاختبارات

لا تحتاج الاختبارات إلى بيانات اعتماد ولا تُجري أي استدعاءات شبكة خارجية:

```bash
dotnet test
```

في حال عدم توفر .NET SDK محليًا، شغّلها داخل Docker:

```bash
docker run --rm -v "$PWD":/src -w /src mcr.microsoft.com/dotnet/sdk:10.0 dotnet test
```

| ملف الاختبار | الوصف |
|-----------|-------------|
| `tests/DomainNameApi.Tests/ConformanceTests.cs` | يعيد تشغيل جميع ملفات المطابقة الـ 134 الخاصة بـ php-dna ‏(`testdata/conformance/`) ويقارن الطلبات والنتائج |
| `tests/DomainNameApi.Tests/HttpLayerTests.cs` | الترويسات، وعناوين URL، وسلاسل الاستعلام، ووضع الاختبار، والمهلات الزمنية، وتنظيم وتيرة طلبات المنطقة |
| `tests/DomainNameApi.Tests/HttpClientTransportTests.cs` | طبقة النقل الافتراضية `HttpClient` مقابل مستمع محلي |
| `tests/DomainNameApi.Tests/ErrorMappingTests.cs` | ربط أخطاء البوابة ورسائل التحقق من الصحة |
| `tests/DomainNameApi.Tests/TelemetryTests.cs` | حمولات القياس عن بُعد، وحجب بيانات الاعتماد، والوضع المصغّر |
| `tests/DomainNameApi.Tests/UtilityTests.cs` | التحقق من جهات الاتصال، وتوحيد صيغ الهاتف/الدولة، والتنسيق المتوافق مع PHP |

<hr style="border: 4px solid #000; border-style: dashed;">

## رموز العودة والأخطاء مع تفسيراتها

| الرمز | الوصف | التفاصيل |
|------|------|------|
| 1000 | تم تنفيذ الأمر بنجاح | العملية ناجحة. |
| 1001 | تم تنفيذ الأمر بنجاح؛ الإجراء معلّق | العملية ناجحة. إلا أن العملية حاليًا في قائمة الانتظار لإكمالها. |
| 2003 | مُعامِل مطلوب مفقود | خطأ مُعامِل مفقود. على سبيل المثال، عدم إدخال رقم الهاتف في معلومات الاتصال. |
| 2105 | الكائن غير مؤهل للتجديد | حالة النطاق غير مناسبة للتجديد، فهو مقفل أمام عمليات التحديث. يجب ألا تكون الحالة "clientupdateprohibited". قد يعود ذلك إلى حالات أخرى. |
| 2200 | خطأ في المصادقة | خطأ في التفويض، رمز الأمان غير صحيح أو النطاق مسجل لدى مسجّل آخر. |
| 2302 | الكائن موجود | اسم النطاق أو معلومات خادم الأسماء موجودة مسبقًا في قاعدة البيانات. لا يمكن التسجيل. |
| 2303 | الكائن غير موجود | اسم النطاق أو معلومات خادم الأسماء غير موجودة في قاعدة البيانات. يلزم تسجيل جديد. |
| 2304 | حالة الكائن تمنع العملية | حالة النطاق غير مناسبة للتحديث، فهو مقفل أمام عمليات التحديث. يجب ألا تكون الحالة "clientupdateprohibited". قد يعود ذلك إلى حالات أخرى. |

| الرمز | التفاصيل |
|------|------|
| 101  | خطأ في النظام، التفاصيل لـ ({0})! |
| 102  | أخطاء متعددة، التفاصيل لـ ({0})! |
| 103  | أخطاء غير معروفة، التفاصيل لـ ({0})! |
| 200  | أمر API غير موجود ({0})! |
| 210  | خدمة API غير موجودة ({0})! |
| 211  | لم يتم تعيين مزود خدمة API! |
| 300  | الموزع غير موجود! |
| 301  | عنوان IP الحالي الخاص بك غير مخوّل بالوصول. يرجى التأكد من أنك تتصل من عنوان IP مخوّل ثم المحاولة مرة أخرى |
| 310  | امتداد النطاق (TLD) غير مدعوم! |
| 320  | API غير موجود! |
| 321  | العملة غير مدعومة! |
| 330  | لم يتم تعيين المُعامِلات المطلوبة ({0}). |
| –    | تأكد من إرسال جميع كائنات جهات الاتصال كاملة |
| 340  | تعريف السعر غير موجود ({0}[{1}]-{2}{3}). |
| 350  | رصيد الموزع غير كافٍ. (معرّف الموزع : {0} - الرصيد الحالي : {1} {2}). |
| 350  | عملة المحاسبة غير متطابقة أو الرصيد غير كافٍ. |
| 360  | طلب API غير صالح للحقل ({0}). |
| 360  | تم تجاوز حصة API! |
| 361  | خطأ تقييد المعدل! |
| 362  | النطاق المميز (Premium) غير متاح للتسجيل حاليًا. |
| 363  | أُلغيت العملية لأن النطاق في فترة التجديد التلقائي |
| 364  | هذا النطاق غير متاح حاليًا لإجراء المعاملات بسبب مشكلة لدى السجل (registry) |

| الرمز | التفاصيل |
|------|------|
| 400  | جهة اتصال غير صالحة ({0}). |
| 401  | تتعذر مزامنة معلومات الاتصال. |
| 402  | لا يوجد وصول إلى معلومات الاتصال من السجل. |
| 403  | توجد معلومات ناقصة في النظام. أدخل معلوماتك الافتراضية أو تواصل مع فريق الدعم. |
| 404  | يتعذر تحديث جهة اتصال النطاق. يلزم إذن من السجل. |
| 410  | جهة الاتصال غير موجودة. |
| 410  | جهة الاتصال غير موجودة ({0}). |
| 420  | أمر Api غير صالح لجهة الاتصال {0}. |
| 430  | واجهة API لجهات الاتصال غير موجودة. |
| 440  | جهة الاتصال غير متزامنة. |
| 450  | عدد جهات اتصال النطاق كبير جدًا! |
| 451  | تعذّر متابعة تحديث جهة الاتصال. |

| الرمز | التفاصيل |
|------|------|
| 500  | معرّف النطاق غير صالح. |
| 500  | معرّف النطاق غير صالح ({0}). |
| 501  | تعذّرت مزامنة النطاق ({0}) |
| 502  | فشل النقل الداخلي |
| 503  | تسجيل النطاق غير متاح. |
| 504  | معلومات النطاق غير متطابقة. القيمة السابقة: {0} |
| 505  | أدخل عنوان IP. |
| 506  | بدأ نقل النطاق ولكن تعذّرت قراءة معلومات الاتصال. |
| 507  | خطأ في التفويض. |
| 510  | النطاق غير موجود. |
| 511  | لا يمكن العثور على النطاقات منتهية الصلاحية. |
| 512  | النطاق غير قابل للتجديد. |
| 513  | النطاق ليس في حالة قابلة للتحديث. يجب أن يكون نشطًا لكي يتم تحديثه |
| 514  | يُتوقع أن يكون النطاق في فترة الاسترداد (Redemption). |
| 520  | أمر Api غير صالح للنطاق "{0}". |
| 530  | مدة النطاق غير صالحة. يجب أن تكون المدة من {0} إلى {1} سنوات. المدة المطلوبة هي {2} سنوات. |
| 540  | لا يمكن تمديد النطاق لأكثر من {0} سنوات من التاريخ الحالي. |
| 550  | اسم النطاق غير صالح. يجب أن يتراوح طول النطاق بين {0} و{1} حرفًا. طول اسم النطاق المطلوب هو {2} حرفًا. |
| 560  | عدد خوادم الأسماء غير صالح. يجب أن يحتوي النطاق على {0} إلى {1} خوادم أسماء. عدد خوادم الأسماء المطلوب هو {2}. |
| 561  | لم يتم العثور على معلومات خادم الأسماء في الطلب الوارد |
| 570  | أسماء النطاقات الدولية (IDN) غير مدعومة للامتداد "{0}". |
| 571  | المدة غير صالحة. |
| 572  | الأمر غير صالح. |
| 573  | لم يتم العثور على أسماء النطاقات. |
| 574  | لم يتم العثور على أسماء امتدادات النطاقات (TLD). |
| 575  | النطاق ليس في حالة قابلة للتحديث. يجب أن يكون نشطًا لكي يتم تحديث خوادم الأسماء |
| 576  | يمكن تجديد النطاق خلال الشهر الأخير قبل تاريخ انتهاء الصلاحية |
| 580  | النقل غير مدعوم للامتداد "{0}". |
| 581  | خادم الأسماء الفرعي غير موجود |
| 582  | بدأ النقل من قِبل موزع آخر. |
| 583  | لم تتم تهيئة النقل. يرجى التواصل مع فريق الدعم. |
| 584  | حالة الكائن تمنع العملية |
| 590  | رمز التفويض مطلوب لهذا النقل. |
| 591  | رمز التفويض غير صالح. |
| 592  | يوجد قفل نقل على النطاق. |
| 593  | لا يمكن نقل النطاق، يجب أن تستوفي معلومات الحالة وقفل النقل المعايير المطلوبة. (معلومات الحالة: #ok) |
| 594  | تعذّر حل عنوان خادم أسماء النطاق ({0}) |
| 595  | تتعذر قراءة معلومات الاتصال (whois.registrar.tld). يرجى التأكد من أن حالة حماية الخصوصية مفتوحة |
| 596  | تعذّر التحقق من معلومات الاتصال |
| 597  | امتداد النطاق (TLD) غير موجود |
| 598  | حدث خطأ غير معروف |
| 599  | إعادة توجيه النطاق غير موجودة |

## الترخيص

Apache License 2.0 — انظر [LICENSE](LICENSE).
