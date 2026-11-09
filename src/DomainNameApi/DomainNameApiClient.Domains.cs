using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using DomainNameApi.Internal;

namespace DomainNameApi
{
    public sealed partial class DomainNameApiClient
    {
        // ------------------------------------------------------------------
        // Account
        // ------------------------------------------------------------------

        /// <summary>Reseller account details with USD and TL balances.</summary>
        public JsonNode GetResellerDetails()
        {
            try
            {
                var response = Request("GET", "deposit/accounts/me");
                var resp = new JsonObject();
                if (Php.Get(response, "resellerId") != null)
                {
                    resp["result"] = ResultOk;
                    resp["id"] = Php.Clone(Php.Get(response, "resellerId"));
                    resp["active"] = true;
                    resp["name"] = Php.Clone(Php.Coalesce(Php.Get(response, "resellerName"), Php.Str("")));
                    resp["balance"] = Php.NumberFormat(Php.ToFloat(Php.Coalesce(Php.Get(response, "usdBalance"), Php.Int(0))), 2);
                    resp["currency"] = "USD";
                    resp["symbol"] = "$";
                    resp["balances"] = new JsonArray
                    {
                        new JsonObject
                        {
                            ["balance"] = Php.NumberFormat(Php.ToFloat(Php.Coalesce(Php.Get(response, "usdBalance"), Php.Int(0))), 2),
                            ["currency"] = "USD",
                            ["symbol"] = "$",
                        },
                        new JsonObject
                        {
                            ["balance"] = Php.NumberFormat(Php.ToFloat(Php.Coalesce(Php.Get(response, "tryBalance"), Php.Int(0))), 2),
                            ["currency"] = "TL",
                            ["symbol"] = "TL",
                        },
                    };
                }
                else
                {
                    resp["result"] = ResultError;
                    resp["error"] = SetError("CREDENTIALS", "Invalid response format", "Response does not contain required fields");
                }
                return resp;
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "RESELLER_DETAILS");
            }
        }

        /// <summary>Balance in one currency (USD, TRY, EUR, GBP), in the legacy OperationResult envelope.</summary>
        public JsonNode GetCurrentBalance(string currencyId = "USD")
        {
            currencyId = currencyId ?? "";
            var currencyName = Php.Upper(currencyId);
            long id;
            string name, symbol;
            switch (currencyName)
            {
                case "USD": id = 2; name = "USD"; symbol = "$"; break;
                case "TRY": id = 1; name = "TL"; symbol = "TL"; break;
                case "EUR": id = 3; name = "EUR"; symbol = "€"; break;
                case "GBP": id = 4; name = "GBP"; symbol = "£"; break;
                default: id = 0; name = currencyName; symbol = ""; break;
            }

            try
            {
                var response = Request("GET", "deposit/accounts/me", new JsonObject { ["currency"] = currencyName });
                var balanceKey = Php.Lower(currencyId) + "Balance";
                if (!Php.IsArray(response) || !HasKey(response, balanceKey))
                {
                    // The account only reports USD and TRY balances.
                    return new JsonObject
                    {
                        ["ErrorCode"] = "BALANCE",
                        ["OperationMessage"] = "No " + currencyName + " balance is reported for this account. Available: USD, TRY.",
                        ["OperationResult"] = "FAILED",
                        ["Balance"] = "0.00",
                        ["CurrencyId"] = Php.Int(id),
                        ["CurrencyInfo"] = null,
                        ["CurrencyName"] = name,
                        ["CurrencySymbol"] = symbol,
                    };
                }
                return new JsonObject
                {
                    ["ErrorCode"] = Php.Int(0),
                    ["OperationMessage"] = "Command completed succesfully.",
                    ["OperationResult"] = "SUCCESS",
                    ["Balance"] = Php.NumberFormat(Php.ToFloat(Php.Coalesce(Php.Get(response, balanceKey), Php.Int(0))), 2),
                    ["CurrencyId"] = Php.Int(id),
                    ["CurrencyInfo"] = null,
                    ["CurrencyName"] = name,
                    ["CurrencySymbol"] = symbol,
                };
            }
            catch (Exception e)
            {
                return new JsonObject
                {
                    ["ErrorCode"] = e is ApiException api ? FormatErrorCode(api.Code) : "BALANCE",
                    ["OperationMessage"] = e.Message,
                    ["OperationResult"] = "FAILED",
                    ["Balance"] = "0.00",
                    ["CurrencyId"] = Php.Int(id),
                    ["CurrencyInfo"] = null,
                    ["CurrencyName"] = name,
                    ["CurrencySymbol"] = symbol,
                };
            }
        }

        // ------------------------------------------------------------------
        // Search and lists
        // ------------------------------------------------------------------

        /// <summary>
        /// Availability of every domain × extension pair. Returns a bare list of
        /// results on success, or an error envelope.
        /// </summary>
        public JsonNode CheckAvailability(IEnumerable<string> domains, IEnumerable<string> extensions, int period, string command = "create")
        {
            try
            {
                var queries = new JsonArray();
                var extList = (extensions ?? Enumerable.Empty<string>()).ToList();
                foreach (var domain in domains ?? Enumerable.Empty<string>())
                {
                    foreach (var tld in extList)
                    {
                        queries.Add(new JsonObject { ["domainName"] = (domain ?? "") + "." + Php.LTrim(tld ?? "", ".") });
                    }
                }

                if (queries.Count == 0)
                {
                    return new JsonObject
                    {
                        ["result"] = ResultError,
                        ["error"] = SetError("AVAILABILITY", "Domain names not found.", "No domain names provided for availability check"),
                    };
                }

                var response = Request("POST", "domains/bulk-search", queries);

                var availability = new JsonArray();
                var items = Php.Coalesce(Php.Get(response, "infos"), response);
                if (Php.IsArray(items))
                {
                    foreach (var item in Php.Values(items))
                    {
                        if (!Php.IsArray(item)) continue;

                        var tldNode = Php.Get(item, "tld");
                        string tld;
                        if (tldNode != null)
                        {
                            tld = Php.Lower(Php.ToStr(tldNode));
                        }
                        else
                        {
                            var dn = Php.ToStr(Php.Get(item, "domainName"));
                            var dot = dn.LastIndexOf('.');
                            tld = Php.Lower(dot >= 0 ? dn.Substring(dot + 1) : "");
                        }
                        var domainRaw = Php.ToStr(Php.Coalesce(Php.Get(item, "domainName"), Php.Str("")));
                        // Strip only the trailing ".tld".
                        var lower = Php.Lower(domainRaw);
                        var suffix = "." + tld;
                        var domainName = tld != "" && lower.EndsWith(suffix, StringComparison.Ordinal)
                            ? lower.Substring(0, lower.Length - suffix.Length)
                            : lower;
                        var status = Php.Lower(Php.ToStr(Php.Coalesce(Php.Get(item, "status"), Php.Str(""))));
                        var isAvailable = status == "available" || status == "true"
                                          || (Php.IsNumericString(status) && Php.StringToFloat(status) == 1.0);

                        var price = Php.Get(item, "price");
                        availability.Add(new JsonObject
                        {
                            ["TLD"] = tld,
                            ["DomainName"] = domainName,
                            ["Status"] = isAvailable ? "available" : "notavailable",
                            ["Command"] = command,
                            ["Period"] = Php.Clone(Php.Coalesce(Php.Get(item, "period"), Php.Int(period))),
                            ["IsFee"] = Php.Clone(Php.Coalesce(Php.Get(item, "isPremium"), Php.Bool(false))),
                            ["Price"] = price != null ? Php.Str(Php.NumberFormat(Php.ToFloat(price), 4)) : null,
                            ["Currency"] = Php.Clone(Php.Get(item, "currency")),
                            ["Reason"] = Php.Clone(Php.Get(item, "reason")),
                        });
                    }
                }
                return availability;
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "AVAILABILITY");
            }
        }

        /// <summary>
        /// Domains in the account. Filters use the gateway names (MaxResultCount,
        /// SkipCount, Sorting, DomainName, ...) or the legacy PageNumber (0-based),
        /// PageSize, OrderColumn and OrderDirection.
        /// </summary>
        public JsonNode GetList(IDictionary<string, object> extraParameters = null)
        {
            try
            {
                var extra = (JsonObject)ToMap(Php.FromObject(extraParameters));

                if (Php.Get(extra, "PageSize") != null)
                {
                    extra["MaxResultCount"] = Php.Int(Php.ToInt(Php.Get(extra, "PageSize")));
                }
                if (Php.Get(extra, "PageNumber") != null)
                {
                    var pageSize = Php.ToInt(Php.Coalesce(Php.Get(extra, "PageSize"), Php.Int(200)));
                    extra["SkipCount"] = Php.Int(Php.ToInt(Php.Get(extra, "PageNumber")) * pageSize);
                }
                if (Php.Get(extra, "OrderColumn") != null)
                {
                    var direction = Php.Upper(Php.ToStr(Php.Coalesce(Php.Get(extra, "OrderDirection"), Php.Str("ASC")))) == "DESC" ? "DESC" : "ASC";
                    extra["Sorting"] = Php.ToStr(Php.Get(extra, "OrderColumn")) + " " + direction;
                }
                extra.Remove("PageNumber");
                extra.Remove("PageSize");
                extra.Remove("OrderColumn");
                extra.Remove("OrderDirection");

                // array_merge($defaults, $extra)
                var parameters = new JsonObject { ["MaxResultCount"] = Php.Int(200), ["SkipCount"] = Php.Int(0) };
                foreach (var kv in Php.Entries(extra)) parameters[kv.Key] = Php.Clone(kv.Value);

                var response = Request("GET", "domains", parameters);
                var itemsNode = Php.Get(response, "items");

                return new JsonObject
                {
                    ["data"] = new JsonObject
                    {
                        ["Domains"] = itemsNode != null ? Php.MapPreserve(itemsNode, ListItem) : new JsonArray(),
                    },
                    ["result"] = ResultOk,
                    ["TotalCount"] = Php.Int(Php.ToInt(Php.Coalesce(Php.Get(response, "totalCount"), Php.Int(0)))),
                };
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "DOMAIN_LIST");
            }
        }

        private JsonNode ListItem(JsonNode item)
        {
            var statusCode = Php.Get(item, "statusCode");
            var statusText = Php.Get(item, "statusText");
            string status;
            if (statusCode != null && !IsEmptyString(statusCode) && !Php.IsNumeric(statusCode))
            {
                status = Php.ToStr(statusCode);
            }
            else if (statusText != null && !IsEmptyString(statusText) && !Php.IsNumeric(statusText))
            {
                status = Php.ToStr(statusText);
            }
            else
            {
                status = NormalizeDomainStatus(Php.Coalesce(Php.Get(item, "status"), Php.Str("")));
            }

            return new JsonObject
            {
                ["ID"] = Php.Int(Php.ToInt(Php.Coalesce(Php.Get(item, "id"), Php.Int(0)))),
                ["Status"] = status,
                ["DomainName"] = Php.Clone(Php.Coalesce(Php.Get(item, "domainName"), Php.Str(""))),
                ["AuthCode"] = Php.Clone(Php.Coalesce(Php.Get(item, "authCode"), Php.Str(""))),
                ["LockStatus"] = !Php.Empty(Php.Get(item, "lockStatus")) ? "true" : "false",
                ["PrivacyProtectionStatus"] = !Php.Empty(Php.Get(item, "privacyProtectionStatus")) ? "true" : "false",
                ["IsChildNameServer"] = !Php.Empty(Php.Get(item, "hosts")) ? "true" : "false",
                ["Contacts"] = new JsonObject
                {
                    ["Billing"] = new JsonObject { ["ID"] = "" },
                    ["Technical"] = new JsonObject { ["ID"] = "" },
                    ["Administrative"] = new JsonObject { ["ID"] = "" },
                    ["Registrant"] = new JsonObject { ["ID"] = "" },
                },
                ["Dates"] = new JsonObject
                {
                    ["Start"] = FormatApiDate(Php.Get(item, "startDate")),
                    ["Expiration"] = FormatApiDate(Php.Get(item, "expirationDate")),
                    ["RemainingDays"] = Php.Int(Php.ToInt(Php.Coalesce(Php.Get(item, "remainingDay"), Php.Int(0)))),
                },
                ["NameServers"] = Php.Clone(Php.Coalesce(Php.Get(item, "nameServers"), new JsonArray())),
                ["Additional"] = new JsonArray(),
                ["ChildNameServers"] = new JsonArray(),
            };
        }

        private static bool IsEmptyString(JsonNode n) => Php.IsString(n) && n.GetValue<string>().Length == 0;

        /// <summary>TLD list with the pricing matrix (prices formatted with four decimals).</summary>
        public JsonNode GetTldList(int count = 20)
        {
            try
            {
                var response = Get("products/tlds", new JsonObject { ["MaxResultCount"] = Php.Int(count), ["SkipCount"] = Php.Int(0) });

                var tldData = new JsonArray();
                long idCounter = 1;
                var items = Php.Get(response, "items");
                if (items != null && Php.IsArray(items))
                {
                    var priceTypes = new[]
                    {
                        ("register", "registration"), ("renew", "renew"), ("transfer", "transfer"),
                        ("restore", "restore"), ("refund", "refund"), ("backorder", "backorder"),
                    };
                    foreach (var tld in Php.Values(items))
                    {
                        var pricing = new JsonObject();
                        var currencies = new JsonObject();
                        var firstPrice = Php.Get(Php.Get(tld, "prices"), 0);

                        if (firstPrice != null && Php.IsArray(firstPrice))
                        {
                            foreach (var (apiType, outType) in priceTypes)
                            {
                                var apiValue = Php.Get(firstPrice, apiType);
                                if (apiValue == null) continue;
                                if (Php.IsArray(apiValue) && Php.Get(apiValue, 0) != null)
                                {
                                    foreach (var priceInfo in Php.Values(apiValue))
                                    {
                                        if (Php.IsArray(priceInfo)) AddPrice(pricing, currencies, outType, priceInfo);
                                    }
                                }
                                else if (Php.IsArray(apiValue))
                                {
                                    AddPrice(pricing, currencies, outType, apiValue);
                                }
                            }
                        }

                        // ksort($pricing[$type], SORT_NUMERIC)
                        foreach (var type in pricing.Select(kv => kv.Key).ToList())
                        {
                            var periods = (JsonObject)pricing[type];
                            var sorted = new JsonObject();
                            foreach (var kv in periods.ToList().OrderBy(kv => long.Parse(kv.Key, CultureInfo.InvariantCulture)))
                            {
                                sorted[kv.Key] = Php.Clone(kv.Value);
                            }
                            pricing[type] = sorted;
                        }

                        var constraints = Php.Get(tld, "constraints");
                        tldData.Add(new JsonObject
                        {
                            ["id"] = Php.Int(idCounter++),
                            ["status"] = Php.Clone(Php.Coalesce(Php.Get(tld, "status"), Php.Str("Active"))),
                            ["maxchar"] = Php.Clone(Php.Coalesce(Php.Get(constraints, "maxLenght"), Php.Int(63))),
                            ["maxperiod"] = Php.Clone(Php.Coalesce(Php.Get(tld, "maxRegistrationPeriod"), Php.Int(10))),
                            ["minchar"] = Php.Clone(Php.Coalesce(Php.Get(constraints, "minLength"), Php.Int(1))),
                            ["minperiod"] = Php.Clone(Php.Coalesce(Php.Get(tld, "minRegistrationPeriod"), Php.Int(1))),
                            ["tld"] = Php.Clone(Php.Get(tld, "name")),
                            ["pricing"] = pricing,
                            ["currencies"] = currencies,
                        });
                    }
                }

                return new JsonObject { ["data"] = tldData, ["result"] = ResultOk };
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "TLD_LIST");
            }
        }

        private static void AddPrice(JsonObject pricing, JsonObject currencies, string outType, JsonNode priceInfo)
        {
            var period = Php.ToInt(Php.Coalesce(Php.Get(priceInfo, "period"), Php.Int(1)));
            if (period < 1) period = 1;
            var priceNode = Php.Get(priceInfo, "price");
            var price = priceNode != null ? Php.NumberFormat(Php.ToFloat(priceNode), 4) : "0.0000";
            if (!(pricing[outType] is JsonObject periods))
            {
                periods = new JsonObject();
                pricing[outType] = periods;
            }
            periods[period.ToString(CultureInfo.InvariantCulture)] = price;
            currencies[outType] = Php.Clone(Php.Coalesce(Php.Get(priceInfo, "currency"), Php.Str("")));
        }

        // ------------------------------------------------------------------
        // Domain
        // ------------------------------------------------------------------

        /// <summary>Domain details: status, dates, lock/privacy flags, nameservers, child nameservers.</summary>
        public JsonNode GetDetails(string domainName)
        {
            try
            {
                var response = Request("GET", "domains/info", new JsonObject { ["DomainName"] = domainName });
                return ParseDomainInfo(response);
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "DOMAIN_DETAILS");
            }
        }

        /// <summary>Same as <see cref="GetDetails"/> (the gateway always answers from the registry).</summary>
        public JsonNode SyncFromRegistry(string domainName) => GetDetails(domainName);

        /// <summary>Replace the domain's nameservers. Accepts a list or a map (the values are used).</summary>
        public JsonNode ModifyNameServer(string domainName, object nameServers)
        {
            var input = Php.FromObject(nameServers);
            try
            {
                var payload = new JsonObject { ["domainName"] = domainName, ["nameServers"] = NormalizeHostNameList(input) };
                var response = Request("PUT", "domains/dns/name-server", payload);
                return new JsonObject
                {
                    ["result"] = ResultOk,
                    ["data"] = new JsonObject
                    {
                        // Report the normalised list that was sent, not the raw input.
                        ["NameServers"] = Php.Clone(Php.Coalesce(Php.Get(response, "nameServers"), payload["nameServers"])),
                    },
                };
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "MODIFY_NS");
            }
        }

        /// <summary>Turn the registrar (theft protection) lock on.</summary>
        public JsonNode EnableTheftProtectionLock(string domainName)
        {
            try
            {
                Request("POST", "domains/lock", new JsonObject { ["domainName"] = domainName });
                return Ok(new JsonObject { ["LockStatus"] = true });
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "ENABLE_LOCK");
            }
        }

        /// <summary>Turn the registrar (theft protection) lock off.</summary>
        public JsonNode DisableTheftProtectionLock(string domainName)
        {
            try
            {
                Request("POST", "domains/unlock", new JsonObject { ["domainName"] = domainName });
                return Ok(new JsonObject { ["LockStatus"] = false });
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "DISABLE_LOCK");
            }
        }

        /// <summary>Create a child nameserver (glue record) under the domain.</summary>
        public JsonNode AddChildNameServer(string domainName, string nameServer, string ipAddress)
        {
            try
            {
                var payload = new JsonObject
                {
                    ["domainName"] = domainName,
                    ["hostName"] = NormalizeHostName(nameServer),
                    ["ipAddresses"] = new JsonArray
                    {
                        new JsonObject { ["ipAddress"] = ipAddress, ["ipVersion"] = IsIpv4(ipAddress) ? "v4" : "v6" },
                    },
                };
                Request("POST", "domains/dns/host", payload);
                return Ok(new JsonObject
                {
                    ["NameServer"] = nameServer,
                    ["IPAdresses"] = new JsonArray(Php.Str(ipAddress)),
                });
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "ADD_CHILD_NS");
            }
        }

        /// <summary>Delete a child nameserver.</summary>
        public JsonNode DeleteChildNameServer(string domainName, string nameServer)
        {
            try
            {
                var payload = new JsonObject { ["domainName"] = domainName, ["hostName"] = NormalizeHostName(nameServer) };
                Request("DELETE", "domains/dns/host", payload);
                return Ok(new JsonObject { ["NameServer"] = nameServer });
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "DELETE_CHILD_NS");
            }
        }

        /// <summary>Change the IP address of a child nameserver.</summary>
        public JsonNode ModifyChildNameServer(string domainName, string nameServer, string ipAddress)
        {
            try
            {
                var host = NormalizeHostName(nameServer);
                var payload = new JsonObject
                {
                    ["domainName"] = domainName,
                    ["hostName"] = host,
                    ["newHostName"] = host,
                    ["ipAddresses"] = new JsonArray
                    {
                        new JsonObject { ["ipAddress"] = ipAddress, ["ipVersion"] = IsIpv4(ipAddress) ? "v4" : "v6" },
                    },
                };
                var response = Request("PUT", "domains/dns/host", payload);
                return Ok(new JsonObject
                {
                    ["NameServer"] = Php.Clone(Php.Coalesce(Php.Get(response, "hostName"), Php.Str(host))),
                    ["IPAdresses"] = Php.Clone(Php.Coalesce(Php.Get(response, "ipAddresses"), new JsonArray(Php.Str(ipAddress)))),
                });
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "MODIFY_CHILD_NS");
            }
        }

        // ------------------------------------------------------------------
        // Transfers
        // ------------------------------------------------------------------

        /// <summary>
        /// Start an incoming transfer with the domain's EPP/auth code.
        /// </summary>
        /// <remarks>
        /// Contacts are never sent with a transfer (the request always carries an
        /// empty contact list): the registrar takes them over from the losing registrar.
        /// </remarks>
        public JsonNode Transfer(string domainName, string eppCode, int period)
        {
            try
            {
                var payload = new JsonObject
                {
                    ["domainName"] = domainName,
                    ["authCode"] = eppCode,
                    ["period"] = Php.Int(period),
                    ["contacts"] = new JsonArray(),
                };
                var response = Request("POST", "domains/transfer", payload);
                var failed = OperationFailure(response, "TRANSFER_DOMAIN");
                if (failed != null) return failed;
                return ParseDomainInfo(response);
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "TRANSFER_DOMAIN");
            }
        }

        /// <summary>Cancel a pending incoming transfer.</summary>
        public JsonNode CancelTransfer(string domainName) => TransferAction(domainName, "cancel", "Cancelled", "CANCEL_TRANSFER");

        /// <summary>Approve a pending outgoing transfer.</summary>
        public JsonNode ApproveTransfer(string domainName) => TransferAction(domainName, "approve", "Approved", "APPROVE_TRANSFER");

        /// <summary>Reject a pending outgoing transfer.</summary>
        public JsonNode RejectTransfer(string domainName) => TransferAction(domainName, "reject", "Rejected", "REJECT_TRANSFER");

        private JsonNode TransferAction(string domainName, string action, string defaultStatus, string errorCode)
        {
            try
            {
                var response = Request("POST", "domains/transfers/" + action, new JsonObject { ["domainName"] = domainName });
                return Ok(new JsonObject
                {
                    ["DomainName"] = domainName,
                    ["Status"] = Php.Clone(Php.Coalesce(Php.Get(response, "status"), Php.Str(defaultStatus))),
                });
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, errorCode);
            }
        }

        /// <summary>
        /// Can the domain be transferred with this auth code? <c>result</c> is OK only
        /// when the registry reports it transferable; the details are under <c>data</c>.
        /// </summary>
        public JsonNode CheckTransfer(string domainName, string authCode)
        {
            try
            {
                var response = Request("POST", "domains/transfers/check", new JsonObject { ["domainName"] = domainName, ["authCode"] = authCode });
                var available = !Php.Empty(Php.Get(response, "transferAvailabilityStatus"));
                var additional = Php.Get(response, "additionalAttributes");
                var envelope = new JsonObject
                {
                    ["result"] = available ? ResultOk : ResultError,
                    ["data"] = new JsonObject
                    {
                        ["TransferAvailabilityStatus"] = available,
                        ["AuthCodeIsRequired"] = !Php.Empty(Php.Get(response, "authCodeIsRequired")),
                        ["AuthCodeIsValid"] = !Php.Empty(Php.Get(response, "authCodeIsValid")),
                        ["UserTransferRequired"] = !Php.Empty(Php.Get(response, "userTransferRequired")),
                        ["TransferLock"] = !Php.Empty(Php.Get(response, "transferLock")),
                        ["Message"] = Php.ToStr(Php.Coalesce(Php.Get(response, "message"), Php.Str(""))),
                        ["MessageKey"] = Php.ToStr(Php.Coalesce(Php.Get(response, "messageKey"), Php.Str(""))),
                        ["Additional"] = additional != null ? Php.ToArray(additional) : new JsonArray(),
                    },
                };
                if (!available)
                {
                    envelope["error"] = SetError(
                        "TRANSFER_NOT_AVAILABLE",
                        Php.Coalesce(Php.Get(response, "message"), Php.Str("Domain is not transferable")),
                        Php.Coalesce(Php.Get(response, "messageKey"), Php.Get(response, "message"), Php.Str("")));
                }
                return envelope;
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "CHECK_TRANSFER");
            }
        }

        // ------------------------------------------------------------------
        // Renew / register / privacy
        // ------------------------------------------------------------------

        /// <summary>Renew the domain for <paramref name="period"/> years.</summary>
        public JsonNode Renew(string domainName, int period)
        {
            try
            {
                var response = Request("POST", "domains/renew", new JsonObject { ["domainName"] = domainName, ["period"] = Php.Int(period) });
                var failed = OperationFailure(response, "RENEW_DOMAIN");
                if (failed != null) return failed;
                if (!Php.Empty(Php.Get(response, "expirationDate")))
                {
                    return Ok(new JsonObject { ["ExpirationDate"] = FormatApiDate(Php.Get(response, "expirationDate")) });
                }
                SendErrorToSentryAsync(new ApiException("[DOMAIN_RENEW] " + DefaultErrors["DOMAIN_RENEW"].Description, 0));
                return new JsonObject { ["result"] = ResultError, ["error"] = SetError("DOMAIN_RENEW") };
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "RENEW_DOMAIN");
            }
        }

        /// <summary>
        /// Register a domain with full contact information.
        /// </summary>
        /// <param name="domainName">Domain to register.</param>
        /// <param name="period">Years.</param>
        /// <param name="contacts">Map of Administrative/Billing/Technical/Registrant to a contact map (flat or nested shape).</param>
        /// <param name="nameServers">Nameservers (list or map); defaults to ns1/ns2.domainnameapi.com when empty.</param>
        /// <param name="eppLock">Registrar lock.</param>
        /// <param name="privacyLock">WHOIS privacy.</param>
        /// <param name="additionalAttributes">TLD attributes (TRABIS fields for .tr).</param>
        public JsonNode RegisterWithContactInfo(
            string domainName,
            int period,
            IDictionary<string, object> contacts,
            object nameServers = null,
            bool eppLock = true,
            bool privacyLock = false,
            IDictionary<string, object> additionalAttributes = null)
        {
            try
            {
                var payloadContacts = BuildPayloadContacts(Php.FromObject(contacts));

                var ns = NormalizeHostNameList(Php.FromObject(nameServers));
                if (ns.Count == 0)
                {
                    ns = new JsonArray(DefaultNameServers.Select(Php.Str).ToArray());
                }

                var attrs = Php.ToArray(Php.FromObject(additionalAttributes));
                if (Php.Last(Php.Lower(domainName ?? ""), 3) == ".tr")
                {
                    attrs = NormalizeTrAttributes(attrs);
                }
                var tldAttributes = StringifyAttributes(attrs);

                var payload = new JsonObject
                {
                    ["domainName"] = domainName,
                    ["period"] = Php.Int(period),
                    ["nameServers"] = ns,
                    ["isLocked"] = eppLock,
                    ["privacyEnabled"] = privacyLock,
                    ["contacts"] = payloadContacts,
                    ["tldAttributes"] = tldAttributes,
                };

                var response = Request("POST", "domains/register-with-contacts", payload);
                var failed = OperationFailure(response, "REGISTER_DOMAIN");
                if (failed != null) return failed;
                return ParseDomainInfo(response);
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "REGISTER_DOMAIN");
            }
        }

        /// <summary>Turn WHOIS privacy protection on or off.</summary>
        /// <param name="domainName">Domain.</param>
        /// <param name="status">Enable or disable privacy.</param>
        /// <param name="reason">Kept for signature compatibility only; the gateway has no reason field and it is not sent.</param>
        public JsonNode ModifyPrivacyProtectionStatus(string domainName, bool status, string reason = DefaultReason)
            => ModifyPrivacy(domainName, status);

        /// <summary>
        /// Turn WHOIS privacy protection on or off with the flag given as text, as
        /// panels store it: "1", "true", "on" and "yes" (any case) enable it.
        /// </summary>
        public JsonNode ModifyPrivacyProtectionStatus(string domainName, string status, string reason = DefaultReason)
            => ModifyPrivacy(domainName, ParseBool(status));

        private JsonNode ModifyPrivacy(string domainName, bool enabled)
        {
            try
            {
                Request("POST", "domains/privacy", new JsonObject { ["domainName"] = domainName, ["privacyStatus"] = enabled });
                return new JsonObject
                {
                    ["data"] = new JsonObject { ["PrivacyProtectionStatus"] = enabled },
                    ["result"] = ResultOk,
                };
            }
            catch (Exception e)
            {
                return ErrorEnvelope(e, "MODIFY_PRIVACY");
            }
        }

        /// <summary>
        /// register/transfer/renew can answer HTTP 200 with {"success": false}: that is a
        /// refusal, returned as an error envelope. Null when the operation succeeded.
        /// </summary>
        private JsonObject OperationFailure(JsonNode response, string code)
        {
            if (!(response is JsonObject o) || !o.TryGetPropertyValue("success", out var success)
                || !Php.IsBool(success) || success.GetValue<bool>())
            {
                return null;
            }
            var message = Php.Trim(Php.ToStr(Php.Coalesce(Php.Get(response, "message"), Php.Str(""))));
            if (message == "")
            {
                message = "The registry did not accept the request.";
            }
            return new JsonObject { ["result"] = ResultError, ["error"] = SetError(code, message, message) };
        }

        private static bool HasKey(JsonNode n, string key)
        {
            if (n is JsonObject o) return o.ContainsKey(key);
            if (n is JsonArray a) return int.TryParse(key, NumberStyles.None, CultureInfo.InvariantCulture, out var i) && i < a.Count && i.ToString(CultureInfo.InvariantCulture) == key;
            return false;
        }

        // ------------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------------

        /// <summary>Coerce tldAttributes values to strings (the gateway types them as a string dictionary).</summary>
        private static JsonObject StringifyAttributes(JsonNode attrs)
        {
            var output = new JsonObject();
            foreach (var kv in Php.Entries(attrs))
            {
                var v = kv.Value;
                if (Php.IsBool(v)) output[kv.Key] = v.GetValue<bool>() ? "true" : "false";
                else if (v == null) output[kv.Key] = "";
                else if (Php.IsScalar(v)) output[kv.Key] = Php.ToStr(v);
                else output[kv.Key] = Php.JsonEncode(v);
            }
            return output;
        }

        private static readonly Dictionary<string, string> TrKeyMap = new Dictionary<string, string>
        {
            ["TRABISDOMAINCATEGORY"] = "TRABISDOMAINCATEGORY",
            ["TRABISCITIZENID"] = "TRABISCITIZENID",
            ["TRABISCITIZIENID"] = "TRABISCITIZENID",
            ["TRABISTAXOFFICE"] = "TRABISTAXOFFICE",
            ["TRABISTAXNUMBER"] = "TRABISTAXNUMBER",
        };

        /// <summary>Reduce legacy .tr TRABIS attributes to the REST schema (BUG-9628).</summary>
        private static JsonNode NormalizeTrAttributes(JsonNode attrs)
        {
            if (Php.Empty(attrs))
            {
                return attrs;
            }
            var normalized = new JsonObject();
            foreach (var kv in Php.Entries(attrs))
            {
                if (!TrKeyMap.TryGetValue(kv.Key, out var target)) continue;
                if (kv.Key == "TRABISCITIZIENID" && normalized.ContainsKey("TRABISCITIZENID")) continue;
                normalized[target] = Php.Clone(kv.Value);
            }

            var cat = Php.Get(normalized, "TRABISDOMAINCATEGORY");
            if (cat != null)
            {
                var c = Php.Lower(Php.Trim(Php.ToStr(cat)));
                if (c == "1" || c == "owner" || c == "personal" || c == "individual") normalized["TRABISDOMAINCATEGORY"] = "Owner";
                else if (c == "0" || c == "corporate" || c == "company") normalized["TRABISDOMAINCATEGORY"] = "Corporate";
            }
            return normalized;
        }

        /// <summary>Numeric DomainStatus enum → label; labels pass through.</summary>
        private static string NormalizeDomainStatus(JsonNode value)
        {
            if (Php.IsNumeric(value))
            {
                return DomainStatusMap.TryGetValue(Php.ToInt(value), out var label) ? label : Php.ToStr(value);
            }
            return Php.ToStr(value);
        }

        private static JsonObject ParseContactIds(JsonNode contacts)
        {
            var result = new JsonObject
            {
                ["Billing"] = new JsonObject { ["ID"] = Php.Int(0) },
                ["Technical"] = new JsonObject { ["ID"] = Php.Int(0) },
                ["Administrative"] = new JsonObject { ["ID"] = Php.Int(0) },
                ["Registrant"] = new JsonObject { ["ID"] = Php.Int(0) },
            };
            if (!Php.IsArray(contacts)) return result;

            var typeMap = new Dictionary<string, string>
            {
                ["Billing"] = "Billing",
                ["Tech"] = "Technical",
                ["Admin"] = "Administrative",
                ["Registrant"] = "Registrant",
            };
            foreach (var c in Php.Values(contacts))
            {
                var apiType = Php.ToStr(Php.Coalesce(Php.Get(c, "ContactType"), Php.Get(c, "contactType"), Php.Str("")));
                if (typeMap.TryGetValue(apiType, out var key))
                {
                    ((JsonObject)result[key])["ID"] = Php.Clone(Php.Coalesce(Php.Get(c, "handle"), Php.Get(c, "id"), Php.Int(0)));
                }
            }
            return result;
        }

        /// <summary>php-dna parseDomainInfo(): the legacy domain envelope.</summary>
        private JsonObject ParseDomainInfo(JsonNode data)
        {
            if (Php.Empty(data))
            {
                return new JsonObject { ["result"] = ResultError, ["error"] = SetError("RESPONSE") };
            }

            var nameservers = Php.Get(data, "nameservers");
            var additional = Php.Get(data, "additionalAttributes");
            var hosts = Php.Get(data, "hosts");
            var remainingDay = Php.Get(data, "remainingDay");

            return new JsonObject
            {
                ["data"] = new JsonObject
                {
                    ["ID"] = Php.Int(Php.ToInt(Php.Coalesce(Php.Get(data, "id"), Php.Int(0)))),
                    ["Status"] = NormalizeDomainStatus(Php.Coalesce(Php.Get(data, "status"), Php.Str(""))),
                    ["DomainName"] = Php.ToStr(Php.Coalesce(Php.Get(data, "domainName"), Php.Get(data, "name"), Php.Str(""))),
                    ["AuthCode"] = Php.ToStr(Php.Coalesce(Php.Get(data, "authCode"), Php.Get(data, "eppCode"), Php.Str(""))),
                    ["LockStatus"] = !Php.Empty(Php.Get(data, "lockStatus")) ? "true" : "false",
                    ["PrivacyProtectionStatus"] = !Php.Empty(Php.Get(data, "privacyProtectionStatus")) ? "true" : "false",
                    ["IsChildNameServer"] = !Php.Empty(hosts) ? "true" : "false",
                    ["Contacts"] = ParseContactIds(Php.Coalesce(Php.Get(data, "contacts"), new JsonArray())),
                    ["Dates"] = new JsonObject
                    {
                        ["Start"] = FormatApiDate(Php.Get(data, "startDate")),
                        ["Expiration"] = FormatApiDate(Php.Get(data, "expirationDate")),
                        ["RemainingDays"] = Php.Int(remainingDay != null
                            ? Php.ToInt(remainingDay)
                            : RemainingDaysFromDate(Php.Get(data, "expirationDate"))),
                    },
                    ["NameServers"] = nameservers != null ? Php.MapPreserve(nameservers, ns => Php.Str(Php.ToStr(ns))) : new JsonArray(),
                    ["Additional"] = additional != null ? Php.ToArray(additional) : new JsonArray(),
                    ["ChildNameServers"] = hosts != null
                        ? Php.MapPreserve(hosts, ns =>
                        {
                            var ips = Php.Values(Php.Coalesce(Php.Get(ns, "ipAddresses"), new JsonArray()))
                                .Select(ip => Php.Get(ip, "ipAddress"))
                                .ToList();
                            return new JsonObject
                            {
                                ["ns"] = Php.Clone(Php.Get(ns, "name")),
                                ["ip"] = ips.Count > 0 ? Php.Clone(ips[ips.Count - 1]) : Php.Str(""),
                            };
                        })
                        : new JsonArray(),
                },
                ["result"] = ResultOk,
            };
        }

        /// <summary>Lower-case, trim and drop the trailing dot of a host name (BUG-10124).</summary>
        private static string NormalizeHostName(string hostName) => Php.Lower(Php.RTrim(Php.Trim(hostName ?? ""), "."));

        /// <summary>Normalize a nameserver list: drop non-strings, empties and duplicates; keep order.</summary>
        private static JsonArray NormalizeHostNameList(JsonNode hostNames)
        {
            var normalized = new JsonArray();
            if (!Php.IsArray(hostNames)) return normalized;
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var h in Php.Values(hostNames))
            {
                if (!Php.IsString(h)) continue;
                var name = NormalizeHostName(h.GetValue<string>());
                if (name != "" && seen.Add(name)) normalized.Add(name);
            }
            return normalized;
        }

        /// <summary>filter_var($ip, FILTER_VALIDATE_IP, FILTER_FLAG_IPV4).</summary>
        internal static bool IsIpv4(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return false;
            var parts = ip.Split('.');
            if (parts.Length != 4) return false;
            foreach (var p in parts)
            {
                if (p.Length == 0 || p.Length > 3) return false;
                if (p.Any(ch => ch < '0' || ch > '9')) return false;
                if (p.Length > 1 && p[0] == '0') return false;
                if (int.Parse(p, CultureInfo.InvariantCulture) > 255) return false;
            }
            return true;
        }

        /// <summary>A JSON array (PHP list) becomes an object keyed by index; objects pass through.</summary>
        private static JsonNode ToMap(JsonNode n)
        {
            if (n is JsonObject) return n;
            var o = new JsonObject();
            foreach (var kv in Php.Entries(n)) o[kv.Key] = Php.Clone(kv.Value);
            return o;
        }
    }
}
