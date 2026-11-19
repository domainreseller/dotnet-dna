namespace DomainNameApi.Contacts;

/// <summary>The answer of <see cref="GetContactVerificationRequest"/>.</summary>
public sealed record ContactVerificationResult
{
    /// <summary>One entry per verified field.</summary>
    public IReadOnlyList<ContactVerification> Results { get; init; } = [];
}
