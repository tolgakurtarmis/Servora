using Servora.Domain.Abstractions;
using Servora.Domain.Entities;

namespace Servora.Domain.Models;

public sealed class MenuCategory : Entity, ITenantEntity, IBranchEntity
{
    public Guid TenantId { get; set; }

    public Tenant Tenant { get; set; } = null!;

    public Guid BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public List<MenuItem> Items { get; set; } = [];
}
