using Servora.Domain.Abstractions;
using Servora.Domain.Entities;
using Servora.Domain.Enums;

namespace Servora.Domain.Models;

public sealed class TableSession : Entity, ITenantEntity, IBranchEntity
{
    public Guid TenantId { get; set; }

    public Tenant Tenant { get; set; } = null!;

    public Guid BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public Guid TableId { get; set; }

    public Table Table { get; set; } = null!;

    public TableSessionStatus Status { get; set; }
}
