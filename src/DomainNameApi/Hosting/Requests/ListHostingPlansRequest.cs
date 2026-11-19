namespace DomainNameApi.Hosting;

/// <summary><c>GET hostings/plans</c>: the hosting service plans and their limits.</summary>
public sealed class ListHostingPlansRequest : BaseRequest<IReadOnlyList<ServicePlan>>
{
    /// <summary>Only the plans available to this hosting.</summary>
    public Guid? HostingId { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "hostings/plans";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) => query.Add("hostingId", HostingId);
}
