using System.Text.Json.Serialization;
using DomainNameApi.Internal;

namespace DomainNameApi.Contacts;

/// <summary>
/// The role of a contact: <see cref="Registrant"/>, <see cref="Admin"/>, <see cref="Tech"/> or <see cref="Billing"/>.
/// Any spelling is accepted and normalised (<c>"admin"</c> becomes <c>Admin</c>).
/// </summary>
[JsonConverter(typeof(StringEnumConverter<ContactType>))]
public readonly struct ContactType : IEquatable<ContactType>, IStringEnum
{
    private static readonly string[] Known = ["Registrant", "Admin", "Tech", "Billing"];
    private readonly string? _value;

    /// <summary>Creates a contact type from its name.</summary>
    /// <param name="value">The role name, in any case.</param>
    public ContactType(string value) => _value = StringEnum.Normalize(value, Known);

    /// <summary>The domain owner.</summary>
    public static ContactType Registrant { get; } = new("Registrant");

    /// <summary>The administrative contact.</summary>
    public static ContactType Admin { get; } = new("Admin");

    /// <summary>The technical contact.</summary>
    public static ContactType Tech { get; } = new("Tech");

    /// <summary>The billing contact.</summary>
    public static ContactType Billing { get; } = new("Billing");

    /// <summary>All four roles, in the order registries list them.</summary>
    public static IReadOnlyList<ContactType> All { get; } = [Registrant, Admin, Tech, Billing];

    /// <summary>The role name as the API spells it.</summary>
    public string Value => _value ?? "";

    /// <summary>Whether the value is one of the four roles. The API silently treats unknown values as Registrant.</summary>
    public bool IsKnown => StringEnum.IsKnown(Value, Known);

    /// <summary>Converts a role name.</summary>
    public static implicit operator ContactType(string value) => new(value);

    /// <summary>Compares two roles, ignoring case.</summary>
    public static bool operator ==(ContactType left, ContactType right) => left.Equals(right);

    /// <summary>Compares two roles, ignoring case.</summary>
    public static bool operator !=(ContactType left, ContactType right) => !left.Equals(right);

    /// <inheritdoc />
    public bool Equals(ContactType other) => string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ContactType other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    /// <inheritdoc />
    public override string ToString() => Value;
}
