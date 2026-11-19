namespace DomainNameApi.Hosting;

/// <summary>The provisioning state of a hosting, and the panel access once it is ready.</summary>
public sealed record HostingStatus
{
    /// <summary>The service ID.</summary>
    public string? ServiceId { get; init; }

    /// <summary>The domain.</summary>
    public string? DomainName { get; init; }

    /// <summary>The state.</summary>
    public string? Status { get; init; }

    /// <summary>A description of the state.</summary>
    public string? StatusMessage { get; init; }

    /// <summary>The control panel address.</summary>
    public string? PanelAddress { get; init; }

    /// <summary>The control panel user name.</summary>
    public string? Username { get; init; }

    /// <summary>The control panel password.</summary>
    public string? Password { get; init; }

    /// <summary>The lookup token.</summary>
    public string? Token { get; init; }

    /// <summary>Why provisioning failed, if it did.</summary>
    public string? FailureReason { get; init; }
}
