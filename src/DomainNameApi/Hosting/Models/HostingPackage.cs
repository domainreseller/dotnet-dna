namespace DomainNameApi.Hosting;

/// <summary>A hosting package and the service plans you can order it with.</summary>
public sealed record HostingPackage
{
    /// <summary>The package name.</summary>
    public string? Name { get; init; }

    /// <summary>The shortest lease, in months.</summary>
    public int RequiredMinLeasing { get; init; }

    /// <summary>The longest lease, in months.</summary>
    public int PermittedMaxLeasing { get; init; }

    /// <summary>The service plans of the package.</summary>
    public IReadOnlyList<HostingPackagePlan> ServicePlans { get; init; } = [];
}

/// <summary>A service plan of a hosting package.</summary>
public sealed record HostingPackagePlan
{
    /// <summary>The plan name, used when ordering.</summary>
    public string? ServicePlan { get; init; }

    /// <summary>The operating system.</summary>
    public string? OperatingSystem { get; init; }

    /// <summary>The control panel.</summary>
    public string? PanelType { get; init; }
}
