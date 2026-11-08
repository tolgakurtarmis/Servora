using Servora.Domain.Abstractions;
using Servora.Domain.Entities;

namespace Servora.Domain.Models;

public sealed class OrderRound : Entity, ITenantEntity, IBranchEntity
{
    public Guid TenantId { get; set; }

    public Guid BranchId { get; set; }

    public Guid TableSessionId { get; set; }

    public List<OrderLine> Lines { get; set; } = [];
}
