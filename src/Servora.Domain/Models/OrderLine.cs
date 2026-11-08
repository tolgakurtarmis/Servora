using Servora.Domain.Abstractions;
using Servora.Domain.Entities;
using Servora.Domain.Enums;

namespace Servora.Domain.Models;

public sealed class OrderLine : Entity, ITenantEntity, IBranchEntity
{
    public Guid TenantId { get; set; }

    public Guid BranchId { get; set; }

    public Guid OrderRoundId { get; set; }

    public Guid MenuItemId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public KitchenStatus KitchenStatus { get; set; }
}
