using System.Text.Json;
using System.Text.Json.Serialization;

namespace DomainNameApi.Internal;

/// <summary>Implemented by the SDK's text enums such as <c>RecordType</c> and <c>ContactType</c>.</summary>
internal interface IStringEnum
{
    string Value { get; }
}

internal static class StringEnum
{
    /// <summary>Trims the value and replaces it with the canonical spelling when it matches a known value in any case.</summary>
    public static string Normalize(string value, string[] known)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        var trimmed = value.Trim();
        foreach (var k in known)
        {
            if (string.Equals(k, trimmed, StringComparison.OrdinalIgnoreCase))
            {
                return k;
            }
        }
        return trimmed;
    }

    public static bool IsKnown(string value, string[] known) => Array.IndexOf(known, value) >= 0;
}

/// <summary>Reads and writes a text enum as a JSON string.</summary>
internal sealed class StringEnumConverter<T> : JsonConverter<T> where T : struct, IStringEnum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return default;
        }
        var text = reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString() ?? "",
            JsonTokenType.Number => reader.GetDecimal().ToString(System.Globalization.CultureInfo.InvariantCulture),
            _ => throw new JsonException("Expected a string for " + typeof(T).Name + "."),
        };
        return (T)Activator.CreateInstance(typeof(T), text)!;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}
