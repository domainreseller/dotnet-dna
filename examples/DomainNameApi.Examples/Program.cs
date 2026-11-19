using DomainNameApi;
using DomainNameApi.Examples;

// Runs one example against the API:
//
//   dotnet run --project examples/DomainNameApi.Examples -- search example.com
//
// Credentials come from DNA_RESELLER_ID and DNA_API_KEY. The examples use the OTE test environment
// unless DNA_ENV=production is set, so nothing here touches live domains or your live balance.

var examples = new Dictionary<string, (string Usage, Func<DomainNameApiClient, string[], Task> Run)>(StringComparer.OrdinalIgnoreCase)
{
    ["account"] = ("account", AccountExamples.ShowBalanceAsync),
    ["transactions"] = ("transactions", AccountExamples.ShowThisMonthsChargesAsync),
    ["search"] = ("search <domain>", DomainExamples.SearchAsync),
    ["bulk-search"] = ("bulk-search <name> [tld...]", DomainExamples.BulkSearchAsync),
    ["domains"] = ("domains", DomainExamples.ListExpiringAsync),
    ["domain"] = ("domain <domain>", DomainExamples.ShowDetailsAsync),
    ["register"] = ("register <domain>", DomainExamples.RegisterAsync),
    ["dns"] = ("dns <domain>", DnsExamples.ListAsync),
    ["dns-add"] = ("dns-add <domain> <name> <type> <value>", DnsExamples.AddAsync),
    ["tld"] = ("tld <name>", ProductExamples.ShowTldAsync),
    ["errors"] = ("errors", ErrorExamples.ShowAsync),
};

if (args.Length == 0 || !examples.TryGetValue(args[0], out var example))
{
    Console.WriteLine("Usage: dotnet run --project examples/DomainNameApi.Examples -- <example> [arguments]");
    Console.WriteLine();
    foreach (var entry in examples.Values)
    {
        Console.WriteLine("  " + entry.Usage);
    }
    return 1;
}

var options = DomainNameApiOptions.FromEnvironment();
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DNA_ENV")) &&
    string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DNA_TEST_MODE")))
{
    options.Environment = ApiEnvironment.Ote;
}

using var client = new DomainNameApiClient(options);
Console.WriteLine($"Environment: {client.Environment} ({client.BaseAddress})");
Console.WriteLine();

try
{
    await example.Run(client, args.Skip(1).ToArray());
    return 0;
}
catch (DomainNameApiException e)
{
    Console.WriteLine($"{e.Kind} [{e.Code}]: {e.Message}");
    foreach (var error in e.ValidationErrors)
    {
        Console.WriteLine("  - " + error);
    }
    if (e.Hint is not null)
    {
        Console.WriteLine("Hint: " + e.Hint);
    }
    return 2;
}
catch (ArgumentException e)
{
    Console.WriteLine("Invalid input, nothing was sent: " + e.Message);
    return 2;
}
