namespace DomainNameApi.Hosting;

/// <summary>A hosting service plan and its limits. Null limits are unlimited or not reported.</summary>
public sealed record ServicePlan
{
    /// <summary>The plan name.</summary>
    public string? Name { get; init; }

    /// <summary>The plan's ID at the hosting platform.</summary>
    public string? ExternalId { get; init; }

    /// <summary>Disk quota in MB.</summary>
    public long? DiskQuotaMb { get; init; }

    /// <summary>Monthly bandwidth in MB.</summary>
    public long? BandwidthLimitMb { get; init; }

    /// <summary>Domains.</summary>
    public int? MaxDomains { get; init; }

    /// <summary>Subdomains.</summary>
    public int? MaxSubDomains { get; init; }

    /// <summary>Databases.</summary>
    public int? MaxDatabases { get; init; }

    /// <summary>FTP accounts.</summary>
    public int? MaxFtpAccounts { get; init; }

    /// <summary>E-mail accounts.</summary>
    public int? MaxEmailAccounts { get; init; }

    /// <summary>Whether the plan has a dedicated IP address.</summary>
    public bool HasDedicatedIp { get; init; }

    /// <summary>Whether the plan has shell access.</summary>
    public bool HasShellAccess { get; init; }
}
