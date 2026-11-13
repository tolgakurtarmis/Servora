namespace Servora.Application.Analytics.GetDailySalesSummary;

public sealed record DailySalesTopProductResponse(string Name, int Quantity, decimal Revenue);

public sealed record GetDailySalesSummaryResponse(
    string Date,
    string Currency,
    decimal TotalRevenue,
    int OrderCount,
    IReadOnlyList<DailySalesTopProductResponse> TopProducts,
    string Source);
