using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DomainNameApi.Internal;

/// <summary>
/// Reads value types the way a loosely typed API sends them: <c>null</c> and <c>""</c> become the default
/// value instead of failing the whole response, numbers and booleans are also accepted as strings, and enums
/// are accepted as numbers or names. Writing is unchanged.
/// </summary>
internal sealed class LenientConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsEnum ||
        typeToConvert == typeof(DateTime) || typeToConvert == typeof(DateTimeOffset) || typeToConvert == typeof(Guid) ||
        typeToConvert == typeof(bool) || typeToConvert == typeof(int) || typeToConvert == typeof(long) ||
        typeToConvert == typeof(decimal) || typeToConvert == typeof(double);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert.IsEnum)
        {
            return (JsonConverter)Activator.CreateInstance(typeof(EnumConverter<>).MakeGenericType(typeToConvert))!;
        }
        if (typeToConvert == typeof(DateTime)) return new Lenient<DateTime>(ReadDateTime, (w, v) => w.WriteStringValue(v));
        if (typeToConvert == typeof(DateTimeOffset)) return new Lenient<DateTimeOffset>(ReadDateTimeOffset, (w, v) => w.WriteStringValue(v));
        if (typeToConvert == typeof(Guid)) return new Lenient<Guid>(ReadGuid, (w, v) => w.WriteStringValue(v));
        if (typeToConvert == typeof(bool)) return new Lenient<bool>(ReadBool, (w, v) => w.WriteBooleanValue(v));
        if (typeToConvert == typeof(int)) return new Lenient<int>(r => (int)ReadNumber(ref r), (w, v) => w.WriteNumberValue(v));
        if (typeToConvert == typeof(long)) return new Lenient<long>(r => (long)ReadNumber(ref r), (w, v) => w.WriteNumberValue(v));
        if (typeToConvert == typeof(double)) return new Lenient<double>(r => (double)ReadNumber(ref r), (w, v) => w.WriteNumberValue(v));
        return new Lenient<decimal>(r => ReadNumber(ref r), (w, v) => w.WriteNumberValue(v));
    }

    private delegate T Reader<out T>(Utf8JsonReader reader);

    private sealed class Lenient<T> : JsonConverter<T> where T : struct
    {
        private readonly Reader<T> _read;
        private readonly Action<Utf8JsonWriter, T> _write;

        public Lenient(Reader<T> read, Action<Utf8JsonWriter, T> write)
        {
            _read = read;
            _write = write;
        }

        public override bool HandleNull => true;

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null ||
                (reader.TokenType == JsonTokenType.String && reader.ValueSpan.Length == 0 && !reader.ValueIsEscaped))
            {
                return default;
            }
            try
            {
                return _read(reader);
            }
            catch (Exception e) when (e is FormatException or InvalidOperationException or OverflowException)
            {
                throw new JsonException("Could not read " + typeof(T).Name + ": " + e.Message, e);
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) => _write(writer, value);
    }

    private sealed class EnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override bool HandleNull => true;

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return default;
                case JsonTokenType.Number:
                    return (T)Enum.ToObject(typeof(T), reader.GetInt64());
                case JsonTokenType.String:
                    var text = (reader.GetString() ?? "").Trim();
                    if (text.Length == 0)
                    {
                        return default;
                    }
                    if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
                    {
                        return (T)Enum.ToObject(typeof(T), number);
                    }
                    if (Enum.TryParse<T>(text, ignoreCase: true, out var named))
                    {
                        return named;
                    }
                    throw new JsonException("'" + text + "' is not a " + typeof(T).Name + " value.");
                default:
                    throw new JsonException("Expected a number or a string for " + typeof(T).Name + ".");
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
            writer.WriteNumberValue(Convert.ToInt64(value, CultureInfo.InvariantCulture));
    }

    private static DateTime ReadDateTime(Utf8JsonReader reader)
    {
        if (reader.TokenType == JsonTokenType.String && reader.TryGetDateTime(out var value))
        {
            return value;
        }
        return DateTime.Parse(ReadText(reader), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }

    private static DateTimeOffset ReadDateTimeOffset(Utf8JsonReader reader)
    {
        if (reader.TokenType == JsonTokenType.String && reader.TryGetDateTimeOffset(out var value))
        {
            return value;
        }
        return DateTimeOffset.Parse(ReadText(reader), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
    }

    private static Guid ReadGuid(Utf8JsonReader reader) => Guid.Parse(ReadText(reader));

    private static bool ReadBool(Utf8JsonReader reader) => reader.TokenType switch
    {
        JsonTokenType.True => true,
        JsonTokenType.False => false,
        JsonTokenType.Number => reader.GetDecimal() != 0,
        _ => ReadText(reader).Trim().ToLowerInvariant() switch
        {
            "true" or "1" or "yes" or "on" => true,
            "false" or "0" or "no" or "off" => false,
            var other => throw new FormatException("'" + other + "' is not a boolean."),
        },
    };

    private static decimal ReadNumber(ref Utf8JsonReader reader)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetDecimal(out var exact))
            {
                return exact;
            }
            return (decimal)reader.GetDouble();
        }
        return decimal.Parse(ReadText(reader).Trim(), NumberStyles.Float, CultureInfo.InvariantCulture);
    }

    private static string ReadText(Utf8JsonReader reader) => reader.TokenType switch
    {
        JsonTokenType.String => reader.GetString() ?? "",
        _ => throw new InvalidOperationException("Unexpected JSON token " + reader.TokenType + "."),
    };
}
