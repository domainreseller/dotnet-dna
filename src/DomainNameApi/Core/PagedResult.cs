namespace DomainNameApi;

/// <summary>One page of a list.</summary>
/// <typeparam name="T">The type of the items.</typeparam>
public sealed record PagedResult<T>
{
    /// <summary>The items on this page.</summary>
    public IReadOnlyList<T> Items { get; init; } = [];

    /// <summary>The number of items across all pages.</summary>
    public long TotalCount { get; init; }
}

/// <summary>A list returned in one piece.</summary>
/// <typeparam name="T">The type of the items.</typeparam>
public sealed record ListResult<T>
{
    /// <summary>The items.</summary>
    public IReadOnlyList<T> Items { get; init; } = [];
}
