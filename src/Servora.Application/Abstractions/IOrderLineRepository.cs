using Servora.Domain.Models;

namespace Servora.Application.Abstractions;

public interface IOrderLineRepository : IRepository<OrderLine>
{
    Task<decimal> SumBySessionIdAsync(Guid tableSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<OrderLine>> ListBySessionIdAsync(Guid tableSessionId, CancellationToken cancellationToken);
}
