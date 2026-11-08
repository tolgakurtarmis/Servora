namespace Servora.Domain.Enums;

public enum KitchenStatus
{
    /// <summary>Sent to the kitchen and not started.</summary>
    Pending = 1,

    /// <summary>The station is preparing the item.</summary>
    Preparing = 2,

    /// <summary>Ready to be served.</summary>
    Ready = 3,

    /// <summary>Delivered to the table.</summary>
    Served = 4
}
