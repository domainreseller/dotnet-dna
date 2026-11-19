namespace DomainNameApi.Accounts;

/// <summary><c>GET deposit/transactions</c>: pages through the balance ledger, with optional filters.</summary>
public sealed class ListTransactionsRequest : PagedRequest<AccountTransaction>
{
    /// <summary>Only these transaction types.</summary>
    public List<TransactionType> TransactionTypes { get; set; } = [];

    /// <summary>Only these product types.</summary>
    public List<ProductType> ProductTypes { get; set; } = [];

    /// <summary>Only these account currencies, for example <c>USD</c> or <c>TRY</c>.</summary>
    public List<string> Currencies { get; set; } = [];

    /// <summary>Only credits or only debits.</summary>
    public ChargeType? ChargeType { get; set; }

    /// <summary>Only the transaction with this trace ID.</summary>
    public string? TraceId { get; set; }

    /// <summary>Only transactions whose description contains this text.</summary>
    public string? Description { get; set; }

    /// <summary>Only transactions of this amount.</summary>
    public decimal? Amount { get; set; }

    /// <summary>From this time.</summary>
    public DateTimeOffset? StartDate { get; set; }

    /// <summary>Until this time.</summary>
    public DateTimeOffset? EndDate { get; set; }

    /// <summary>Free-text search.</summary>
    public string? FilterText { get; set; }

    /// <inheritdoc />
    public override string Path => "deposit/transactions";

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query)
    {
        query.AddEach("TransactionTypes", TransactionTypes)
             .AddEach("ProductTypes", ProductTypes)
             .AddEach("AccountCurrencyTypes", Currencies)
             .Add("ChargeType", ChargeType)
             .Add("TraceId", TraceId)
             .Add("Description", Description)
             .Add("Amount", Amount)
             .Add("StartDate", StartDate)
             .Add("EndDate", EndDate)
             .Add("FilterText", FilterText);
        base.AddQueryParameters(query);
    }
}
