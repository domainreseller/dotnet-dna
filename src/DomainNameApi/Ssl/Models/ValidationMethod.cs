using System.Text.Json.Serialization;
using DomainNameApi.Internal;

namespace DomainNameApi.Ssl;

/// <summary>
/// How the certificate authority checks that you control the domain (the API's <c>authenticationType</c>):
/// <see cref="Email"/>, <see cref="Cname"/>, <see cref="Http"/> or <see cref="File"/>.
/// </summary>
[JsonConverter(typeof(StringEnumConverter<ValidationMethod>))]
public readonly struct ValidationMethod : IEquatable<ValidationMethod>, IStringEnum
{
    private static readonly string[] Known = ["Email", "Cname", "Http", "File"];
    private readonly string? _value;

    /// <summary>Creates a validation method from its name.</summary>
    /// <param name="value">The name, in any case.</param>
    public ValidationMethod(string value) => _value = StringEnum.Normalize(value, Known);

    /// <summary>An approval e-mail to the approver address.</summary>
    public static ValidationMethod Email { get; } = new("Email");

    /// <summary>A DNS CNAME record.</summary>
    public static ValidationMethod Cname { get; } = new("Cname");

    /// <summary>A file served over HTTP on the site.</summary>
    public static ValidationMethod Http { get; } = new("Http");

    /// <summary>File-based validation.</summary>
    public static ValidationMethod File { get; } = new("File");

    /// <summary>The name as the API spells it.</summary>
    public string Value => _value ?? "";

    /// <summary>Converts a name.</summary>
    public static implicit operator ValidationMethod(string value) => new(value);

    /// <summary>Compares two values, ignoring case.</summary>
    public static bool operator ==(ValidationMethod left, ValidationMethod right) => left.Equals(right);

    /// <summary>Compares two values, ignoring case.</summary>
    public static bool operator !=(ValidationMethod left, ValidationMethod right) => !left.Equals(right);

    /// <inheritdoc />
    public bool Equals(ValidationMethod other) => string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ValidationMethod other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    /// <inheritdoc />
    public override string ToString() => Value;
}
