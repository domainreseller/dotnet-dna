namespace DomainNameApi;

/// <summary>Kind of product.</summary>
public enum ProductType
{
    /// <summary>A domain name.</summary>
    Domain = 1,

    /// <summary>An SSL certificate.</summary>
    Ssl = 2,

    /// <summary>A balance deposit.</summary>
    Deposit = 3,

    /// <summary>A trustee (local presence) service for a domain.</summary>
    DomainTrustee = 4,

    /// <summary>Web hosting.</summary>
    Hosting = 5,

    /// <summary>A domain owner change.</summary>
    OwnerChange = 6,
}
