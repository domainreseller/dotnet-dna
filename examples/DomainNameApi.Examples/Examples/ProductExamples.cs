namespace DomainNameApi.Examples;

internal static class ProductExamples
{
    public static async Task ShowTldAsync(DomainNameApiClient client, string[] args)
    {
        var tld = await client.Products.GetTldAsync(DomainExamples.Arg(args, 0, "tld"));
        if (tld is null)
        {
            Console.WriteLine("This TLD is not sold.");
            return;
        }

        Console.WriteLine($".{tld.Name}: {tld.MinRegistrationPeriod}-{tld.MaxRegistrationPeriod} years");
        Console.WriteLine($"Register: {tld.GetRegistrationPrice()?.Price} {tld.GetRegistrationPrice()?.Currency}, renew: {tld.GetRenewalPrice()?.Price}");
        Console.WriteLine($"Transfer: {tld.SupportsTransfer}, DNSSEC: {tld.SupportsDnssec}, privacy: {tld.SupportsPrivacy}, documents: {tld.RequiresDocument}");
        foreach (var attribute in tld.Attributes)
        {
            Console.WriteLine($"Attribute {attribute.Key}{(attribute.IsRequired ? " (required)" : "")}: {attribute.Description}");
        }
    }
}
