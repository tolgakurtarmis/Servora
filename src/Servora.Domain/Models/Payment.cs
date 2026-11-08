using Servora.Domain.Abstractions;
using Servora.Domain.Entities;
using Servora.Domain.Enums;

namespace Servora.Domain.Models;

public sealed class Payment : Entity, ITenantEntity, IBranchEntity
{
    public Guid TenantId { get; set; }

    public Guid BranchId { get; set; }

    public Guid TableSessionId { get; set; }

    public decimal Amount { get; set; }

    public PaymentMethod Method { get; set; }
}
