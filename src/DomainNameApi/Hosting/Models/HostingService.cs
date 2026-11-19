using System.Text.Json.Serialization;

namespace DomainNameApi.Hosting;

/// <summary>A hosting service in your account.</summary>
public sealed record HostingService
{
    /// <summary>The hosting ID.</summary>
    public Guid HostingId { get; init; }

    /// <summary>The service ID at the hosting platform.</summary>
    public string? ServiceId { get; init; }

    /// <summary>The domain.</summary>
    public string? DomainName { get; init; }

    /// <summary>The package name.</summary>
    public string? PackageName { get; init; }

    /// <summary>When the lease started.</summary>
    public DateTime LeaseOn { get; init; }

    /// <summary>When the lease ends.</summary>
    [JsonPropertyName("expiredOn")]
    public DateTime ExpiresOn { get; init; }

    /// <summary>The state.</summary>
    public string? Status { get; init; }

    /// <summary>The customer ID at the hosting platform.</summary>
    public string? ExternalCustomerId { get; init; }
}
