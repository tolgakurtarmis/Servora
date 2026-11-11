using FluentValidation;

namespace Servora.Application.Ordering.ListOpenTables;

public sealed class ListOpenTablesQueryValidator : AbstractValidator<ListOpenTablesQuery>
{
    public ListOpenTablesQueryValidator()
    {

        RuleFor(query => query.BranchId)
            .NotEmpty();
    }
}
