namespace DomainNameApi.Hosting;

/// <summary>The result of ordering a hosting. Provisioning continues in the background; poll its status.</summary>
public sealed record HostingCreateResult
{
    /// <summary>The service ID.</summary>
    public string? ServiceId { get; init; }

    /// <summary>The state.</summary>
    public string? Status { get; init; }

    /// <summary>The control panel address, once ready.</summary>
    public string? PanelAddress { get; init; }

    /// <summary>The control panel user name, once ready.</summary>
    public string? Username { get; init; }

    /// <summary>The control panel password, once ready.</summary>
    public string? Password { get; init; }

    /// <summary>A token to look the hosting up with <see cref="HostingClient.GetByTokenAsync"/>.</summary>
    public string? Token { get; init; }
}
