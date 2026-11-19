using DomainNameApi.Accounts;

namespace DomainNameApi.Examples;

internal static class AccountExamples
{
    /// <summary>Your balances. Also the cheapest way to check that the credentials work.</summary>
    public static async Task ShowBalanceAsync(DomainNameApiClient client, string[] args)
    {
        var account = await client.Account.GetAsync();

        Console.WriteLine($"{account.ResellerName} ({account.ResellerGroupName})");
        Console.WriteLine($"USD balance: {account.UsdBalance:N2}");
        Console.WriteLine($"TRY balance: {account.TryBalance:N2}");
    }

    /// <summary>Every charge since the start of the month, read page by page.</summary>
    public static async Task ShowThisMonthsChargesAsync(DomainNameApiClient client, string[] args)
    {
        var now = DateTimeOffset.UtcNow;
        var request = new ListTransactionsRequest
        {
            ChargeType = ChargeType.Subtract,
            StartDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero),
            Sorting = "creationTime desc",
        };

        var total = 0m;
        await foreach (var transaction in client.Account.ListAllTransactionsAsync(request))
        {
            Console.WriteLine($"{transaction.CreationTime:yyyy-MM-dd}  {transaction.Type,-10} {transaction.Amount,10:N2} {transaction.AmountCurrency}  {transaction.Description}");
            total += transaction.Amount;
        }
        Console.WriteLine($"Total: {total:N2}");
    }
}
