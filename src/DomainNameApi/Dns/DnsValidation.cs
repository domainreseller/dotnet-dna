namespace DomainNameApi.Dns;

/// <summary>Checks the rules the API enforces on zone records, so mistakes fail before anything is sent.</summary>
internal static class DnsValidation
{
    public static void Check(string? name, RecordType type, int ttl, IReadOnlyList<string> contents)
    {
        if (ttl < 1 || ttl > 86400)
        {
            throw new ArgumentException("Ttl must be between 1 and 86400 seconds.", "ttl");
        }
        if (contents.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Contents contains an empty value.", "contents");
        }
        if (type == RecordType.CNAME && (string.IsNullOrWhiteSpace(name) || name!.Trim() == "@"))
        {
            throw new ArgumentException("A CNAME record needs a name such as www; use ALIAS at the apex.", "name");
        }
        if (type == RecordType.CAA)
        {
            foreach (var content in contents)
            {
                if (content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length < 3)
                {
                    throw new ArgumentException(
                        "CAA content needs three parts, for example: 0 issue \"letsencrypt.org\". Got: " + content, "contents");
                }
            }
        }
    }
}
