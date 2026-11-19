using System.Text.Json.Serialization;

namespace DomainNameApi.Products;

/// <summary>An SSL certificate product with its prices.</summary>
public sealed record SslProduct
{
    /// <summary>The product name.</summary>
    public string? Name { get; init; }

    /// <summary>The product code, used to buy the certificate.</summary>
    public string? ProductCode { get; init; }

    /// <summary>The certificate authority.</summary>
    public string? VendorName { get; init; }

    /// <summary>The validation level (DV, OV, EV).</summary>
    public string? ValidationType { get; init; }

    /// <summary>Whether it covers <c>*.example.com</c>.</summary>
    public bool IsWildcard { get; init; }

    /// <summary>Whether it covers several domain names.</summary>
    public bool IsMultiDomain { get; init; }

    /// <summary>Domain names included in the price.</summary>
    public int DefaultSanCount { get; init; }

    /// <summary>The most domain names it can cover.</summary>
    public int MaxSanCount { get; init; }

    /// <summary>The warranty amount.</summary>
    public decimal? WarrantyAmount { get; init; }

    /// <summary>The usual time to issue, in minutes.</summary>
    public int IssueTimeMinutes { get; init; }

    /// <summary>Whether reissuing is free.</summary>
    public bool IsReissueFree { get; init; }

    /// <summary>Whether a CSR is needed.</summary>
    public bool RequiresCsr { get; init; }

    /// <summary>Prices per price group.</summary>
    public IReadOnlyList<SslPriceGroup> Prices { get; init; } = [];
}

/// <summary>The prices of an SSL product in one price group.</summary>
public sealed record SslPriceGroup
{
    /// <summary>The price group.</summary>
    public string? PriceGroup { get; init; }

    /// <summary>Purchase prices per period.</summary>
    [JsonPropertyName("create")]
    public IReadOnlyList<SslPrice> Purchase { get; init; } = [];
}

/// <summary>A price for a period.</summary>
public sealed record SslPrice
{
    /// <summary>The period in years.</summary>
    public int Period { get; init; }

    /// <summary>The price.</summary>
    public decimal Price { get; init; }

    /// <summary>The currency.</summary>
    public string? Currency { get; init; }
}
