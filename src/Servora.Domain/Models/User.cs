using Servora.Domain.Entities;
using Servora.Domain.Enums;

namespace Servora.Domain.Models;

public sealed class User : AuditedIdentityUser
{
    public string Name { get; set; } = string.Empty;

    public Role Role { get; set; }

    public Guid? TenantId { get; set; }
}
