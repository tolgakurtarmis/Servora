using Servora.Domain.Models;

namespace Servora.Application.Abstractions;

public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<Subscription?> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken);
}
