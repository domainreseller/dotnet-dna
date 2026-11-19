namespace DomainNameApi.Contacts;

/// <summary>The verification state of one contact field (an e-mail address or a phone number).</summary>
public sealed record ContactVerification
{
    /// <summary>The contact's name.</summary>
    public string? OwnerName { get; init; }

    /// <summary>The verified value, for example the e-mail address.</summary>
    public string? FieldValue { get; init; }

    /// <summary>What is verified, for example <c>Email</c>.</summary>
    public string? FieldType { get; init; }

    /// <summary>When the verification link expires.</summary>
    public DateTime TokenExpireDate { get; init; }

    /// <summary>When the notice was last sent again.</summary>
    public DateTime ResendDate { get; init; }

    /// <summary>How often the notice was sent again.</summary>
    public int ResendRetryCount { get; init; }

    /// <summary>When the contact last tried to verify.</summary>
    public DateTime LastAttemptDate { get; init; }

    /// <summary>How often the contact tried to verify.</summary>
    public int AttemptNumber { get; init; }

    /// <summary>The state of the verification process.</summary>
    public string? OperationStatus { get; init; }

    /// <summary>When the verification was started.</summary>
    public DateTime CreatedDate { get; init; }

    /// <summary>When the contact verified, if they did.</summary>
    public DateTime? UserVerificationDate { get; init; }

    /// <summary>Whether the contact has verified.</summary>
    public bool IsUserVerified { get; init; }
}
