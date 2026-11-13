namespace Servora.Application.Analytics.GetSalesRangeSummary;

public sealed record SalesRangeDayResponse(string Date, decimal TotalRevenue, int OrderCount);

public sealed record SalesRangeTopProductResponse(string Name, int Quantity, decimal Revenue);

public sealed record GetSalesRangeSummaryResponse(
    string FromDate,
    string ToDate,
    string Currency,
    decimal TotalRevenue,
    int OrderCount,
    IReadOnlyList<SalesRangeTopProductResponse> TopProducts,
    SalesRangeDayResponse? WeakestDay,
    IReadOnlyList<SalesRangeDayResponse> Days,
    string Source);
