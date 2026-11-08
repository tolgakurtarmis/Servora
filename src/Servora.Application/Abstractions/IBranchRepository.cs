using Servora.Domain.Models;

namespace Servora.Application.Abstractions;

public interface IBranchRepository : IRepository<Branch>
{
    Task<int> CountByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Branch>> ListByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken);
}
