namespace DomainNameApi.Hosting;

/// <summary><c>GET hostings/packages</c>: the hosting packages and their service plans.</summary>
public sealed class ListHostingPackagesRequest : BaseRequest<IReadOnlyList<HostingPackage>>
{
    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "hostings/packages";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;
}
