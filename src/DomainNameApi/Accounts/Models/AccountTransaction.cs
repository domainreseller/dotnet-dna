namespace DomainNameApi.Accounts;

/// <summary>One entry of the balance ledger: a charge, refund or deposit.</summary>
public sealed record AccountTransaction
{
    /// <summary>The trace ID that ties the transaction to an operation.</summary>
    public string? TraceId { get; init; }

    /// <summary>What the transaction was for.</summary>
    public TransactionType Type { get; init; }

    /// <summary>The outcome.</summary>
    public TransactionStatus Status { get; init; }

    /// <summary>Whether money was added or taken.</summary>
    public ChargeType ChargeType { get; init; }

    /// <summary>The amount in <see cref="AmountCurrency"/>.</summary>
    public decimal Amount { get; init; }

    /// <summary>The currency of <see cref="Amount"/>.</summary>
    public string? AmountCurrency { get; init; }

    /// <summary>The amount after currency conversion.</summary>
    public decimal ExchangeAmount { get; init; }

    /// <summary>The exchange figure the API reports with <see cref="ExchangeAmount"/> (sent as <c>exchangeCurrency</c>).</summary>
    public decimal ExchangeCurrency { get; init; }

    /// <summary>The kind of product involved.</summary>
    public ProductType ProductType { get; init; }

    /// <summary>The product's ID.</summary>
    public Guid? ProductId { get; init; }

    /// <summary>A description, usually naming the domain or product.</summary>
    public string? Description { get; init; }

    /// <summary>The period in years.</summary>
    public int Period { get; init; }

    /// <summary>When the transaction was recorded.</summary>
    public DateTime CreationTime { get; init; }

    /// <summary>The reseller the transaction belongs to.</summary>
    public string? ResellerName { get; init; }

    /// <summary>The balance after the transaction.</summary>
    public decimal Balance { get; init; }

    /// <summary>The balance before the transaction.</summary>
    public decimal OldBalance { get; init; }
}
