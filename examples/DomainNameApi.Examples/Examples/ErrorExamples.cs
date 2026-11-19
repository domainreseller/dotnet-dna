namespace DomainNameApi.Examples;

internal static class ErrorExamples
{
    /// <summary>Shows how failures look: a missing domain, then an invalid request that is never sent.</summary>
    public static async Task ShowAsync(DomainNameApiClient client, string[] args)
    {
        try
        {
            await client.Domains.GetAsync("this-domain-is-not-in-your-account-" + Guid.NewGuid().ToString("N").Substring(0, 8) + ".com");
        }
        catch (DomainNameApiException e) when (e.Kind == ApiErrorKind.NotFound)
        {
            Console.WriteLine($"Not found, as expected. Code {e.Code}, HTTP {e.StatusCode}, correlation ID {e.CorrelationId}");
        }

        try
        {
            await client.Domains.RenewAsync("example.com", years: 0);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine("Rejected before sending: " + e.Message);
        }
    }
}
