using Servora.Domain.Abstractions;
using Servora.Domain.Entities;

namespace Servora.Domain.Models;

public sealed class UserBranch : Entity, ITenantEntity, IBranchEntity
{
    public Guid UserId { get; set; }

    public Guid TenantId { get; set; }

    public Guid BranchId { get; set; }
}
