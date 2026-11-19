using DomainNameApi.Dns;

namespace DomainNameApi.Examples;

internal static class DnsExamples
{
    public static async Task ListAsync(DomainNameApiClient client, string[] args)
    {
        var sets = await client.Dns.ListAsync(DomainExamples.Arg(args, 0, "domain"));

        foreach (var set in sets.OrderBy(s => s.Name).ThenBy(s => s.Type.Value))
        {
            Console.WriteLine($"{set.Name,-20} {set.Type,-6} {set.Ttl,6}  {string.Join(" | ", set.Values)}");
        }
    }

    /// <summary>Adds a value to a record set. Records resolve once the domain uses the platform nameservers.</summary>
    public static async Task AddAsync(DomainNameApiClient client, string[] args)
    {
        var domain = DomainExamples.Arg(args, 0, "domain");
        RecordType type = DomainExamples.Arg(args, 2, "type"); // "txt" becomes TXT

        await client.Dns.AddAsync(domain, DomainExamples.Arg(args, 1, "name"), type, DomainExamples.Arg(args, 3, "value"));

        Console.WriteLine($"Added. Call client.Dns.ApplyAsync(\"{domain}\") to serve the zone from the platform nameservers.");
    }
}
