using Microsoft.EntityFrameworkCore;
using Servora.Application.Abstractions;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence;

internal sealed class BranchRepository : EfRepository<Branch>, IBranchRepository
{
    private readonly ServoraDbContext _dbContext;

    public BranchRepository(ServoraDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> CountByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        return _dbContext.Branches.CountAsync(branch => branch.TenantId == tenantId, cancellationToken);
    }

    public async Task<IReadOnlyList<Branch>> ListByTenantIdAsync(
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Branches
            .AsNoTracking()
            .Where(branch => branch.TenantId == tenantId)
            .OrderBy(branch => branch.Name)
            .ToListAsync(cancellationToken);
    }
}
