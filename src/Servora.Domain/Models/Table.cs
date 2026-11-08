using Servora.Domain.Abstractions;
using Servora.Domain.Entities;

namespace Servora.Domain.Models;

public sealed class Table : Entity, ITenantEntity, IBranchEntity
{
    public Guid TenantId { get; set; }

    public Guid BranchId { get; set; }

    public string Name { get; set; } = string.Empty;
}
