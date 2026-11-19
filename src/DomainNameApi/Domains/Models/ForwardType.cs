using System.Text.Json.Serialization;
using DomainNameApi.Internal;

namespace DomainNameApi.Domains;

/// <summary>How a domain forwards to a URL: <see cref="Standard"/> (HTTP 301) or <see cref="Frame"/> (masked).</summary>
[JsonConverter(typeof(StringEnumConverter<ForwardType>))]
public readonly struct ForwardType : IEquatable<ForwardType>, IStringEnum
{
    private static readonly string[] Known = ["Standard", "Frame"];
    private readonly string? _value;

    /// <summary>Creates a forward type from its name.</summary>
    /// <param name="value">The name, in any case.</param>
    public ForwardType(string value) => _value = StringEnum.Normalize(value, Known);

    /// <summary>An HTTP 301 redirect.</summary>
    public static ForwardType Standard { get; } = new("Standard");

    /// <summary>Shows the target inside a frame, keeping the domain in the address bar.</summary>
    public static ForwardType Frame { get; } = new("Frame");

    /// <summary>The name as the API spells it.</summary>
    public string Value => _value ?? "";

    /// <summary>Converts a name.</summary>
    public static implicit operator ForwardType(string value) => new(value);

    /// <summary>Compares two values, ignoring case.</summary>
    public static bool operator ==(ForwardType left, ForwardType right) => left.Equals(right);

    /// <summary>Compares two values, ignoring case.</summary>
    public static bool operator !=(ForwardType left, ForwardType right) => !left.Equals(right);

    /// <inheritdoc />
    public bool Equals(ForwardType other) => string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ForwardType other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    /// <inheritdoc />
    public override string ToString() => Value;
}
