using System.Text;

namespace DomainNameApi.Internal;

/// <summary>Normalizes the <c>X-App</c> and <c>X-Version</c> header values, the same way the PHP library does.</summary>
internal static class ClientInfo
{
    /// <summary>Lowercase, keeping only <c>a-z</c>, <c>0-9</c>, <c>.</c>, <c>_</c> and <c>-</c>. Empty when nothing is left.</summary>
    public static string NormalizeApp(string? app)
    {
        var builder = new StringBuilder();
        foreach (var c in (app ?? "").Trim().ToLowerInvariant())
        {
            if (c is >= 'a' and <= 'z' or >= '0' and <= '9' or '.' or '_' or '-')
            {
                builder.Append(c);
            }
        }
        return builder.ToString();
    }

    /// <summary>Keeps only digits and dots and trims leading and trailing dots: <c>V2.0.20-beta</c> becomes <c>2.0.20</c>.</summary>
    public static string NormalizeVersion(string? version)
    {
        var builder = new StringBuilder();
        foreach (var c in version ?? "")
        {
            if (c is >= '0' and <= '9' or '.')
            {
                builder.Append(c);
            }
        }
        return builder.ToString().Trim('.');
    }
}
