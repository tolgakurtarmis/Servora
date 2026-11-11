using FluentValidation;

namespace Servora.Application.Ordering.GetTableSession;

public sealed class GetTableSessionQueryValidator : AbstractValidator<GetTableSessionQuery>
{
    public GetTableSessionQueryValidator()
    {

        RuleFor(query => query.BranchId)
            .NotEmpty();

        RuleFor(query => query.TableSessionId)
            .NotEmpty();
    }
}
