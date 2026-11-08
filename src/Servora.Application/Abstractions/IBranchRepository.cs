using Servora.Domain.Models;

namespace Servora.Application.Abstractions;

public interface IBranchRepository : IRepository<Branch>
{
    Task<int> CountByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken);
}
