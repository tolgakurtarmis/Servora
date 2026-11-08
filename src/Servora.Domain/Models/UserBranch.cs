using Servora.Domain.Abstractions;
using Servora.Domain.Entities;

namespace Servora.Domain.Models;

public sealed class UserBranch : Entity, ITenantEntity, IBranchEntity
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid TenantId { get; set; }

    public Tenant Tenant { get; set; } = null!;

    public Guid BranchId { get; set; }

    public Branch Branch { get; set; } = null!;
}
