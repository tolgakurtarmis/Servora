using Servora.Domain.Abstractions;
using Servora.Domain.Entities;
using Servora.Domain.Enums;

namespace Servora.Domain.Models;

public sealed class Subscription : Entity, ITenantEntity
{
    public Guid TenantId { get; set; }

    public SubscriptionStatus Status { get; set; }

    public int MaxBranches { get; set; }

    public int MaxUsers { get; set; }
}
