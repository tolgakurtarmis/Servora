using Servora.Domain.Models;

namespace Servora.Application.Abstractions;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<IReadOnlyList<Tenant>> ListAsync(CancellationToken cancellationToken);
}
