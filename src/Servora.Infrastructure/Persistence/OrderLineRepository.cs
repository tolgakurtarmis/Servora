using Microsoft.EntityFrameworkCore;
using Servora.Application.Abstractions;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence;

internal sealed class OrderLineRepository : EfRepository<OrderLine>, IOrderLineRepository
{
    private readonly ServoraDbContext _dbContext;

    public OrderLineRepository(ServoraDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<decimal> SumBySessionIdAsync(Guid tableSessionId, CancellationToken cancellationToken)
    {
        return _dbContext.OrderLines
            .Where(line => _dbContext.OrderRounds.Any(round =>
                round.Id == line.OrderRoundId && round.TableSessionId == tableSessionId))
            .SumAsync(line => line.UnitPrice * line.Quantity, cancellationToken);
    }
}
