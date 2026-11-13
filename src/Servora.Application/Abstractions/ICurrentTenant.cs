namespace Servora.Application.Abstractions;

public interface ICurrentTenant
{
    Guid? UserId { get; }

    Guid? TenantId { get; }

    Guid? BranchId { get; }

    string? Role { get; }

    bool IsPlatformAdmin { get; }
}
