using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using DomainNameApi.Internal;

namespace DomainNameApi
{
    public sealed partial class DomainNameApiClient
    {
        // ------------------------------------------------------------------
        // DNS zone
        // ------------------------------------------------------------------

        /// <summary>
        /// All resource records of the domain's zone, one row per value:
        /// <c>{Name, Type, TTL, Content, Disabled}</c>. Names come back fully
        /// qualified with a trailing dot (<c>www.example.com.</c>).
        /// </summary>
        public JsonNode GetResourceRecords(string domainName)
        {
            try
            {
                var response = Request("GET", "domains/zones", new JsonObject { ["domainName"] = domainName });

                var records = new JsonArray();
                foreach (var set in Php.Values(Php.ToArray(response)))
                {
                    if (!Php.IsArray(set)) continue;
                    var name = Php.Coalesce(Php.Get(set, "name"), Php.Str(""));
                    var type = Php.Coalesce(Php.Get(set, "type"), Php.Str(""));
                    var ttl = Php.ToInt(Php.Coalesce(Php.Get(set, "ttl"), Php.Int(0)));
                    var rows = Php.Coalesce(Php.Get(set, "records"), new JsonArray());
                    if (!Php.IsArray(rows) || Php.Empty(rows))
                    {
                        records.Add(Record(name, type, ttl, Php.Str(""), false));
                        continue;
                    }
                    foreach (var rec in Php.Values(rows))
                    {
                        records.Add(Record(name, type, ttl,
                            Php.Coalesce(Php.Get(rec, "content"), Php.Str("")),
                            !Php.Empty(Php.Get(rec, "disabled"))));
                    }
                }

                return Ok(new JsonObject { ["records"] = records });
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "GET_ZONE");
            }
        }

        private static JsonObject Record(JsonNode name, JsonNode type, long ttl, JsonNode content, bool disabled) => new JsonObject
        {
            ["Name"] = Php.Clone(name),
            ["Type"] = Php.Clone(type),
            ["TTL"] = Php.Int(ttl),
            ["Content"] = Php.Clone(content),
            ["Disabled"] = disabled,
        };

        /// <summary>
        /// Add a record set. <paramref name="content"/> is one value or a list of
        /// values. Note that the gateway replaces an existing name+type set; use
        /// <see cref="AddResourceRecordValue"/> to append a value instead.
        /// </summary>
        public JsonNode AddResourceRecord(string domainName, string name, string type, object content, int ttl = 3600, bool apply = true)
        {
            var contentNode = Php.FromObject(content);
            try
            {
                var why = UnsupportedZoneType(type);
                if (why != null)
                {
                    return new JsonObject { ["result"] = ResultError, ["error"] = SetError("ADD_ZONE", why, why) };
                }

                name = RelativizeRecordName(name, domainName);
                var body = new JsonObject { ["zoneStruct"] = BuildZoneStruct(name, type, contentNode, ttl) };
                Request("POST", "domains/zones?" + Php.BuildQuery(new JsonObject { ["domainName"] = domainName }), body);

                if (apply)
                {
                    // A failed apply leaves the change staged but not live: report the apply error.
                    var applied = ApplyResourceRecords(domainName);
                    if (Php.ToStr(Php.Get(applied, "result")) != ResultOk) return applied;
                }

                return Ok(new JsonObject
                {
                    ["Name"] = name,
                    ["Type"] = Php.Clone(body["zoneStruct"]["type"]),
                    ["TTL"] = Php.Clone(body["zoneStruct"]["ttl"]),
                    ["Content"] = Php.Clone(contentNode),
                });
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "ADD_ZONE");
            }
        }

        /// <summary>Edit the record set identified by <paramref name="recordName"/>.</summary>
        public JsonNode EditResourceRecord(string domainName, string recordName, string name, string type, object content, int ttl = 3600, bool apply = true)
        {
            var contentNode = Php.FromObject(content);
            try
            {
                var why = UnsupportedZoneType(type);
                if (why != null)
                {
                    return new JsonObject { ["result"] = ResultError, ["error"] = SetError("EDIT_ZONE", why, why) };
                }

                name = RelativizeRecordName(name, domainName);
                var body = new JsonObject { ["zoneStruct"] = BuildZoneStruct(name, type, contentNode, ttl) };
                var query = Php.BuildQuery(new JsonObject { ["domainName"] = domainName, ["recordName"] = recordName });
                Request("PUT", "domains/zones?" + query, body);

                if (apply)
                {
                    // A failed apply leaves the change staged but not live: report the apply error.
                    var applied = ApplyResourceRecords(domainName);
                    if (Php.ToStr(Php.Get(applied, "result")) != ResultOk) return applied;
                }

                return Ok(new JsonObject
                {
                    ["Name"] = name,
                    ["Type"] = Php.Clone(body["zoneStruct"]["type"]),
                    ["TTL"] = Php.Clone(body["zoneStruct"]["ttl"]),
                    ["Content"] = Php.Clone(contentNode),
                });
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "EDIT_ZONE");
            }
        }

        /// <summary>
        /// Delete a record set. The gateway removes the whole name+type set, not
        /// only the given value; see <see cref="RemoveResourceRecordValue"/>.
        /// </summary>
        public JsonNode DeleteResourceRecord(string domainName, string name, string record, string recordType, bool apply = true)
        {
            try
            {
                Request("DELETE", "domains/zones", new JsonObject
                {
                    ["domainName"] = domainName,
                    ["Name"] = name,
                    ["Record"] = record,
                    ["RecordType"] = recordType,
                });

                if (apply)
                {
                    // A failed apply leaves the change staged but not live: report the apply error.
                    var applied = ApplyResourceRecords(domainName);
                    if (Php.ToStr(Php.Get(applied, "result")) != ResultOk) return applied;
                }

                return Ok(new JsonObject { ["Name"] = name, ["Type"] = recordType, ["Content"] = record });
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "DELETE_ZONE");
            }
        }

        /// <summary>Commit staged zone changes.</summary>
        public JsonNode ApplyResourceRecords(string domainName)
        {
            try
            {
                Request("POST", "domains/zones/apply?" + Php.BuildQuery(new JsonObject { ["domainName"] = domainName }), new JsonArray());
                return Ok(new JsonObject { ["DomainName"] = domainName });
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "APPLY_ZONE");
            }
        }

        /// <summary>
        /// Make the name+type set hold exactly <paramref name="contents"/> (an empty
        /// list removes the set).
        /// </summary>
        public JsonNode SetResourceRecordSet(string domainName, string name, string type, IEnumerable<string> contents, int ttl = 3600, bool apply = true)
        {
            try
            {
                var why = UnsupportedZoneType(type);
                if (why != null)
                {
                    return new JsonObject { ["result"] = ResultError, ["error"] = SetError("SET_ZONE", why, why) };
                }

                name = RelativizeRecordName(name, domainName);
                var values = (contents ?? Enumerable.Empty<string>()).Select(v => v ?? "").Where(v => Php.Trim(v) != "").ToList();

                if (values.Count == 0)
                {
                    return DeleteResourceRecordSet(domainName, name, type, apply);
                }

                var contentNode = new JsonArray(values.Select(Php.Str).ToArray());
                var body = new JsonObject { ["zoneStruct"] = BuildZoneStruct(name, type, contentNode, ttl) };
                Request("POST", "domains/zones?" + Php.BuildQuery(new JsonObject { ["domainName"] = domainName }), body);

                if (apply)
                {
                    // A failed apply leaves the change staged but not live: report the apply error.
                    var applied = ApplyResourceRecords(domainName);
                    if (Php.ToStr(Php.Get(applied, "result")) != ResultOk) return applied;
                }

                return Ok(new JsonObject
                {
                    ["Name"] = name,
                    ["Type"] = Php.Clone(body["zoneStruct"]["type"]),
                    ["TTL"] = Php.Clone(body["zoneStruct"]["ttl"]),
                    ["Contents"] = new JsonArray(values.Select(Php.Str).ToArray()),
                });
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "SET_ZONE");
            }
        }

        /// <summary>
        /// Remove every value under name+type. Without <paramref name="knownValue"/>
        /// the zone is read first to find one; a missing set is reported as OK
        /// with <c>Removed=false</c>.
        /// </summary>
        public JsonNode DeleteResourceRecordSet(string domainName, string name, string type, bool apply = true, string knownValue = "")
        {
            try
            {
                type = Php.Upper(Php.Trim(type ?? ""));
                var value = knownValue ?? "";

                if (value == "")
                {
                    var current = GetResourceRecords(domainName);
                    if (Php.ToStr(Php.Get(current, "result")) != ResultOk)
                    {
                        return current;
                    }
                    var fqdn = QualifyRecordName(name, domainName);
                    foreach (var rec in Php.Values(Php.Get(current, "data", "records")))
                    {
                        if (Php.EqualsIgnoreCase(Php.ToStr(Php.Get(rec, "Name")), fqdn) && Php.EqualsIgnoreCase(Php.ToStr(Php.Get(rec, "Type")), type))
                        {
                            value = Php.ToStr(Php.Get(rec, "Content"));
                            break;
                        }
                    }
                    if (value == "")
                    {
                        return Ok(new JsonObject { ["Name"] = name, ["Type"] = type, ["Removed"] = false });
                    }
                }

                return DeleteResourceRecord(domainName, QualifyRecordName(name, domainName), value, type, apply);
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "DELETE_ZONE");
            }
        }

        /// <summary>
        /// Add one value to a record set, keeping the existing ones (read-merge-write).
        /// Adding a value that is already there is a no-op. <paramref name="ttl"/>
        /// null keeps the set's current TTL.
        /// </summary>
        public JsonNode AddResourceRecordValue(string domainName, string name, string type, string value, int? ttl = null, bool apply = true)
        {
            try
            {
                var why = UnsupportedZoneType(type);
                if (why != null)
                {
                    return new JsonObject { ["result"] = ResultError, ["error"] = SetError("ADD_ZONE", why, why) };
                }

                name = RelativizeRecordName(name, domainName);
                var set = ReadResourceRecordSet(domainName, name, type);
                if (set.Error != null)
                {
                    return set.Error;
                }

                foreach (var existing in set.Contents)
                {
                    if (SameRecordValue(type, existing, value))
                    {
                        return Ok(new JsonObject
                        {
                            ["Name"] = name,
                            ["Type"] = Php.Upper(type ?? ""),
                            ["Contents"] = new JsonArray(set.Contents.Select(Php.Str).ToArray()),
                            ["Changed"] = false,
                        });
                    }
                }

                var contents = new List<string>(set.Contents) { value ?? "" };
                return SetResourceRecordSet(domainName, name, type, contents, ttl ?? (set.Ttl != 0 ? (int)set.Ttl : 3600), apply);
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "ADD_ZONE");
            }
        }

        /// <summary>
        /// Remove one value from a record set, keeping the others; the set is deleted
        /// only when that was its last value. Removing an absent value is a no-op.
        /// </summary>
        public JsonNode RemoveResourceRecordValue(string domainName, string name, string type, string value, bool apply = true)
        {
            try
            {
                var why = UnsupportedZoneType(type);
                if (why != null)
                {
                    return new JsonObject { ["result"] = ResultError, ["error"] = SetError("DELETE_ZONE", why, why) };
                }

                name = RelativizeRecordName(name, domainName);
                var set = ReadResourceRecordSet(domainName, name, type);
                if (set.Error != null)
                {
                    return set.Error;
                }

                var remaining = new List<string>();
                var found = false;
                foreach (var existing in set.Contents)
                {
                    if (!found && SameRecordValue(type, existing, value))
                    {
                        found = true;
                        continue;
                    }
                    remaining.Add(existing);
                }

                if (!found)
                {
                    return Ok(new JsonObject
                    {
                        ["Name"] = name,
                        ["Type"] = Php.Upper(type ?? ""),
                        ["Contents"] = new JsonArray(set.Contents.Select(Php.Str).ToArray()),
                        ["Changed"] = false,
                    });
                }

                if (remaining.Count == 0)
                {
                    return DeleteResourceRecordSet(domainName, name, type, apply, value ?? "");
                }

                return SetResourceRecordSet(domainName, name, type, remaining, set.Ttl != 0 ? (int)set.Ttl : 3600, apply);
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "DELETE_ZONE");
            }
        }

        /// <summary>
        /// Replace one value of a record set with another, keeping the rest.
        /// <paramref name="ttl"/> null keeps the set's current TTL.
        /// </summary>
        public JsonNode ReplaceResourceRecordValue(string domainName, string name, string type, string oldValue, string newValue, int? ttl = null, bool apply = true)
        {
            try
            {
                var why = UnsupportedZoneType(type);
                if (why != null)
                {
                    return new JsonObject { ["result"] = ResultError, ["error"] = SetError("EDIT_ZONE", why, why) };
                }

                name = RelativizeRecordName(name, domainName);
                var set = ReadResourceRecordSet(domainName, name, type);
                if (set.Error != null)
                {
                    return set.Error;
                }

                var contents = new List<string>();
                var found = false;
                foreach (var existing in set.Contents)
                {
                    if (!found && SameRecordValue(type, existing, oldValue))
                    {
                        found = true;
                        contents.Add(newValue ?? "");
                        continue;
                    }
                    contents.Add(existing);
                }

                if (!found)
                {
                    var msg = "No " + Php.Upper(type ?? "") + " record \"" + name + "\" with value \"" + oldValue + "\" to edit.";
                    return new JsonObject { ["result"] = ResultError, ["error"] = SetError("EDIT_ZONE", msg, msg) };
                }

                return SetResourceRecordSet(domainName, name, type, contents, ttl ?? (set.Ttl != 0 ? (int)set.Ttl : 3600), apply);
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "EDIT_ZONE");
            }
        }

        private sealed class RecordSet
        {
            public JsonNode Error;
            public long Ttl;
            public List<string> Contents = new List<string>();
        }

        private RecordSet ReadResourceRecordSet(string domainName, string name, string type)
        {
            var current = GetResourceRecords(domainName);
            if (Php.ToStr(Php.Get(current, "result")) != ResultOk)
            {
                return new RecordSet { Error = current };
            }

            var fqdn = QualifyRecordName(name, domainName);
            type = Php.Upper(Php.Trim(type ?? ""));
            var set = new RecordSet();
            foreach (var rec in Php.Values(Php.Get(current, "data", "records")))
            {
                if (!Php.EqualsIgnoreCase(Php.ToStr(Php.Get(rec, "Name")), fqdn) || !Php.EqualsIgnoreCase(Php.ToStr(Php.Get(rec, "Type")), type))
                {
                    continue;
                }
                var content = Php.ToStr(Php.Get(rec, "Content"));
                if (content != "")
                {
                    set.Contents.Add(content);
                }
                set.Ttl = Php.ToInt(Php.Get(rec, "TTL"));
            }
            return set;
        }

        /// <summary>TXT values compare without surrounding quotes; host targets case- and trailing-dot-insensitively.</summary>
        private static bool SameRecordValue(string type, string a, string b)
        {
            string Normalize(string v)
            {
                v = Php.Trim(v ?? "");
                if (Php.Upper(type ?? "") == "TXT")
                {
                    if (v.Length > 1 && v[0] == '"' && v[v.Length - 1] == '"')
                    {
                        v = v.Substring(1, v.Length - 2);
                    }
                    return v;
                }
                return Php.Lower(Php.RTrim(v, "."));
            }
            return Normalize(a) == Normalize(b);
        }

        /// <summary>"www" → "www.example.com."; already-qualified names pass through.</summary>
        private static string QualifyRecordName(string name, string domainName)
        {
            name = Php.Trim(name ?? "");
            var domain = Php.RTrim(Php.Lower(Php.Trim(domainName ?? "")), ".");
            var apex = domain + ".";

            if (name == "" || name == "@")
            {
                return apex;
            }
            var bare = Php.RTrim(name, ".");
            if (Php.EqualsIgnoreCase(bare, domain) || Php.Lower(bare).EndsWith("." + domain, StringComparison.Ordinal))
            {
                return bare + ".";
            }
            return bare + "." + apex;
        }

        /// <summary>"@", the apex or an FQDN → the relative label the gateway expects ('' for the apex).</summary>
        private static string RelativizeRecordName(string name, string domainName)
        {
            name = Php.Trim(name ?? "");
            var domain = Php.RTrim(Php.Lower(Php.Trim(domainName ?? "")), ".");

            if (name == "" || name == "@")
            {
                return "";
            }
            var bare = Php.RTrim(name, ".");
            if (Php.EqualsIgnoreCase(bare, domain))
            {
                return "";
            }
            var suffix = "." + domain;
            if (bare.Length > suffix.Length && Php.EqualsIgnoreCase(bare.Substring(bare.Length - suffix.Length), suffix))
            {
                return bare.Substring(0, bare.Length - suffix.Length);
            }
            return bare;
        }

        /// <summary>Error message for a record type the gateway cannot store, or null.</summary>
        private static string UnsupportedZoneType(string type)
        {
            type = Php.Upper(Php.Trim(type ?? ""));
            if (type == "")
            {
                return "Record type is required.";
            }
            if (!ZoneRecordTypes.Contains(type))
            {
                return "Record type \"" + type + "\" is not supported by the DNS zone API. Supported types: " + string.Join(", ", ZoneRecordTypes) + ".";
            }
            return null;
        }

        private static JsonObject BuildZoneStruct(string name, string type, JsonNode content, long ttl)
        {
            if (ttl < 1) ttl = 3600;
            if (ttl > 86400) ttl = 86400;
            var contents = new JsonArray();
            foreach (var v in Php.Values(Php.ToArray(content)))
            {
                contents.Add(Php.ToStr(v));
            }
            return new JsonObject
            {
                ["name"] = name ?? "",
                ["ttl"] = Php.Int(ttl),
                ["type"] = Php.Upper(type ?? ""),
                ["contents"] = contents,
            };
        }

        // ------------------------------------------------------------------
        // URL forwarding
        // ------------------------------------------------------------------

        /// <summary>
        /// The domain's URL forwarding: <c>{Enabled, RedirectAddress, ForwardType}</c>.
        /// "Not configured" is an OK result with Enabled=false.
        /// </summary>
        public JsonNode GetForwarding(string domainName)
        {
            try
            {
                var response = Request("GET", "domains/forwards", new JsonObject { ["domainName"] = domainName });
                return Ok(new JsonObject
                {
                    ["Enabled"] = !Php.Empty(Php.Get(response, "redirectAlias")),
                    ["RedirectAddress"] = Php.ToStr(Php.Coalesce(Php.Get(response, "redirectAlias"), Php.Str(""))),
                    ["ForwardType"] = Php.ToStr(Php.Coalesce(Php.Get(response, "redirectType"), Php.Str(""))),
                });
            }
            catch (Exception e)
            {
                var raw = Php.Coalesce(Php.Get(_lastResponse, "raw_response"), Php.Str(e.Message));
                // Only the gateway's "no forwarding configured" answer means disabled.
                if (IsForwardNotSet(e))
                {
                    return Ok(new JsonObject { ["Enabled"] = false, ["RedirectAddress"] = "", ["ForwardType"] = "" });
                }

                var code = e is ApiException api ? FormatErrorCode(api.Code) : "GET_FORWARD";
                return new JsonObject
                {
                    ["result"] = ResultError,
                    ["error"] = SetError(code, Php.Str(e.Message), Php.Clone(Php.Coalesce(Php.Get(_lastParsedResponse, "Details"), raw))),
                };
            }
        }

        /// <summary>Point the domain at a URL. <paramref name="forwardType"/>: "Standard" (redirect) or "Frame" (masked).</summary>
        public JsonNode SetForwarding(string domainName, string redirectAddress, string forwardType = "Standard")
        {
            try
            {
                var type = Php.UcFirst(Php.Lower(Php.Trim(forwardType ?? "")));
                if (!ForwardTypes.Contains(type))
                {
                    var why = "Forward type \"" + forwardType + "\" is not supported. Supported types: " + string.Join(", ", ForwardTypes) + ".";
                    return new JsonObject { ["result"] = ResultError, ["error"] = SetError("SET_FORWARD", why, why) };
                }
                if (Php.Trim(redirectAddress ?? "") == "")
                {
                    const string why = "Redirect address is required.";
                    return new JsonObject { ["result"] = ResultError, ["error"] = SetError("SET_FORWARD", why, why) };
                }

                Request("POST", "domains/forwards", new JsonObject
                {
                    ["domainName"] = domainName,
                    ["redirectAddress"] = redirectAddress,
                    ["forwardType"] = type,
                });

                return Ok(new JsonObject { ["RedirectAddress"] = redirectAddress, ["ForwardType"] = type });
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "SET_FORWARD");
            }
        }

        /// <summary>Remove the domain's URL forwarding.</summary>
        public JsonNode DeleteForwarding(string domainName)
        {
            try
            {
                Request("DELETE", "domains/forwards", new JsonObject { ["DomainName"] = domainName });
                return Ok(new JsonObject { ["DomainName"] = domainName, ["Removed"] = true });
            }
            catch (Exception e)
            {
                // Nothing configured: deleting is a no-op.
                if (IsForwardNotSet(e))
                {
                    return Ok(new JsonObject { ["DomainName"] = domainName, ["Removed"] = false });
                }
                return ErrorEnvelope(e, "DELETE_FORWARD");
            }
        }

        /// <summary>The gateway answers "Redirect information not found" when no forwarding is configured.</summary>
        private bool IsForwardNotSet(Exception e)
        {
            const string needle = "redirect information not found";
            return e.Message.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0
                || Php.ToStr(Php.Get(_lastResponse, "raw_response")).IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0
                || Php.JsonEncode(_lastResponse).IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
