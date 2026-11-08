using Servora.Domain.Abstractions;
using Servora.Domain.Entities;
using Servora.Domain.Enums;

namespace Servora.Domain.Models;

public sealed class OrderLine : Entity, ITenantEntity, IBranchEntity
{
    public Guid TenantId { get; set; }

    public Tenant Tenant { get; set; } = null!;

    public Guid BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public Guid OrderRoundId { get; set; }

    public OrderRound OrderRound { get; set; } = null!;

    public Guid MenuItemId { get; set; }

    public MenuItem MenuItem { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public KitchenStatus KitchenStatus { get; set; }
}
