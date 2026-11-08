namespace Servora.Domain.Enums;

public enum SubscriptionStatus
{
    /// <summary>Trial period before paid subscription.</summary>
    Trial = 1,

    /// <summary>Subscription is active and paid.</summary>
    Active = 2,

    /// <summary>Payment overdue; new sessions blocked.</summary>
    PastDue = 3,

    /// <summary>Tenant operations are suspended.</summary>
    Suspended = 4
}
