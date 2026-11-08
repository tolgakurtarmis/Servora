using Microsoft.EntityFrameworkCore;
using Servora.Application.Abstractions;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence;

internal sealed class PaymentRepository : EfRepository<Payment>, IPaymentRepository
{
    private readonly ServoraDbContext _dbContext;

    public PaymentRepository(ServoraDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<decimal> SumBySessionIdAsync(Guid tableSessionId, CancellationToken cancellationToken)
    {
        return _dbContext.Payments
            .Where(payment => payment.TableSessionId == tableSessionId)
            .SumAsync(payment => payment.Amount, cancellationToken);
    }

    public Task<bool> AnyBySessionIdAsync(Guid tableSessionId, CancellationToken cancellationToken)
    {
        return _dbContext.Payments.AnyAsync(
            payment => payment.TableSessionId == tableSessionId,
            cancellationToken);
    }
}
