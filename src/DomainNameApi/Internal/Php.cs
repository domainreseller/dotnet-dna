using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DomainNameApi.Internal
{
    /// <summary>
    /// PHP value semantics over <see cref="JsonNode"/>. php-dna works on PHP arrays
    /// (ordered maps that double as lists) and leans on PHP casts, isset/empty and
    /// string functions; this class reproduces exactly the behaviour the port needs
    /// so the C# client returns byte-for-byte the same structures.
    ///
    /// Conventions: a C# <c>null</c> node is PHP <c>null</c> (missing key and JSON
    /// null are the same thing for <c>isset</c>/<c>??</c>). Integers are always held
    /// as <see cref="long"/>, floats as <see cref="double"/>.
    /// </summary>
    internal static class Php
    {
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        // ------------------------------------------------------------------
        // Construction
        // ------------------------------------------------------------------

        public static JsonNode Str(string s) => s == null ? null : JsonValue.Create(s);
        public static JsonNode Int(long v) => JsonValue.Create(v);
        public static JsonNode Float(double v) => JsonValue.Create(v);
        public static JsonNode Bool(bool v) => JsonValue.Create(v);

        /// <summary>Deep copy (nodes can only have one parent).</summary>
        public static JsonNode Clone(JsonNode n) => n?.DeepClone();

        /// <summary>
        /// Convert a CLR value supplied by the caller (dictionaries, lists, strings,
        /// numbers, booleans, JsonNode/JsonElement) into a JsonNode tree.
        /// </summary>
        public static JsonNode FromObject(object value)
        {
            switch (value)
            {
                case null: return null;
                case JsonNode node: return node.DeepClone();
                case JsonElement el: return FromElement(el);
                case string s: return JsonValue.Create(s);
                case char c: return JsonValue.Create(c.ToString());
                case bool b: return JsonValue.Create(b);
                case byte or sbyte or short or ushort or int or uint or long:
                    return JsonValue.Create(Convert.ToInt64(value, Inv));
                case ulong ul:
                    return ul <= long.MaxValue ? JsonValue.Create((long)ul) : JsonValue.Create((double)ul);
                case float or double or decimal:
                    return JsonValue.Create(Convert.ToDouble(value, Inv));
                case Enum e: return JsonValue.Create(e.ToString());
                case IDictionary<string, object> gd:
                {
                    var o = new JsonObject();
                    foreach (var kv in gd) o[kv.Key] = FromObject(kv.Value);
                    return o;
                }
                case IDictionary<string, string> sd:
                {
                    var o = new JsonObject();
                    foreach (var kv in sd) o[kv.Key] = FromObject(kv.Value);
                    return o;
                }
                case IDictionary d:
                {
                    var o = new JsonObject();
                    foreach (DictionaryEntry kv in d) o[Convert.ToString(kv.Key, Inv) ?? ""] = FromObject(kv.Value);
                    return o;
                }
                case IEnumerable<KeyValuePair<string, object>> kvs:
                {
                    var o = new JsonObject();
                    foreach (var kv in kvs) o[kv.Key] = FromObject(kv.Value);
                    return o;
                }
                case IEnumerable seq:
                {
                    var a = new JsonArray();
                    foreach (var item in seq) a.Add(FromObject(item));
                    return a;
                }
                default:
                    return JsonValue.Create(Convert.ToString(value, Inv));
            }
        }

        private static JsonNode FromElement(JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.Undefined || el.ValueKind == JsonValueKind.Null) return null;
            return Decode(el.GetRawText());
        }

        /// <summary>
        /// PHP json_decode($s, true): invalid or empty input is null. Numbers keep
        /// the int/float split of the JSON text.
        /// </summary>
        public static JsonNode Decode(string json)
        {
            if (json == null) return null;
            try
            {
                using (var doc = JsonDocument.Parse(json, new JsonDocumentOptions { MaxDepth = 512 }))
                {
                    return FromJson(doc.RootElement);
                }
            }
            catch (JsonException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private static JsonNode FromJson(JsonElement el)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Object:
                {
                    var o = new JsonObject();
                    foreach (var p in el.EnumerateObject()) o[p.Name] = FromJson(p.Value); // last duplicate wins, as in PHP
                    return o;
                }
                case JsonValueKind.Array:
                {
                    var a = new JsonArray();
                    foreach (var item in el.EnumerateArray()) a.Add(FromJson(item));
                    return a;
                }
                case JsonValueKind.String: return JsonValue.Create(el.GetString());
                case JsonValueKind.True: return JsonValue.Create(true);
                case JsonValueKind.False: return JsonValue.Create(false);
                case JsonValueKind.Number:
                {
                    var raw = el.GetRawText();
                    if (raw.IndexOfAny(new[] { '.', 'e', 'E' }) < 0 && el.TryGetInt64(out var l)) return JsonValue.Create(l);
                    return JsonValue.Create(el.GetDouble());
                }
                default: return null;
            }
        }

        internal static bool TryLong(JsonValue v, out long value)
        {
            if (v.TryGetValue<long>(out value)) return true;
            if (v.TryGetValue<int>(out var i)) { value = i; return true; }
            if (v.TryGetValue<short>(out var sh)) { value = sh; return true; }
            if (v.TryGetValue<byte>(out var by)) { value = by; return true; }
            if (v.TryGetValue<uint>(out var ui)) { value = ui; return true; }
            if (v.TryGetValue<System.Text.Json.JsonElement>(out var el) && el.ValueKind == JsonValueKind.Number
                && el.GetRawText().IndexOfAny(new[] { '.', 'e', 'E' }) < 0 && el.TryGetInt64(out value)) return true;
            value = 0;
            return false;
        }

        internal static bool TryDouble(JsonValue v, out double value)
        {
            if (v.TryGetValue<double>(out value)) return true;
            if (v.TryGetValue<float>(out var f)) { value = f; return true; }
            if (v.TryGetValue<decimal>(out var m)) { value = (double)m; return true; }
            if (TryLong(v, out var l)) { value = l; return true; }
            if (v.TryGetValue<System.Text.Json.JsonElement>(out var el) && el.ValueKind == JsonValueKind.Number) { value = el.GetDouble(); return true; }
            value = 0;
            return false;
        }

        // ------------------------------------------------------------------
        // Type checks
        // ------------------------------------------------------------------

        public static bool IsArray(JsonNode n) => n is JsonObject || n is JsonArray;

        public static bool IsString(JsonNode n) => n is JsonValue v && v.TryGetValue<string>(out _);

        public static bool IsBool(JsonNode n) => n is JsonValue v && v.TryGetValue<bool>(out _);

        public static bool IsInt(JsonNode n) => n is JsonValue v && TryLong(v, out var _l);

        public static bool IsFloat(JsonNode n) => n is JsonValue v && !TryLong(v, out var _l) && TryDouble(v, out var _d);

        public static bool IsNumber(JsonNode n) => IsInt(n) || IsFloat(n);

        public static bool IsScalar(JsonNode n) => n is JsonValue;

        // ------------------------------------------------------------------
        // Array access
        // ------------------------------------------------------------------

        /// <summary>$arr[$key] ?? null — works for maps and lists; non-arrays yield null.</summary>
        public static JsonNode Get(JsonNode n, string key)
        {
            if (n is JsonObject o)
            {
                return o.TryGetPropertyValue(key, out var v) ? v : null;
            }
            if (n is JsonArray a && TryIndex(key, out var i) && i < a.Count)
            {
                return a[i];
            }
            return null;
        }

        public static JsonNode Get(JsonNode n, int index)
        {
            if (n is JsonArray a) return index >= 0 && index < a.Count ? a[index] : null;
            if (n is JsonObject o) return o.TryGetPropertyValue(index.ToString(Inv), out var v) ? v : null;
            return null;
        }

        /// <summary>Nested lookup: Get(n, "a", "b") is $n['a']['b'] ?? null.</summary>
        public static JsonNode Get(JsonNode n, params string[] path)
        {
            var cur = n;
            foreach (var k in path)
            {
                cur = Get(cur, k);
                if (cur == null) return null;
            }
            return cur;
        }

        /// <summary>$a ?? $b — returns the first non-null.</summary>
        public static JsonNode Coalesce(params JsonNode[] values)
        {
            foreach (var v in values) if (v != null) return v;
            return null;
        }

        private static bool TryIndex(string key, out int index)
        {
            index = -1;
            if (string.IsNullOrEmpty(key)) return false;
            if (key.Length > 1 && key[0] == '0') return false;
            foreach (var ch in key) if (ch < '0' || ch > '9') return false;
            return int.TryParse(key, NumberStyles.None, Inv, out index);
        }

        /// <summary>foreach ($arr as $k => $v). Non-arrays iterate nothing.</summary>
        public static IEnumerable<KeyValuePair<string, JsonNode>> Entries(JsonNode n)
        {
            if (n is JsonObject o)
            {
                foreach (var kv in o.ToList()) yield return kv;
            }
            else if (n is JsonArray a)
            {
                var items = a.ToList();
                for (var i = 0; i < items.Count; i++) yield return new KeyValuePair<string, JsonNode>(i.ToString(Inv), items[i]);
            }
        }

        /// <summary>foreach ($arr as $v).</summary>
        public static IEnumerable<JsonNode> Values(JsonNode n) => Entries(n).Select(kv => kv.Value);

        public static int Count(JsonNode n) => n is JsonObject o ? o.Count : n is JsonArray a ? a.Count : 0;

        /// <summary>array_map($fn, $arr) with one array: keys are preserved.</summary>
        public static JsonNode MapPreserve(JsonNode n, Func<JsonNode, JsonNode> fn)
        {
            if (n is JsonArray a)
            {
                var r = new JsonArray();
                foreach (var v in a.ToList()) r.Add(fn(v));
                return r;
            }
            if (n is JsonObject o)
            {
                var r = new JsonObject();
                foreach (var kv in o.ToList()) r[kv.Key] = fn(kv.Value);
                return r;
            }
            return new JsonArray();
        }

        /// <summary>(array) $value.</summary>
        public static JsonNode ToArray(JsonNode n)
        {
            if (n == null) return new JsonArray();
            if (IsArray(n)) return n.DeepClone();
            return new JsonArray(n.DeepClone());
        }

        // ------------------------------------------------------------------
        // Truthiness and casts
        // ------------------------------------------------------------------

        /// <summary>PHP empty().</summary>
        public static bool Empty(JsonNode n)
        {
            switch (n)
            {
                case null: return true;
                case JsonObject o: return o.Count == 0;
                case JsonArray a: return a.Count == 0;
                case JsonValue v:
                    if (v.TryGetValue<bool>(out var b)) return !b;
                    if (TryLong(v, out var l)) return l == 0;
                    if (TryDouble(v, out var d)) return d == 0.0;
                    if (v.TryGetValue<string>(out var s)) return s.Length == 0 || s == "0";
                    return false;
                default: return false;
            }
        }

        /// <summary>(bool) $value.</summary>
        public static bool ToBool(JsonNode n) => !Empty(n);

        /// <summary>(string) $value.</summary>
        public static string ToStr(JsonNode n)
        {
            switch (n)
            {
                case null: return "";
                case JsonObject _:
                case JsonArray _: return "Array";
                case JsonValue v:
                    if (v.TryGetValue<string>(out var s)) return s;
                    if (v.TryGetValue<bool>(out var b)) return b ? "1" : "";
                    if (TryLong(v, out var l)) return l.ToString(Inv);
                    if (TryDouble(v, out var d)) return FloatToString(d);
                    return v.ToJsonString();
                default: return "";
            }
        }

        /// <summary>(int) $value.</summary>
        public static long ToInt(JsonNode n)
        {
            switch (n)
            {
                case null: return 0;
                case JsonObject o: return o.Count > 0 ? 1 : 0;
                case JsonArray a: return a.Count > 0 ? 1 : 0;
                case JsonValue v:
                    if (TryLong(v, out var l)) return l;
                    if (v.TryGetValue<bool>(out var b)) return b ? 1 : 0;
                    if (TryDouble(v, out var d)) return DoubleToInt(d);
                    if (v.TryGetValue<string>(out var s)) return StringToInt(s);
                    return 0;
                default: return 0;
            }
        }

        /// <summary>(float) $value.</summary>
        public static double ToFloat(JsonNode n)
        {
            switch (n)
            {
                case null: return 0;
                case JsonObject o: return o.Count > 0 ? 1 : 0;
                case JsonArray a: return a.Count > 0 ? 1 : 0;
                case JsonValue v:
                    if (TryLong(v, out var l)) return l;
                    if (TryDouble(v, out var d)) return d;
                    if (v.TryGetValue<bool>(out var b)) return b ? 1 : 0;
                    if (v.TryGetValue<string>(out var s)) return StringToFloat(s);
                    return 0;
                default: return 0;
            }
        }

        public static long DoubleToInt(double d)
        {
            if (double.IsNaN(d) || double.IsInfinity(d)) return 0;
            if (d >= 9.2233720368547758E18 || d <= -9.2233720368547758E18) return 0;
            return (long)Math.Truncate(d);
        }

        private const string PhpWhitespace = " \t\n\r\v\f";

        /// <summary>Longest numeric prefix of a string, PHP style (leading whitespace allowed).</summary>
        private static string NumericPrefix(string s, out bool isFloatSyntax)
        {
            isFloatSyntax = false;
            var i = 0;
            while (i < s.Length && PhpWhitespace.IndexOf(s[i]) >= 0) i++;
            var start = i;
            if (i < s.Length && (s[i] == '+' || s[i] == '-')) i++;
            var digitsStart = i;
            while (i < s.Length && s[i] >= '0' && s[i] <= '9') i++;
            var intDigits = i - digitsStart;
            var fracDigits = 0;
            if (i < s.Length && s[i] == '.')
            {
                var j = i + 1;
                while (j < s.Length && s[j] >= '0' && s[j] <= '9') j++;
                fracDigits = j - i - 1;
                if (intDigits > 0 || fracDigits > 0)
                {
                    i = j;
                    isFloatSyntax = true;
                }
            }
            if (intDigits == 0 && fracDigits == 0) return "";
            if (i < s.Length && (s[i] == 'e' || s[i] == 'E'))
            {
                var j = i + 1;
                if (j < s.Length && (s[j] == '+' || s[j] == '-')) j++;
                var expStart = j;
                while (j < s.Length && s[j] >= '0' && s[j] <= '9') j++;
                if (j > expStart)
                {
                    i = j;
                    isFloatSyntax = true;
                }
            }
            return s.Substring(start, i - start);
        }

        public static long StringToInt(string s)
        {
            var prefix = NumericPrefix(s, out var isFloat);
            if (prefix.Length == 0) return 0;
            if (!isFloat)
            {
                if (long.TryParse(prefix, NumberStyles.AllowLeadingSign, Inv, out var l)) return l;
                return prefix.StartsWith("-", StringComparison.Ordinal) ? long.MinValue : long.MaxValue;
            }
            return DoubleToInt(double.Parse(prefix, NumberStyles.Float, Inv));
        }

        public static double StringToFloat(string s)
        {
            var prefix = NumericPrefix(s, out _);
            if (prefix.Length == 0) return 0;
            return double.Parse(prefix, NumberStyles.Float, Inv);
        }

        /// <summary>PHP is_numeric().</summary>
        public static bool IsNumeric(JsonNode n)
        {
            if (IsInt(n) || IsFloat(n)) return true;
            if (n is JsonValue v && v.TryGetValue<string>(out var s)) return IsNumericString(s);
            return false;
        }

        public static bool IsNumericString(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            var prefix = NumericPrefix(s, out _);
            if (prefix.Length == 0) return false;
            // NumericPrefix skipped leading whitespace; the rest may only be trailing whitespace.
            var lead = 0;
            while (lead < s.Length && PhpWhitespace.IndexOf(s[lead]) >= 0) lead++;
            var rest = s.Substring(lead + prefix.Length);
            return rest.All(ch => PhpWhitespace.IndexOf(ch) >= 0);
        }

        /// <summary>PHP float to string conversion (precision=14).</summary>
        public static string FloatToString(double d)
        {
            if (double.IsNaN(d)) return "NAN";
            if (double.IsPositiveInfinity(d)) return "INF";
            if (double.IsNegativeInfinity(d)) return "-INF";
            if (d == 0) return BitConverter.DoubleToInt64Bits(d) < 0 ? "-0" : "0";
            var s = d.ToString("G14", Inv);
            var e = s.IndexOf('E');
            if (e < 0) return s;
            var mantissa = s.Substring(0, e);
            if (mantissa.IndexOf('.') < 0) mantissa += ".0";
            var exp = int.Parse(s.Substring(e + 1), NumberStyles.AllowLeadingSign, Inv);
            return mantissa + "E" + (exp < 0 ? "-" : "+") + Math.Abs(exp).ToString(Inv);
        }

        /// <summary>number_format($value, $decimals, '.', '').</summary>
        public static string NumberFormat(double value, int decimals)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) return FloatToString(value);
            string text;
            if (Math.Abs(value) < 7.9e27)
            {
                // decimal conversion keeps 15 significant digits, which mirrors PHP's
                // pre-rounding (number_format(1.005, 2) === "1.01").
                var m = Math.Round((decimal)value, decimals, MidpointRounding.AwayFromZero);
                if (m == 0m) m = 0m;
                text = m.ToString("F" + decimals.ToString(Inv), Inv);
            }
            else
            {
                text = Math.Round(value, Math.Min(decimals, 15), MidpointRounding.AwayFromZero).ToString("F" + decimals.ToString(Inv), Inv);
            }
            return text;
        }

        // ------------------------------------------------------------------
        // Strings (byte-agnostic: these mirror PHP 8's ASCII-only behaviour)
        // ------------------------------------------------------------------

        private const string TrimChars = " \t\n\r\0\x0B";

        public static string Trim(string s) => s?.Trim(TrimChars.ToCharArray()) ?? "";
        public static string Trim(string s, string chars) => s?.Trim(chars.ToCharArray()) ?? "";
        public static string LTrim(string s, string chars) => s?.TrimStart(chars.ToCharArray()) ?? "";
        public static string RTrim(string s, string chars) => s?.TrimEnd(chars.ToCharArray()) ?? "";

        public static string Lower(string s)
        {
            if (s == null) return "";
            var chars = s.ToCharArray();
            for (var i = 0; i < chars.Length; i++) if (chars[i] >= 'A' && chars[i] <= 'Z') chars[i] = (char)(chars[i] + 32);
            return new string(chars);
        }

        public static string Upper(string s)
        {
            if (s == null) return "";
            var chars = s.ToCharArray();
            for (var i = 0; i < chars.Length; i++) if (chars[i] >= 'a' && chars[i] <= 'z') chars[i] = (char)(chars[i] - 32);
            return new string(chars);
        }

        public static string UcFirst(string s)
        {
            if (string.IsNullOrEmpty(s)) return s ?? "";
            var c = s[0];
            if (c >= 'a' && c <= 'z') return (char)(c - 32) + s.Substring(1);
            return s;
        }

        public static bool EqualsIgnoreCase(string a, string b) => string.Equals(Lower(a), Lower(b), StringComparison.Ordinal);

        /// <summary>strlen(): byte length of the UTF-8 encoding.</summary>
        public static int StrLen(string s) => s == null ? 0 : Encoding.UTF8.GetByteCount(s);

        /// <summary>substr($s, -$n) for the common "last n characters" case.</summary>
        public static string Last(string s, int n) => s.Length >= n ? s.Substring(s.Length - n) : s;

        // ------------------------------------------------------------------
        // Query strings
        // ------------------------------------------------------------------

        /// <summary>http_build_query($data) with PHP_QUERY_RFC1738 encoding.</summary>
        public static string BuildQuery(JsonNode data)
        {
            var parts = new List<string>();
            foreach (var kv in Entries(data)) AppendQuery(parts, UrlEncode(kv.Key), kv.Value);
            return string.Join("&", parts);
        }

        private static void AppendQuery(List<string> parts, string encodedKey, JsonNode value)
        {
            if (value == null) return;
            if (IsArray(value))
            {
                foreach (var kv in Entries(value)) AppendQuery(parts, encodedKey + "%5B" + UrlEncode(kv.Key) + "%5D", kv.Value);
                return;
            }
            string text;
            if (IsBool(value)) text = value.GetValue<bool>() ? "1" : "0";
            else text = ToStr(value);
            parts.Add(encodedKey + "=" + UrlEncode(text));
        }

        /// <summary>PHP urlencode().</summary>
        public static string UrlEncode(string s)
        {
            var sb = new StringBuilder();
            foreach (var b in Encoding.UTF8.GetBytes(s ?? ""))
            {
                var c = (char)b;
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '-' || c == '_' || c == '.')
                {
                    sb.Append(c);
                }
                else if (c == ' ')
                {
                    sb.Append('+');
                }
                else
                {
                    sb.Append('%').Append(b.ToString("X2", Inv));
                }
            }
            return sb.ToString();
        }

        // ------------------------------------------------------------------
        // JSON encoding (PHP json_encode default flags)
        // ------------------------------------------------------------------

        /// <summary>json_encode($value) with PHP's default flags (escaped slashes and unicode).</summary>
        public static string JsonEncode(JsonNode n)
        {
            var sb = new StringBuilder();
            Encode(sb, n);
            return sb.ToString();
        }

        private static void Encode(StringBuilder sb, JsonNode n)
        {
            switch (n)
            {
                case null:
                    sb.Append("null");
                    return;
                case JsonObject o:
                    if (o.Count == 0) { sb.Append("[]"); return; }
                    sb.Append('{');
                    var first = true;
                    foreach (var kv in o)
                    {
                        if (!first) sb.Append(',');
                        first = false;
                        EncodeString(sb, kv.Key);
                        sb.Append(':');
                        Encode(sb, kv.Value);
                    }
                    sb.Append('}');
                    return;
                case JsonArray a:
                    sb.Append('[');
                    for (var i = 0; i < a.Count; i++)
                    {
                        if (i > 0) sb.Append(',');
                        Encode(sb, a[i]);
                    }
                    sb.Append(']');
                    return;
                default:
                    if (IsString(n)) EncodeString(sb, n.GetValue<string>());
                    else if (IsBool(n)) sb.Append(n.GetValue<bool>() ? "true" : "false");
                    else if (TryLong((JsonValue)n, out var li)) sb.Append(li.ToString(Inv));
                    else if (TryDouble((JsonValue)n, out var d))
                    {
                        var r = d.ToString("R", Inv);
                        if (r.IndexOfAny(new[] { '.', 'E', 'N', 'I' }) < 0) r += ".0";
                        sb.Append(r.Replace("E+", "e+").Replace("E-", "e-"));
                    }
                    else sb.Append(n.ToJsonString());
                    return;
            }
        }

        private static void EncodeString(StringBuilder sb, string s)
        {
            sb.Append('"');
            foreach (var c in s)
            {
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '/': sb.Append("\\/"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < 0x20 || c > 0x7E) sb.Append("\\u").Append(((int)c).ToString("x4", Inv));
                        else sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
        }
    }
}
