using System.Text.Json.Serialization;
using DomainNameApi.Internal;

namespace DomainNameApi.Ssl;

/// <summary>The web server software the certificate is for.</summary>
[JsonConverter(typeof(StringEnumConverter<WebServerType>))]
public readonly struct WebServerType : IEquatable<WebServerType>, IStringEnum
{
    private static readonly string[] Known = ["Apache", "IIS", "AOL", "C2Net"];
    private readonly string? _value;

    /// <summary>Creates a web server type from its name.</summary>
    /// <param name="value">The name, in any case.</param>
    public WebServerType(string value) => _value = StringEnum.Normalize(value, Known);

    /// <summary>Apache, nginx and most other servers that use PEM files.</summary>
    public static WebServerType Apache { get; } = new("Apache");

    /// <summary>Microsoft IIS.</summary>
    public static WebServerType IIS { get; } = new("IIS");

    /// <summary>AOL server.</summary>
    public static WebServerType AOL { get; } = new("AOL");

    /// <summary>C2Net Stronghold.</summary>
    public static WebServerType C2Net { get; } = new("C2Net");

    /// <summary>The name as the API spells it.</summary>
    public string Value => _value ?? "";

    /// <summary>Converts a name.</summary>
    public static implicit operator WebServerType(string value) => new(value);

    /// <summary>Compares two values, ignoring case.</summary>
    public static bool operator ==(WebServerType left, WebServerType right) => left.Equals(right);

    /// <summary>Compares two values, ignoring case.</summary>
    public static bool operator !=(WebServerType left, WebServerType right) => !left.Equals(right);

    /// <inheritdoc />
    public bool Equals(WebServerType other) => string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is WebServerType other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    /// <inheritdoc />
    public override string ToString() => Value;
}
