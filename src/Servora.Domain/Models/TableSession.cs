using Servora.Domain.Abstractions;
using Servora.Domain.Entities;
using Servora.Domain.Enums;

namespace Servora.Domain.Models;

public sealed class TableSession : Entity, ITenantEntity, IBranchEntity
{
    public Guid TenantId { get; set; }

    public Guid BranchId { get; set; }

    public Guid TableId { get; set; }

    public Table Table { get; set; } = null!;

    public TableSessionStatus Status { get; set; }
}
