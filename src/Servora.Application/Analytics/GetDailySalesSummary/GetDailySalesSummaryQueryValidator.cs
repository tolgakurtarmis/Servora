using FluentValidation;

namespace Servora.Application.Analytics.GetDailySalesSummary;

public sealed class GetDailySalesSummaryQueryValidator : AbstractValidator<GetDailySalesSummaryQuery>
{
    public GetDailySalesSummaryQueryValidator()
    {
        RuleFor(query => query.BranchId)
            .NotEmpty();

        RuleFor(query => query.Date)
            .Must(date => date != default)
            .WithMessage("Date is required.");
    }
}
