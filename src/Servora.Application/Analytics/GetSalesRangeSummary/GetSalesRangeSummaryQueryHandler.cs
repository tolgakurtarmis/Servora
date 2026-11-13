using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Analytics.GetSalesRangeSummary;

public sealed class GetSalesRangeSummaryQueryHandler(
    ICachedBranchReader branches,
    IOrderLineRepository lines)
        : IRequestHandler<GetSalesRangeSummaryQuery, GetSalesRangeSummaryResponse>
{
    public async Task<GetSalesRangeSummaryResponse> Handle(
        GetSalesRangeSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var branch = await branches.GetAsync(request.BranchId, cancellationToken);
        if (branch is null)
        {
            throw new NotFoundException("Branch was not found.");
        }

        var summary = await lines.GetSalesRangeSummaryAsync(
            branch.Id,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        return new GetSalesRangeSummaryResponse(
            summary.FromDate.ToString("yyyy-MM-dd"),
            summary.ToDate.ToString("yyyy-MM-dd"),
            "TRY",
            summary.TotalRevenue,
            summary.OrderCount,
            [.. summary.TopProducts.Select(product =>
                new SalesRangeTopProductResponse(product.Name, product.Quantity, product.Revenue))],
            summary.WeakestDay is null
                ? null
                : new SalesRangeDayResponse(
                    summary.WeakestDay.Date.ToString("yyyy-MM-dd"),
                    summary.WeakestDay.TotalRevenue,
                    summary.WeakestDay.OrderCount),
            [.. summary.Days.Select(day =>
                new SalesRangeDayResponse(
                    day.Date.ToString("yyyy-MM-dd"),
                    day.TotalRevenue,
                    day.OrderCount))],
            "servora-api");
    }
}
