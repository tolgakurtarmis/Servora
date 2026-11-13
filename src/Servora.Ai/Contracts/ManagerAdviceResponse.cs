namespace Servora.Ai.Contracts;

public sealed record ManagerAdviceResponse(
    string Advice,
    string Model,
    ManagerAdviceFacts Facts);

public sealed record ManagerAdviceFacts(
    string FromDate,
    string ToDate,
    decimal TotalRevenue,
    int OrderCount,
    decimal AverageDailyRevenue,
    string? TopProductName,
    decimal? TopProductSharePercent,
    string? WeakestDay,
    decimal? WeakestDayRevenue);
