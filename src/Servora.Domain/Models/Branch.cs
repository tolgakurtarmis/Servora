using Servora.Domain.Abstractions;
using Servora.Domain.Entities;

namespace Servora.Domain.Models;

public sealed class Branch : Entity, ITenantEntity
{
    public Guid TenantId { get; set; }

    public Tenant Tenant { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public bool UsesKitchen { get; set; } = true;
}
