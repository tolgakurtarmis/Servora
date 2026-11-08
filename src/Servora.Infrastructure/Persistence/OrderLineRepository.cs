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
        return LinesBySession(tableSessionId)
            .SumAsync(line => line.UnitPrice * line.Quantity, cancellationToken);
    }

    public async Task<IReadOnlyList<OrderLine>> ListBySessionIdAsync(
        Guid tableSessionId,
        CancellationToken cancellationToken)
    {
        return await LinesBySession(tableSessionId)
            .OrderBy(line => line.CreateDate)
            .ThenBy(line => line.Name)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<OrderLine> LinesBySession(Guid tableSessionId)
    {
        return _dbContext.OrderLines.Where(line => line.OrderRound.TableSessionId == tableSessionId);
    }
}
