using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using DomainNameApi.Internal;

namespace DomainNameApi
{
    public sealed partial class DomainNameApiClient
    {
        // ------------------------------------------------------------------
        // Contacts
        // ------------------------------------------------------------------

        /// <summary>The domain's contacts keyed Administrative, Billing, Registrant, Technical.</summary>
        public JsonNode GetContacts(string domainName)
        {
            try
            {
                var domainInfo = Request("GET", "domains/info", new JsonObject { ["DomainName"] = domainName });

                var typeMap = new Dictionary<string, string>
                {
                    ["Admin"] = "Administrative",
                    ["Billing"] = "Billing",
                    ["Tech"] = "Technical",
                    ["Registrant"] = "Registrant",
                };

                var contacts = new Dictionary<string, JsonNode>();
                var list = Php.Get(domainInfo, "contacts");
                if (list != null && Php.IsArray(list))
                {
                    foreach (var item in Php.Values(list))
                    {
                        var apiType = Php.ToStr(Php.Coalesce(Php.Get(item, "contactType"), Php.Str("")));
                        var key = typeMap.TryGetValue(apiType, out var k) ? k : Php.UcFirst(Php.Lower(apiType));
                        var merged = ToMap(Php.Clone(item) ?? new JsonObject());
                        if (!(merged is JsonObject mergedObj))
                        {
                            mergedObj = new JsonObject();
                        }
                        mergedObj["type"] = key;
                        contacts[key] = ParseContactInfo(mergedObj);
                    }
                }

                var ordered = new JsonObject();
                foreach (var key in new[] { "Administrative", "Billing", "Registrant", "Technical" })
                {
                    if (contacts.TryGetValue(key, out var c) && c != null)
                    {
                        ordered[key] = c;
                    }
                }

                return new JsonObject
                {
                    ["data"] = new JsonObject { ["contacts"] = ordered },
                    ["result"] = ResultOk,
                };
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "GET_CONTACTS");
            }
        }

        /// <summary>
        /// Save all four contacts of a domain. <paramref name="contacts"/> maps
        /// Administrative/Billing/Technical/Registrant to contact maps.
        /// </summary>
        public JsonNode SaveContacts(string domainName, IDictionary<string, object> contacts)
        {
            try
            {
                var payloadContacts = BuildPayloadContacts(Php.FromObject(contacts));
                var response = Request("PUT", "domains/contacts/update", new JsonObject { ["domainName"] = domainName, ["contacts"] = payloadContacts });

                var parsed = new JsonObject();
                var list = Php.Get(response, "contacts");
                if (list != null && Php.IsArray(list))
                {
                    foreach (var contact in Php.Values(list))
                    {
                        parsed[Php.UcFirst(Php.Lower(Php.ToStr(Php.Get(contact, "type"))))] = ParseContactInfo(contact);
                    }
                }

                return Ok(new JsonObject { ["contacts"] = parsed });
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "SAVE_CONTACTS");
            }
        }

        private JsonArray BuildPayloadContacts(JsonNode contacts)
        {
            var payload = new JsonArray();
            foreach (var kv in Php.Entries(contacts))
            {
                payload.Add(ParseContact(kv.Value, Php.UcFirst(Php.Lower(kv.Key))));
            }
            return payload;
        }

        /// <summary>Gateway contact → legacy contact structure.</summary>
        private static JsonNode ParseContactInfo(JsonNode data)
        {
            if (Php.Empty(data))
            {
                return new JsonArray();
            }
            JsonNode G(params string[] keys) => Php.Clone(Php.Coalesce(keys.Select(k => Php.Get(data, k)).Concat(new[] { Php.Str("") }).ToArray()));

            return new JsonObject
            {
                ["ID"] = G("handle", "id"),
                ["Status"] = Php.Clone(Php.Coalesce(Php.Get(data, "status"), Php.Str("Active"))),
                ["AuthCode"] = G("authCode"),
                ["FirstName"] = G("firstName"),
                ["LastName"] = G("lastName"),
                ["Company"] = G("companyName", "organizationName", "company"),
                ["EMail"] = G("eMail", "emailAddress", "email"),
                ["Type"] = G("type"),
                ["Address"] = new JsonObject
                {
                    ["Line1"] = G("address", "addressLine1"),
                    ["Line2"] = G("addressLine2"),
                    ["Line3"] = G("addressLine3"),
                    ["State"] = G("state", "stateOrProvince"),
                    ["City"] = G("city"),
                    ["Country"] = G("country", "countryCode"),
                    ["ZipCode"] = G("postalCode", "zipCode"),
                },
                ["Phone"] = new JsonObject
                {
                    ["Phone"] = new JsonObject
                    {
                        ["Number"] = G("phone", "phoneNumber"),
                        ["CountryCode"] = G("phoneCountryCode"),
                    },
                    ["Fax"] = new JsonObject
                    {
                        ["Number"] = G("fax", "faxNumber"),
                        ["CountryCode"] = G("faxCountryCode"),
                    },
                },
                ["Additional"] = Php.Clone(Php.Coalesce(Php.Get(data, "additionalAttributes"), new JsonArray())),
            };
        }

        /// <summary>Legacy contact (flat or nested) → gateway ContactLiteDto.</summary>
        private JsonObject ParseContact(JsonNode contact, string type)
        {
            var typeMap = new Dictionary<string, string>
            {
                ["Administrative"] = "Admin",
                ["Technical"] = "Tech",
                ["Billing"] = "Billing",
                ["Registrant"] = "Registrant",
            };
            var apiType = typeMap.TryGetValue(type, out var t) ? t : type;

            JsonNode G(params string[] path) => Php.Get(contact, path);

            var addressNode = Php.Coalesce(G("AddressLine1"), G("Address", "Line1"), Php.Str(""));
            var line2 = Php.Coalesce(G("AddressLine2"), G("Address", "Line2"), Php.Str(""));
            JsonNode address = addressNode;
            if (!Php.Empty(line2))
            {
                address = Php.Str(Php.ToStr(addressNode) + " " + Php.ToStr(line2));
            }

            var phone = Php.Coalesce(G("Phone"), G("Phone", "Phone", "Number"), Php.Str(""));
            var phoneCc = Php.Coalesce(G("PhoneCountryCode"), G("Phone", "Phone", "CountryCode"), Php.Str(""));
            var fax = Php.Coalesce(G("Fax"), G("Phone", "Fax", "Number"), Php.Str(""));
            var faxCc = Php.Coalesce(G("FaxCountryCode"), G("Phone", "Fax", "CountryCode"), Php.Str(""));

            if (Php.IsArray(phone))
            {
                phoneCc = Php.Coalesce(Php.Get(phone, "Phone", "CountryCode"), Php.Str(""));
                phone = Php.Coalesce(Php.Get(phone, "Phone", "Number"), Php.Str(""));
            }
            if (Php.IsArray(fax))
            {
                faxCc = Php.Coalesce(Php.Get(fax, "Fax", "CountryCode"), Php.Str(""));
                fax = Php.Coalesce(Php.Get(fax, "Fax", "Number"), Php.Str(""));
            }

            var input = new JsonObject
            {
                ["FirstName"] = Php.Clone(Php.Coalesce(G("FirstName"), Php.Str(""))),
                ["LastName"] = Php.Clone(Php.Coalesce(G("LastName"), Php.Str(""))),
                ["AddressLine1"] = Php.Clone(address),
                ["City"] = Php.Clone(Php.Coalesce(G("City"), G("Address", "City"), Php.Str(""))),
                ["State"] = Php.Clone(Php.Coalesce(G("State"), G("Address", "State"), Php.Str(""))),
                ["Country"] = Php.Clone(Php.Coalesce(G("Country"), G("Address", "Country"), Php.Str(""))),
                ["ZipCode"] = Php.Clone(Php.Coalesce(G("ZipCode"), G("Address", "ZipCode"), Php.Str(""))),
                ["Phone"] = Php.Clone(phone),
                ["PhoneCountryCode"] = Php.Clone(phoneCc),
                ["Fax"] = Php.Clone(fax),
                ["FaxCountryCode"] = Php.Clone(faxCc),
            };
            var validated = ValidateContactNode(input);

            return new JsonObject
            {
                ["contactType"] = apiType,
                ["firstName"] = Php.Clone(validated["FirstName"]),
                ["lastName"] = Php.Clone(validated["LastName"]),
                ["companyName"] = Php.Clone(Php.Coalesce(G("Company"), Php.Str(""))),
                ["eMail"] = Php.Clone(Php.Coalesce(G("EMail"), Php.Str(""))),
                ["address"] = Php.Clone(validated["AddressLine1"]),
                ["city"] = Php.Clone(validated["City"]),
                ["state"] = Php.Clone(validated["State"]),
                ["country"] = Php.Clone(validated["Country"]),
                ["postalCode"] = Php.Clone(validated["ZipCode"]),
                ["phoneCountryCode"] = Php.Clone(validated["PhoneCountryCode"]),
                ["phone"] = Php.Clone(validated["Phone"]),
                ["faxCountryCode"] = Php.Clone(validated["FaxCountryCode"]),
                ["fax"] = Php.Clone(validated["Fax"]),
                ["isHidden"] = Php.ToBool(Php.Coalesce(G("IsHidden"), Php.Bool(false))),
            };
        }

        // ------------------------------------------------------------------
        // Utilities (no network)
        // ------------------------------------------------------------------

        /// <summary>True for .tr domains (case-insensitive).</summary>
        public bool IsTrTLD(string domain) => Php.Lower(Php.Last(domain ?? "", 3)) == ".tr";

        /// <summary>
        /// Complete and normalize a contact the way registration does: placeholder
        /// values for missing fields, names stripped of digits/punctuation, state
        /// falling back to the city, ISO country codes, phone/fax split into
        /// number and country code.
        /// </summary>
        public JsonNode ValidateContact(IDictionary<string, object> contact)
        {
            return ValidateContactNode(ToMap(Php.FromObject(contact) ?? new JsonObject()) as JsonObject ?? new JsonObject());
        }

        /// <summary>Same as <see cref="ValidateContact(IDictionary{string, object})"/> for a contact already held as JSON.</summary>
        public JsonNode ValidateContact(JsonObject contact)
        {
            return ValidateContactNode((JsonObject)(contact?.DeepClone() ?? new JsonObject()));
        }

        private static readonly (string Key, string Value)[] ContactDefaults =
        {
            ("FirstName", "Isimyok"),
            ("LastName", "Isimyok"),
            ("AddressLine1", "Addres yok"),
            ("City", "ISTANBUL"),
            ("State", "ISTANBUL"),
            ("Country", "TR"),
            ("ZipCode", "34000"),
            ("Phone", "5555555555"),
            ("PhoneCountryCode", "90"),
        };

        private static JsonObject ValidateContactNode(JsonObject contact)
        {
            string Def(string key) => ContactDefaults.First(d => d.Key == key).Value;
            bool Blank(string key) => Php.StrLen(Php.Trim(Php.ToStr(contact[key]))) == 0;

            foreach (var (key, value) in ContactDefaults)
            {
                if (Php.Get(contact, key) == null)
                {
                    contact[key] = value;
                }
            }

            contact["FirstName"] = SanitizePersonName(Php.ToStr(contact["FirstName"]));
            contact["LastName"] = SanitizePersonName(Php.ToStr(contact["LastName"]));

            if (Blank("FirstName")) contact["FirstName"] = Def("FirstName");
            if (Blank("LastName")) contact["LastName"] = Php.Clone(contact["FirstName"]);
            if (Blank("AddressLine1")) contact["AddressLine1"] = Def("AddressLine1");
            if (Blank("City")) contact["City"] = Def("City");
            if (Blank("State")) contact["State"] = Php.Clone(contact["City"]);
            contact["Country"] = NormalizeCountryCode(Php.ToStr(contact["Country"]));
            if (Blank("Country")) contact["Country"] = Def("Country");
            if (Blank("ZipCode")) contact["ZipCode"] = Def("ZipCode");

            var (phone, phoneCc) = NormalizePhone(Php.ToStr(contact["Phone"]), Php.ToStr(contact["PhoneCountryCode"]));
            contact["Phone"] = phone;
            contact["PhoneCountryCode"] = phoneCc;
            if (Blank("PhoneCountryCode")) contact["PhoneCountryCode"] = Def("PhoneCountryCode");
            if (Blank("Phone")) contact["Phone"] = Def("Phone");

            var (fax, faxCc) = NormalizePhone(
                Php.ToStr(Php.Coalesce(Php.Get(contact, "Fax"), Php.Str(""))),
                Php.ToStr(Php.Coalesce(Php.Get(contact, "FaxCountryCode"), Php.Str(""))));
            contact["Fax"] = fax;
            contact["FaxCountryCode"] = fax == "" ? "" : faxCc;

            return contact;
        }

        private static readonly Regex NameStrip = new Regex(@"[^\p{L}\s'\-]", RegexOptions.CultureInvariant);
        private static readonly Regex Spaces = new Regex(@"\s+", RegexOptions.CultureInvariant);

        /// <summary>Keep letters (any alphabet), spaces, hyphens and apostrophes.</summary>
        internal static string SanitizePersonName(string name)
        {
            var clean = NameStrip.Replace(name ?? "", "");
            return Php.Trim(Spaces.Replace(clean, " "));
        }

        private static readonly Dictionary<string, string> CountryNames = new Dictionary<string, string>
        {
            ["turkey"] = "TR", ["türkiye"] = "TR", ["turkiye"] = "TR",
            ["united states"] = "US", ["united states of america"] = "US", ["usa"] = "US",
            ["united kingdom"] = "GB", ["great britain"] = "GB", ["england"] = "GB",
            ["germany"] = "DE", ["deutschland"] = "DE", ["france"] = "FR", ["spain"] = "ES",
            ["italy"] = "IT", ["netherlands"] = "NL", ["india"] = "IN", ["china"] = "CN",
            ["russia"] = "RU", ["canada"] = "CA", ["australia"] = "AU", ["brazil"] = "BR",
        };

        /// <summary>Country → 2-letter ISO code (a few common names are mapped).</summary>
        internal static string NormalizeCountryCode(string country)
        {
            country = Php.Trim(country ?? "");
            if (country == "") return "";
            if (Php.StrLen(country) == 2) return Php.Upper(country);
            return CountryNames.TryGetValue(Php.Lower(country), out var code) ? code : country;
        }

        private static readonly Regex IntlPrefix = new Regex(@"^[ \t\n\r\v\f]*(\+|00)", RegexOptions.CultureInvariant);
        private static readonly Regex NonDigit = new Regex(@"[^0-9]", RegexOptions.CultureInvariant);

        /// <summary>Split a free-form phone number into (subscriber number, country code).</summary>
        internal static (string Number, string CountryCode) NormalizePhone(string number, string countryCode)
        {
            var hasIntlPrefix = IntlPrefix.IsMatch(number ?? "");
            var digits = NonDigit.Replace(number ?? "", "");
            var cc = NonDigit.Replace(countryCode ?? "", "");

            if (digits == "")
            {
                return ("", cc);
            }

            if (hasIntlPrefix)
            {
                if (digits.StartsWith("00", StringComparison.Ordinal))
                {
                    digits = digits.Substring(2);
                }
                if (cc != "" && digits.StartsWith(cc, StringComparison.Ordinal) && digits.Length > cc.Length)
                {
                    digits = digits.Substring(cc.Length);
                }
            }
            else if (cc != "" && digits.StartsWith(cc, StringComparison.Ordinal) && digits.Length > 10)
            {
                digits = digits.Substring(cc.Length);
            }

            if (cc == "")
            {
                if (digits.Length == 12 && digits.StartsWith("90", StringComparison.Ordinal))
                {
                    cc = "90";
                    digits = digits.Substring(2);
                }
                else
                {
                    cc = "90";
                }
            }

            if (digits.Length > 1 && digits[0] == '0')
            {
                digits = digits.TrimStart('0');
            }

            return (digits, cc);
        }

        // ------------------------------------------------------------------
        // Dates
        // ------------------------------------------------------------------

        private static readonly string[] DateFormats =
        {
            "yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK",
            "yyyy-MM-dd'T'HH:mm:ssK",
            "yyyy-MM-dd'T'HH:mmK",
            "yyyy-MM-dd HH:mm:ss.FFFFFFFK",
            "yyyy-MM-dd HH:mm:ssK",
            "yyyy-MM-dd HH:mmK",
            "yyyy-MM-ddK",
            "yyyy-MM-dd",
            "r",
        };

        /// <summary>
        /// strtotime() for the gateway's ISO-8601 dates. Values without an offset are
        /// taken as UTC; values with one are converted to UTC.
        /// </summary>
        internal static bool TryParseApiDate(JsonNode value, out DateTime utc)
        {
            utc = default;
            if (value == null) return false;
            var text = Php.Trim(Php.ToStr(value));
            if (text == "") return false;
            // Fractions longer than 7 digits are legal ISO-8601 but not for .NET.
            text = Regex.Replace(text, @"(\.\d{7})\d+", "$1");
            if (DateTimeOffset.TryParseExact(text, DateFormats, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dto))
            {
                utc = dto.UtcDateTime;
                return true;
            }
            return false;
        }

        /// <summary>formatApiDate(): 'Y-m-d\TH:i:s', or "" when missing or unparseable.</summary>
        private static string FormatApiDate(JsonNode value)
        {
            if (value == null || IsEmptyString(value)) return "";
            return TryParseApiDate(value, out var utc) ? utc.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture) : "";
        }

        /// <summary>Days until the date (rounded up), never negative; 0 when missing or unparseable.</summary>
        private static long RemainingDaysFromDate(JsonNode value)
        {
            if (value == null || IsEmptyString(value)) return 0;
            if (!TryParseApiDate(value, out var utc)) return 0;
            var seconds = Math.Floor((utc - DateTime.UtcNow).TotalSeconds);
            return Math.Max(0, (long)Math.Ceiling(seconds / 86400.0));
        }
    }
}
