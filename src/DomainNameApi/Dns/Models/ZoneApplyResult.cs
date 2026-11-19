namespace DomainNameApi.Dns;

/// <summary>The result of switching a domain to the platform nameservers.</summary>
public sealed record ZoneApplyResult
{
    /// <summary>Whether the nameservers had to be changed.</summary>
    public bool NameServersChanged { get; init; }

    /// <summary>The nameservers before.</summary>
    public IReadOnlyList<string> PreviousNameServers { get; init; } = [];

    /// <summary>The nameservers now.</summary>
    public IReadOnlyList<string> CurrentNameServers { get; init; } = [];

    /// <summary>The platform nameservers that serve the zone.</summary>
    public IReadOnlyList<string> SystemNameServers { get; init; } = [];
}
