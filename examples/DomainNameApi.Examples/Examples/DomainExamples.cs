using DomainNameApi.Contacts;
using DomainNameApi.Domains;

namespace DomainNameApi.Examples;

internal static class DomainExamples
{
    public static async Task SearchAsync(DomainNameApiClient client, string[] args)
    {
        var result = await client.Domains.SearchAsync(Arg(args, 0, "domain"));
        var info = result.Info;

        Console.WriteLine(info is null
            ? $"No answer: {result.Reason ?? result.OperationMessage}"
            : info.IsAvailable
                ? $"{info.DomainName} is available: {info.Price} {info.Currency} for {info.Period} year(s){(info.IsPremium == true ? " (premium)" : "")}"
                : $"{info.DomainName} is taken: {info.Reason}");
    }

    /// <summary>One name across several TLDs in a single call.</summary>
    public static async Task BulkSearchAsync(DomainNameApiClient client, string[] args)
    {
        var name = Arg(args, 0, "name");
        IEnumerable<string> tlds = args.Length > 1 ? args.Skip(1) : ["com", "net", "org", "com.tr"];

        var result = await client.Domains.BulkSearchAsync(tlds.Select(tld => name + "." + tld.TrimStart('.')));

        foreach (var domain in result.Results)
        {
            Console.WriteLine($"{domain.DomainName,-30} {(domain.IsAvailable ? "available" : "taken"),-10} {domain.Price} {domain.Currency}");
        }
    }

    /// <summary>Domains that expire within 30 days, across all pages.</summary>
    public static async Task ListExpiringAsync(DomainNameApiClient client, string[] args)
    {
        await foreach (var domain in client.Domains.ListAllAsync(new ListDomainsRequest { Statuses = { DomainStatus.Active } }))
        {
            if (domain.RemainingDays <= 30)
            {
                Console.WriteLine($"{domain.DomainName,-30} expires {domain.ExpirationDate:yyyy-MM-dd} ({domain.RemainingDays} days)");
            }
        }
    }

    public static async Task ShowDetailsAsync(DomainNameApiClient client, string[] args)
    {
        var domain = await client.Domains.GetAsync(Arg(args, 0, "domain"));

        Console.WriteLine($"{domain.DomainName}: {domain.Status}, expires {domain.ExpirationDate:yyyy-MM-dd}");
        Console.WriteLine($"Locked: {domain.IsLocked}, privacy: {domain.IsPrivacyProtected}");
        Console.WriteLine($"Nameservers: {string.Join(", ", domain.NameServers)}");
        Console.WriteLine($"Registrant: {domain.GetContact(ContactType.Registrant)?.Email}");
        foreach (var host in domain.Hosts)
        {
            Console.WriteLine($"Child nameserver {host.Name}: {string.Join(", ", host.IpAddresses.Select(a => a.IpAddress))}");
        }
    }

    /// <summary>
    /// Registers a domain with one contact for all four roles. This charges the balance, so the example
    /// refuses to run outside the OTE test environment and checks the price first.
    /// </summary>
    public static async Task RegisterAsync(DomainNameApiClient client, string[] args)
    {
        var domainName = Arg(args, 0, "domain");
        if (client.Environment != ApiEnvironment.Ote)
        {
            Console.WriteLine("This example only registers in the OTE test environment. Unset DNA_ENV or set it to ote.");
            return;
        }

        var search = await client.Domains.SearchAsync(domainName);
        if (search.Info?.IsAvailable != true)
        {
            Console.WriteLine($"{domainName} cannot be registered: {search.Info?.Reason}");
            return;
        }
        Console.WriteLine($"Registering {domainName} for {search.Info.Price} {search.Info.Currency}...");

        var contact = new ContactInfo
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = "ada@example.com",
            Address = "1 Example Street",
            City = "Istanbul",
            State = "Istanbul",
            PostalCode = "34000",
            Country = "TR",
            PhoneCountryCode = "90",
            Phone = "5551234567",
        };

        var result = await client.Domains.RegisterAsync(domainName, contact, ["tr.apiname.com", "eu.apiname.com"]);

        Console.WriteLine($"{(result.Success ? "Registered" : "Not registered")}: {result.Message} (expires {result.ExpirationDate:yyyy-MM-dd})");
    }

    internal static string Arg(string[] args, int index, string name) =>
        args.Length > index ? args[index] : throw new ArgumentException("Missing argument: <" + name + ">.");
}
