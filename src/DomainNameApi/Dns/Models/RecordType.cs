using System.Text.Json.Serialization;
using DomainNameApi.Internal;

namespace DomainNameApi.Dns;

/// <summary>
/// A DNS record type such as <see cref="A"/>, <see cref="CNAME"/> or <see cref="TXT"/>. The API is case-sensitive;
/// any spelling of a known type is normalised (<c>"txt"</c> becomes <c>TXT</c>, <c>"dnskey"</c> becomes <c>DNSKey</c>).
/// </summary>
[JsonConverter(typeof(StringEnumConverter<RecordType>))]
public readonly struct RecordType : IEquatable<RecordType>, IStringEnum
{
    private static readonly string[] Known =
    [
        "A", "AAAA", "ALIAS", "NS", "CNAME", "MX", "SOA", "TXT", "PTR", "SRV", "DNSKey", "DS", "NSEC", "NSEC3",
        "NSEC3Param", "RRSIG", "AFSDB", "ATMA", "CAA", "CERT", "DHCID", "DName", "HInfo", "ISDN", "LOC", "MB", "MG",
        "MInfo", "MR", "NAPTR", "NSAP", "RP", "RT", "TLSA", "X25",
    ];

    private readonly string? _value;

    /// <summary>Creates a record type from its name.</summary>
    /// <param name="value">The type name, in any case.</param>
    public RecordType(string value) => _value = StringEnum.Normalize(value, Known);

    /// <summary>IPv4 address.</summary>
    public static RecordType A { get; } = new("A");

    /// <summary>IPv6 address.</summary>
    public static RecordType AAAA { get; } = new("AAAA");

    /// <summary>Alias for the zone apex (CNAME-like, allowed at <c>@</c>).</summary>
    public static RecordType ALIAS { get; } = new("ALIAS");

    /// <summary>Certification authority authorisation. Content needs three parts: <c>0 issue "letsencrypt.org"</c>.</summary>
    public static RecordType CAA { get; } = new("CAA");

    /// <summary>Canonical name. Needs a record name (not the apex).</summary>
    public static RecordType CNAME { get; } = new("CNAME");

    /// <summary>Mail exchanger, for example <c>10 mail.example.com</c>.</summary>
    public static RecordType MX { get; } = new("MX");

    /// <summary>Naming authority pointer.</summary>
    public static RecordType NAPTR { get; } = new("NAPTR");

    /// <summary>Nameserver delegation.</summary>
    public static RecordType NS { get; } = new("NS");

    /// <summary>Pointer (reverse DNS).</summary>
    public static RecordType PTR { get; } = new("PTR");

    /// <summary>Service locator, for example <c>10 5 5060 sip.example.com</c>.</summary>
    public static RecordType SRV { get; } = new("SRV");

    /// <summary>TLS certificate association (DANE).</summary>
    public static RecordType TLSA { get; } = new("TLSA");

    /// <summary>Text, for example an SPF or verification value.</summary>
    public static RecordType TXT { get; } = new("TXT");

    /// <summary>The type name as the API spells it.</summary>
    public string Value => _value ?? "";

    /// <summary>Whether the type is one the API documents.</summary>
    public bool IsKnown => StringEnum.IsKnown(Value, Known);

    /// <summary>Converts a type name.</summary>
    public static implicit operator RecordType(string value) => new(value);

    /// <summary>Compares two types, ignoring case.</summary>
    public static bool operator ==(RecordType left, RecordType right) => left.Equals(right);

    /// <summary>Compares two types, ignoring case.</summary>
    public static bool operator !=(RecordType left, RecordType right) => !left.Equals(right);

    /// <inheritdoc />
    public bool Equals(RecordType other) => string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is RecordType other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    /// <inheritdoc />
    public override string ToString() => Value;
}
