using System.Collections;
using System.Globalization;
using System.Text;

namespace DomainNameApi;

/// <summary>
/// Builds a query string. Null values are skipped, so optional filters can be added unconditionally.
/// Lists are sent as repeated parameters (<c>Types=1&amp;Types=2</c>) and enums as their numbers.
/// </summary>
public sealed class QueryParameters : IEnumerable<KeyValuePair<string, string>>
{
    private readonly List<KeyValuePair<string, string>> _items = [];

    /// <summary>The number of parameters added so far.</summary>
    public int Count => _items.Count;

    /// <summary>Adds a text parameter; null or empty values are skipped.</summary>
    public QueryParameters Add(string name, string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _items.Add(new KeyValuePair<string, string>(name, value!));
        }
        return this;
    }

    /// <summary>Adds a number; null is skipped.</summary>
    public QueryParameters Add(string name, int? value) =>
        value is null ? this : Add(name, value.Value.ToString(CultureInfo.InvariantCulture));

    /// <summary>Adds a number; null is skipped.</summary>
    public QueryParameters Add(string name, long? value) =>
        value is null ? this : Add(name, value.Value.ToString(CultureInfo.InvariantCulture));

    /// <summary>Adds a decimal number; null is skipped.</summary>
    public QueryParameters Add(string name, decimal? value) =>
        value is null ? this : Add(name, value.Value.ToString(CultureInfo.InvariantCulture));

    /// <summary>Adds <c>true</c> or <c>false</c>; null is skipped.</summary>
    public QueryParameters Add(string name, bool? value) =>
        value is null ? this : Add(name, value.Value ? "true" : "false");

    /// <summary>Adds an ISO 8601 date and time; null is skipped.</summary>
    public QueryParameters Add(string name, DateTime? value) =>
        value is null ? this : Add(name, value.Value.ToString("o", CultureInfo.InvariantCulture));

    /// <summary>Adds an ISO 8601 date, time and offset; null is skipped.</summary>
    public QueryParameters Add(string name, DateTimeOffset? value) =>
        value is null ? this : Add(name, value.Value.ToString("o", CultureInfo.InvariantCulture));

    /// <summary>Adds a UUID; null is skipped.</summary>
    public QueryParameters Add(string name, Guid? value) =>
        value is null ? this : Add(name, value.Value.ToString("D"));

    /// <summary>Adds an enum as its number; null is skipped.</summary>
    public QueryParameters Add<TEnum>(string name, TEnum? value) where TEnum : struct, Enum =>
        value is null ? this : Add(name, EnumNumber(value.Value));

    /// <summary>Adds one parameter per value; null or empty values are skipped.</summary>
    public QueryParameters AddEach(string name, IEnumerable<string>? values)
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                Add(name, value);
            }
        }
        return this;
    }

    /// <summary>Adds one parameter per enum value, each as its number.</summary>
    public QueryParameters AddEach<TEnum>(string name, IEnumerable<TEnum>? values) where TEnum : struct, Enum =>
        values is null ? this : AddEach(name, values.Select(EnumNumber));

    /// <summary>Adds the enum values as one comma-separated list of numbers, for example <c>0,7</c>.</summary>
    public QueryParameters AddCommaSeparated<TEnum>(string name, IEnumerable<TEnum>? values) where TEnum : struct, Enum
    {
        if (values is null)
        {
            return this;
        }
        var joined = string.Join(",", values.Select(EnumNumber));
        return Add(name, joined);
    }

    /// <summary>Returns the encoded query string without the leading <c>?</c>.</summary>
    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var item in _items)
        {
            if (sb.Length > 0)
            {
                sb.Append('&');
            }
            sb.Append(Uri.EscapeDataString(item.Key)).Append('=').Append(Uri.EscapeDataString(item.Value));
        }
        return sb.ToString();
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static string EnumNumber<TEnum>(TEnum value) where TEnum : struct, Enum =>
        Convert.ToInt64(value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
}
