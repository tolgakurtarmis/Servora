using Servora.Domain.Abstractions;
using Servora.Domain.Entities;

namespace Servora.Domain.Models;

public sealed class MenuItem : Entity, ITenantEntity, IBranchEntity
{
    public Guid TenantId { get; set; }

    public Tenant Tenant { get; set; } = null!;

    public Guid BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public Guid CategoryId { get; set; }

    public MenuCategory Category { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }
}
