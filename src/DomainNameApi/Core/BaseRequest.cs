using System.Text.Json.Serialization;

namespace DomainNameApi;

/// <summary>
/// Base class of every API request. A request knows its HTTP method, its path, what it can do
/// to your account (<see cref="Safety"/>), its query string and its JSON body.
/// </summary>
/// <remarks>
/// <para>
/// Derive from <see cref="BaseRequest{TResponse}"/> when the operation returns data and from this
/// class when it returns nothing. Send any request with
/// <see cref="DomainNameApiClient.SendAsync(BaseRequest, CancellationToken)"/>.
/// </para>
/// <para>
/// For POST and PUT requests the request object itself is the JSON body: its public properties are
/// written in camelCase and <c>null</c> values are left out. Properties that belong to the query
/// string get <see cref="JsonIgnoreAttribute"/> and are added in <see cref="AddQueryParameters"/>.
/// GET and DELETE requests have no body, so their properties only go where you put them.
/// </para>
/// <example>
/// A request for an endpoint the SDK does not cover yet:
/// <code>
/// public sealed class GetDomainNotesRequest : BaseRequest&lt;DomainNotes&gt;
/// {
///     public GetDomainNotesRequest(string domainName) =&gt; DomainName = domainName;
///
///     public string DomainName { get; }
///
///     public override HttpMethod Method =&gt; HttpMethod.Get;
///     public override string Path =&gt; "domains/notes";
///     public override OperationSafety Safety =&gt; OperationSafety.Read;
///
///     protected override void AddQueryParameters(QueryParameters query) =&gt; query.Add("DomainName", DomainName);
/// }
/// </code>
/// </example>
/// </remarks>
public abstract class BaseRequest
{
    /// <summary>The HTTP method.</summary>
    [JsonIgnore]
    public abstract HttpMethod Method { get; }

    /// <summary>
    /// The path below <c>/api/v1/</c> with path parameters filled in, for example <c>domains/info</c>
    /// or <c>contacts/DNA-12345</c>.
    /// </summary>
    [JsonIgnore]
    public abstract string Path { get; }

    /// <summary>What the operation can do to your account.</summary>
    [JsonIgnore]
    public abstract OperationSafety Safety { get; }

    /// <summary>
    /// Whether the request may be sent again after a network error or a 502/503/504 answer.
    /// True for reads. Billable requests are never repeated, so nothing is charged twice.
    /// </summary>
    [JsonIgnore]
    public virtual bool IsIdempotent => Safety == OperationSafety.Read;

    /// <summary>Adds the request's query string parameters. Does nothing by default.</summary>
    /// <param name="query">The query string being built.</param>
    protected internal virtual void AddQueryParameters(QueryParameters query)
    {
    }

    /// <summary>
    /// Returns the object written as the JSON body: the request itself for POST and PUT, nothing for
    /// GET and DELETE. Override it when the API expects a different shape, such as an array.
    /// </summary>
    protected internal virtual object? GetBody() =>
        Method == HttpMethod.Post || Method == HttpMethod.Put ? this : null;

    /// <summary>
    /// Checks the request before it is sent. Throw an <see cref="ArgumentException"/> for missing or
    /// invalid values; nothing is sent in that case.
    /// </summary>
    protected internal virtual void Validate()
    {
    }

    /// <summary>Throws an <see cref="ArgumentException"/> when <paramref name="value"/> is null or blank.</summary>
    /// <param name="value">The value to check.</param>
    /// <param name="name">The property name used in the error message.</param>
    protected static void Require(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(name + " is required.", name);
        }
    }

    /// <summary>Throws an <see cref="ArgumentException"/> when <paramref name="values"/> is null or empty.</summary>
    /// <param name="values">The values to check.</param>
    /// <param name="name">The property name used in the error message.</param>
    protected static void Require<T>(IReadOnlyCollection<T>? values, string name)
    {
        if (values is null || values.Count == 0)
        {
            throw new ArgumentException(name + " needs at least one value.", name);
        }
    }

    /// <summary>Throws an <see cref="ArgumentException"/> when <paramref name="value"/> is null.</summary>
    /// <param name="value">The value to check.</param>
    /// <param name="name">The property name used in the error message.</param>
    protected static void Require(object? value, string name)
    {
        if (value is null)
        {
            throw new ArgumentException(name + " is required.", name);
        }
    }

    /// <summary>Returns <c>METHOD path</c>, for example <c>POST domains/renew</c>.</summary>
    public override string ToString() => Method.Method + " " + Path;
}

/// <summary>A request whose successful answer is read as <typeparamref name="TResponse"/>.</summary>
/// <typeparam name="TResponse">The type of the response body.</typeparam>
public abstract class BaseRequest<TResponse> : BaseRequest
{
}
