using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Analytics.GetDailySalesSummary;

public sealed class GetDailySalesSummaryQueryHandler(
    ICachedBranchReader branches,
    IOrderLineRepository lines)
        : IRequestHandler<GetDailySalesSummaryQuery, GetDailySalesSummaryResponse>
{
    public async Task<GetDailySalesSummaryResponse> Handle(
        GetDailySalesSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var branch = await branches.GetAsync(request.BranchId, cancellationToken);
        if (branch is null)
        {
            throw new NotFoundException("Branch was not found.");
        }

        var summary = await lines.GetDailySalesSummaryAsync(branch.Id, request.Date, cancellationToken);

        return new GetDailySalesSummaryResponse(
            summary.Date.ToString("yyyy-MM-dd"),
            "TRY",
            summary.TotalRevenue,
            summary.OrderCount,
            [.. summary.TopProducts.Select(product =>
                new DailySalesTopProductResponse(product.Name, product.Quantity, product.Revenue))],
            "servora-api");
    }
}
