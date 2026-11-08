using Servora.Domain.Abstractions;
using Servora.Domain.Entities;
using Servora.Domain.Enums;

namespace Servora.Domain.Models;

public sealed class Payment : Entity, ITenantEntity, IBranchEntity
{
    public Guid TenantId { get; set; }

    public Tenant Tenant { get; set; } = null!;

    public Guid BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public Guid TableSessionId { get; set; }

    public TableSession TableSession { get; set; } = null!;

    public decimal Amount { get; set; }

    public PaymentMethod Method { get; set; }
}
