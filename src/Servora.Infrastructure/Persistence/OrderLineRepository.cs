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

    public Task<bool> AnyBySessionIdAsync(Guid tableSessionId, CancellationToken cancellationToken)
    {
        return LinesBySession(tableSessionId).AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrderLine>> ListBySessionIdAsync(
        Guid tableSessionId,
        CancellationToken cancellationToken)
    {
        return await LinesBySession(tableSessionId)
            .AsNoTracking()
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
                line.Note,
                line.Quantity,
                line.KitchenStatus,
                line.CreateDate))
            .ToListAsync(cancellationToken);
    }

    public async Task<SalesRangeSummaryInfo> GetSalesRangeSummaryAsync(
        Guid branchId,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken)
    {
        if (toDate < fromDate)
        {
            (fromDate, toDate) = (toDate, fromDate);
        }

        var from = fromDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toExclusive = toDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddDays(1);

        var lines = await _dbContext.OrderLines
            .AsNoTracking()
            .Where(line =>
                line.BranchId == branchId &&
                line.CreateDate >= from &&
                line.CreateDate < toExclusive &&
                line.OrderRound.TableSession.Status != TableSessionStatus.Cancelled)
            .Select(line => new
            {
                line.Name,
                line.Quantity,
                Revenue = line.UnitPrice * line.Quantity,
                SessionId = line.OrderRound.TableSessionId,
                line.CreateDate
            })
            .ToListAsync(cancellationToken);

        var dayMap = lines
            .GroupBy(line => DateOnly.FromDateTime(DateTime.SpecifyKind(line.CreateDate, DateTimeKind.Utc)))
            .ToDictionary(
                group => group.Key,
                group => new SalesRangeDayInfo(
                    group.Key,
                    group.Sum(line => line.Revenue),
                    group.Select(line => line.SessionId).Distinct().Count()));

        var days = new List<SalesRangeDayInfo>();
        for (var day = fromDate; day <= toDate; day = day.AddDays(1))
        {
            days.Add(dayMap.TryGetValue(day, out var info)
                ? info
                : new SalesRangeDayInfo(day, 0m, 0));
        }

        var topProducts = lines
            .GroupBy(line => line.Name)
            .Select(group => new DailySalesTopProductInfo(
                group.Key,
                group.Sum(line => line.Quantity),
                group.Sum(line => line.Revenue)))
            .OrderByDescending(product => product.Revenue)
            .ThenBy(product => product.Name)
            .Take(5)
            .ToList();

        var weakestDay = days
            .OrderBy(day => day.TotalRevenue)
            .ThenBy(day => day.Date)
            .FirstOrDefault();

        return new SalesRangeSummaryInfo(
            fromDate,
            toDate,
            lines.Sum(line => line.Revenue),
            lines.Select(line => line.SessionId).Distinct().Count(),
            topProducts,
            weakestDay,
            days);
    }

    private IQueryable<OrderLine> LinesBySession(Guid tableSessionId)
    {
        return _dbContext.OrderLines.Where(line => line.OrderRound.TableSessionId == tableSessionId);
    }
}
