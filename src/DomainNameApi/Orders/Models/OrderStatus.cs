namespace DomainNameApi.Orders;

/// <summary>The state of an order.</summary>
public enum OrderStatus
{
    /// <summary>Completed and active.</summary>
    Active = 1,

    /// <summary>Waiting.</summary>
    Pending = 2,

    /// <summary>Cancelled.</summary>
    Canceled = 3,

    /// <summary>Being processed.</summary>
    InProgress = 4,
}

/// <summary>The state of an order line.</summary>
public enum OrderLineStatus
{
    /// <summary>Ordered.</summary>
    Ordered = 1,

    /// <summary>Cancelled.</summary>
    Canceled = 2,

    /// <summary>Being processed.</summary>
    InProgress = 3,
}
