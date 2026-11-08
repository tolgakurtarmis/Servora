namespace Servora.Application.Abstractions;

public interface ICurrentTenant
{
    Guid? TenantId { get; }

    Guid? BranchId { get; }

    bool IsPlatformAdmin { get; }
}
