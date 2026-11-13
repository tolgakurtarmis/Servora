using Servora.Application.Abstractions;

namespace Servora.Application.Analytics.GetDailySalesSummary;

public sealed record GetDailySalesSummaryQuery(Guid BranchId, DateOnly Date)
    : IQuery<GetDailySalesSummaryResponse>, IBranchRequest;
