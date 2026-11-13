using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Application.Abstractions;

public interface IOrderLineRepository : IRepository<OrderLine>
{
    Task<decimal> SumBySessionIdAsync(Guid tableSessionId, CancellationToken cancellationToken);

    Task<bool> AnyBySessionIdAsync(Guid tableSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<OrderLine>> ListBySessionIdAsync(Guid tableSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<KitchenQueueItemInfo>> ListKitchenQueueByBranchIdAsync(
        Guid branchId,
        CancellationToken cancellationToken);

    Task<DailySalesSummaryInfo> GetDailySalesSummaryAsync(
        Guid branchId,
        DateOnly date,
        CancellationToken cancellationToken);

    Task<SalesRangeSummaryInfo> GetSalesRangeSummaryAsync(
        Guid branchId,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken);
}

public sealed record DailySalesTopProductInfo(string Name, int Quantity, decimal Revenue);

public sealed record DailySalesSummaryInfo(
    DateOnly Date,
    decimal TotalRevenue,
    int OrderCount,
    IReadOnlyList<DailySalesTopProductInfo> TopProducts);

public sealed record SalesRangeDayInfo(DateOnly Date, decimal TotalRevenue, int OrderCount);

public sealed record SalesRangeSummaryInfo(
    DateOnly FromDate,
    DateOnly ToDate,
    decimal TotalRevenue,
    int OrderCount,
    IReadOnlyList<DailySalesTopProductInfo> TopProducts,
    SalesRangeDayInfo? WeakestDay,
    IReadOnlyList<SalesRangeDayInfo> Days);

public sealed record KitchenQueueItemInfo(
    Guid OrderLineId,
    Guid TableId,
    string TableName,
    Guid SessionId,
    string Name,
    string Note,
    int Quantity,
    KitchenStatus KitchenStatus,
    DateTime OrderedAt);
