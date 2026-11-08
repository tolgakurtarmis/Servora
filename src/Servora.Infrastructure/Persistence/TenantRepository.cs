using Microsoft.EntityFrameworkCore;
using Servora.Application.Abstractions;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence;

internal sealed class TenantRepository : EfRepository<Tenant>, ITenantRepository
{
    private readonly ServoraDbContext _dbContext;

    public TenantRepository(ServoraDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Tenant>> ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Tenants
            .AsNoTracking()
            .OrderBy(tenant => tenant.Name)
            .ToListAsync(cancellationToken);
    }
}
