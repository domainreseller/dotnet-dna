using System.Globalization;

namespace DomainNameApi.Dnssec;

/// <summary>
/// A DS record to publish or remove. Copy the values from your DNS provider, or use <see cref="Parse"/>
/// with the DS record text (<c>keytag algorithm digesttype digest</c>).
/// </summary>
public sealed record DsRecordData
{
    /// <summary>Creates an empty record.</summary>
    public DsRecordData()
    {
    }

    /// <summary>Creates a record.</summary>
    /// <param name="keyTag">The key tag.</param>
    /// <param name="algorithm">The algorithm.</param>
    /// <param name="digestType">The digest type.</param>
    /// <param name="digest">The digest, in hexadecimal.</param>
    public DsRecordData(int keyTag, DnssecAlgorithm algorithm, DsDigestType digestType, string digest)
    {
        KeyTag = keyTag;
        Algorithm = algorithm;
        DigestType = digestType;
        Digest = digest;
    }

    /// <summary>The key tag.</summary>
    public int KeyTag { get; init; }

    /// <summary>The algorithm.</summary>
    public DnssecAlgorithm Algorithm { get; init; }

    /// <summary>The digest type.</summary>
    public DsDigestType DigestType { get; init; }

    /// <summary>The digest, in hexadecimal.</summary>
    public string? Digest { get; init; }

    /// <summary>
    /// Reads a DS record in presentation format, for example
    /// <c>12345 13 2 49FD46E6C4B45C55D4AC69CBD3CD34AC1AFE51DE</c>. A leading <c>example.com. IN DS</c> is ignored.
    /// </summary>
    /// <param name="text">The DS record text.</param>
    /// <exception cref="FormatException">The text is not a DS record.</exception>
    public static DsRecordData Parse(string text)
    {
        var parts = (text ?? "").Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        var ds = parts.FindIndex(p => p.Equals("DS", StringComparison.OrdinalIgnoreCase));
        if (ds >= 0)
        {
            parts = parts.Skip(ds + 1).ToList();
        }
        if (parts.Count < 4 ||
            !int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var keyTag) ||
            !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var algorithm) ||
            !int.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out var digestType))
        {
            throw new FormatException("Expected a DS record such as '12345 13 2 49FD46E6...', got '" + text + "'.");
        }
        return new DsRecordData(keyTag, (DnssecAlgorithm)algorithm, (DsDigestType)digestType, string.Concat(parts.Skip(3)));
    }
}
