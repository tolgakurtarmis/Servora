using Microsoft.EntityFrameworkCore;
using Servora.Application.Abstractions;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence;

internal sealed class SubscriptionRepository : EfRepository<Subscription>, ISubscriptionRepository
{
    private readonly ServoraDbContext _dbContext;

    public SubscriptionRepository(ServoraDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Subscription?> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        return _dbContext.Subscriptions.FirstOrDefaultAsync(
            subscription => subscription.TenantId == tenantId,
            cancellationToken);
    }
}
