using Servora.Application.Abstractions;

namespace Servora.Application.Analytics.GetSalesRangeSummary;

public sealed record GetSalesRangeSummaryQuery(Guid BranchId, DateOnly FromDate, DateOnly ToDate)
    : IQuery<GetSalesRangeSummaryResponse>, IBranchRequest;
