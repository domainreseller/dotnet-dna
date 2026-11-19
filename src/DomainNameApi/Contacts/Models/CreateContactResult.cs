namespace DomainNameApi.Contacts;

/// <summary>The result of creating a contact.</summary>
public sealed record CreateContactResult
{
    /// <summary>The new contact handle. Use it to register domains or change their contacts.</summary>
    public string? Handle { get; init; }

    /// <summary>The API's message about the operation.</summary>
    public string? OperationMessage { get; init; }

    /// <summary>Whether the contact was created.</summary>
    public bool IsSuccess { get; init; }
}
