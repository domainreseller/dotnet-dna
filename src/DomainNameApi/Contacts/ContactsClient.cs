namespace DomainNameApi.Contacts;

/// <summary>
/// Reusable contact handles (registrant, admin, tech, billing) and their verification.
/// Reach it through <see cref="DomainNameApiClient.Contacts"/>.
/// </summary>
public sealed class ContactsClient
{
    private readonly DomainNameApiClient _api;

    internal ContactsClient(DomainNameApiClient api) => _api = api;

    /// <summary>Returns a contact handle and the state of its verification.</summary>
    /// <param name="handle">The contact handle, for example <c>DNA-12345</c>.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<ContactDetails> GetAsync(string handle, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new GetContactRequest(handle), cancellationToken);

    /// <summary>Creates a reusable contact handle.</summary>
    /// <param name="contact">The contact details.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The result, with the new handle in <see cref="CreateContactResult.Handle"/>.</returns>
    public Task<CreateContactResult> CreateAsync(ContactInfo contact, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new CreateContactRequest(contact), cancellationToken);

    /// <summary>Updates a contact handle and the domains that use it.</summary>
    /// <param name="handle">The contact handle.</param>
    /// <param name="contact">The new details.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<HandleUpdateResult> UpdateAsync(string handle, ContactInfo contact, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new UpdateContactRequest(handle, contact), cancellationToken);

    /// <summary>Returns the e-mail and phone verification state of a contact.</summary>
    /// <param name="handle">The contact handle.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public async Task<IReadOnlyList<ContactVerification>> GetVerificationAsync(string handle, CancellationToken cancellationToken = default) =>
        (await _api.SendAsync(new GetContactVerificationRequest(handle), cancellationToken).ConfigureAwait(false)).Results;

    /// <summary>Sends the contact verification notice again.</summary>
    /// <param name="handle">The contact handle.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task ResendVerificationAsync(string handle, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new ResendContactVerificationRequest(handle), cancellationToken);
}
