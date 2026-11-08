namespace Servora.Domain.Enums;

public enum Role
{
    /// <summary>Platform operator. Not tied to a tenant.</summary>
    PlatformAdmin = 1,

    /// <summary>Tenant owner. Can access the tenant's branches.</summary>
    Owner = 2,

    /// <summary>Branch manager.</summary>
    Manager = 3,

    /// <summary>Takes orders on the floor.</summary>
    Waiter = 4,

    /// <summary>Prepares items for a station.</summary>
    Kitchen = 5,

    /// <summary>Takes payment and closes the table.</summary>
    Cashier = 6
}
