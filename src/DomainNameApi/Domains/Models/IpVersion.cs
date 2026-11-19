using System.Text.Json.Serialization;
using DomainNameApi.Internal;

namespace DomainNameApi.Domains;

/// <summary>The IP version of a child nameserver address: <see cref="V4"/> or <see cref="V6"/>.</summary>
[JsonConverter(typeof(StringEnumConverter<IpVersion>))]
public readonly struct IpVersion : IEquatable<IpVersion>, IStringEnum
{
    private static readonly string[] Known = ["v4", "v6"];
    private readonly string? _value;

    /// <summary>Creates an IP version from its name (<c>v4</c> or <c>v6</c>).</summary>
    /// <param name="value">The name, in any case.</param>
    public IpVersion(string value) => _value = StringEnum.Normalize(value, Known);

    /// <summary>IPv4.</summary>
    public static IpVersion V4 { get; } = new("v4");

    /// <summary>IPv6. Rejected when the TLD does not support IPv6 glue.</summary>
    public static IpVersion V6 { get; } = new("v6");

    /// <summary>The name as the API spells it.</summary>
    public string Value => _value ?? "";

    /// <summary>Converts a name.</summary>
    public static implicit operator IpVersion(string value) => new(value);

    /// <summary>Compares two values, ignoring case.</summary>
    public static bool operator ==(IpVersion left, IpVersion right) => left.Equals(right);

    /// <summary>Compares two values, ignoring case.</summary>
    public static bool operator !=(IpVersion left, IpVersion right) => !left.Equals(right);

    /// <inheritdoc />
    public bool Equals(IpVersion other) => string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is IpVersion other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    /// <inheritdoc />
    public override string ToString() => Value;
}
