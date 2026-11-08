using Servora.Domain.Abstractions;
using Servora.Domain.Entities;

namespace Servora.Domain.Models;

public sealed class OrderRound : Entity, ITenantEntity, IBranchEntity
{
    public Guid TenantId { get; set; }

    public Tenant Tenant { get; set; } = null!;

    public Guid BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public Guid TableSessionId { get; set; }

    public TableSession TableSession { get; set; } = null!;

    public List<OrderLine> Lines { get; set; } = [];
}
