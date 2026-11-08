using Microsoft.EntityFrameworkCore;
using Servora.Application.Abstractions;
using Servora.Domain.Enums;
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

    public async Task<IReadOnlyList<KitchenQueueItemInfo>> ListKitchenQueueByBranchIdAsync(
        Guid branchId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.OrderLines
            .AsNoTracking()
            .Where(line =>
                line.BranchId == branchId &&
                line.OrderRound.TableSession.Status == TableSessionStatus.Open &&
                (line.KitchenStatus == KitchenStatus.Pending || line.KitchenStatus == KitchenStatus.Preparing))
            .OrderBy(line => line.CreateDate)
            .ThenBy(line => line.Name)
            .Select(line => new KitchenQueueItemInfo(
                line.Id,
                line.OrderRound.TableSession.Table.Id,
                line.OrderRound.TableSession.Table.Name,
                line.OrderRound.TableSession.Id,
                line.Name,
                line.Quantity,
                line.KitchenStatus,
                line.CreateDate))
            .ToListAsync(cancellationToken);
    }

    private IQueryable<OrderLine> LinesBySession(Guid tableSessionId)
    {
        return _dbContext.OrderLines.Where(line => line.OrderRound.TableSessionId == tableSessionId);
    }
}
