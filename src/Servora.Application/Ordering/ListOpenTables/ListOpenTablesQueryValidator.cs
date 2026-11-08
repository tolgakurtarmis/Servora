using FluentValidation;

namespace Servora.Application.Ordering.ListOpenTables;

public sealed class ListOpenTablesQueryValidator : AbstractValidator<ListOpenTablesQuery>
{
    public ListOpenTablesQueryValidator()
    {
        RuleFor(query => query.TenantId)
            .NotEmpty();

        RuleFor(query => query.BranchId)
            .NotEmpty();
    }
}
