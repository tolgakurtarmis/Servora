using FluentValidation;

namespace Servora.Application.Analytics.GetSalesRangeSummary;

public sealed class GetSalesRangeSummaryQueryValidator : AbstractValidator<GetSalesRangeSummaryQuery>
{
    public GetSalesRangeSummaryQueryValidator()
    {
        RuleFor(query => query.BranchId)
            .NotEmpty();

        RuleFor(query => query.FromDate)
            .Must(date => date != default)
            .WithMessage("FromDate is required.");

        RuleFor(query => query.ToDate)
            .Must(date => date != default)
            .WithMessage("ToDate is required.")
            .GreaterThanOrEqualTo(query => query.FromDate)
            .WithMessage("ToDate must be on or after FromDate.");

        RuleFor(query => query)
            .Must(query => query.ToDate.DayNumber - query.FromDate.DayNumber <= 31)
            .WithMessage("Date range cannot exceed 31 days.");
    }
}
