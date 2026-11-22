namespace DomainNameApi.Ssl;

/// <summary>
/// <c>GET ssls</c>: pages through your certificates, newest first. Use it to find a certificate's ID.
/// The API does not sort this list on request, so <see cref="PagedRequest{TItem}.Sorting"/> must stay unset.
/// </summary>
public sealed class ListCertificatesRequest : PagedRequest<CertificateListItem>
{
    /// <summary>
    /// Only certificates of this domain. For wildcard certificates both <c>*.example.com</c> and <c>example.com</c> match.
    /// </summary>
    public string? DomainName { get; set; }

    /// <inheritdoc />
    public override string Path => "ssls";

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) =>
        query.Add("DomainName", DomainName)
             .Add("SkipCount", SkipCount)
             .Add("MaxResultCount", MaxResultCount);

    /// <inheritdoc />
    protected internal override void Validate()
    {
        base.Validate();
        if (!string.IsNullOrWhiteSpace(Sorting))
        {
            throw new ArgumentException("The certificate list cannot be sorted; it is always newest first. Leave Sorting unset.", nameof(Sorting));
        }
    }
}
