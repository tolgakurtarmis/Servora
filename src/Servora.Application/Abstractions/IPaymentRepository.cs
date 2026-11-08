using Servora.Domain.Models;

namespace Servora.Application.Abstractions;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<decimal> SumBySessionIdAsync(Guid tableSessionId, CancellationToken cancellationToken);

    Task<bool> AnyBySessionIdAsync(Guid tableSessionId, CancellationToken cancellationToken);
}
