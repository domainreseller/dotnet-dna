namespace DomainNameApi;

/// <summary>
/// Base class of list requests that return one page at a time. Use <see cref="SkipCount"/> and
/// <see cref="MaxResultCount"/> to choose the page, or let the client page for you with the
/// <c>ListAllAsync</c> methods.
/// </summary>
/// <typeparam name="TItem">The type of the listed items.</typeparam>
public abstract class PagedRequest<TItem> : BaseRequest<PagedResult<TItem>>
{
    /// <summary>The page size the <c>ListAllAsync</c> methods use when <see cref="MaxResultCount"/> is not set.</summary>
    public const int DefaultPageSize = 100;

    /// <summary>How many items to skip. The API starts at 0.</summary>
    public int? SkipCount { get; set; }

    /// <summary>Page size. The API uses 10 when it is not set.</summary>
    public int? MaxResultCount { get; set; }

    /// <summary>Sort expression, for example <c>expirationDate desc</c>.</summary>
    public string? Sorting { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query)
    {
        query.Add("Sorting", Sorting)
             .Add("SkipCount", SkipCount)
             .Add("MaxResultCount", MaxResultCount);
    }

    /// <inheritdoc />
    protected internal override void Validate()
    {
        if (SkipCount < 0)
        {
            throw new ArgumentException("SkipCount cannot be negative.", nameof(SkipCount));
        }
        if (MaxResultCount < 1)
        {
            throw new ArgumentException("MaxResultCount must be at least 1.", nameof(MaxResultCount));
        }
    }

    /// <summary>Returns a copy of this request for another page; the original is not changed.</summary>
    internal PagedRequest<TItem> ForPage(int skipCount, int maxResultCount)
    {
        var copy = (PagedRequest<TItem>)MemberwiseClone();
        copy.SkipCount = skipCount;
        copy.MaxResultCount = maxResultCount;
        return copy;
    }
}
