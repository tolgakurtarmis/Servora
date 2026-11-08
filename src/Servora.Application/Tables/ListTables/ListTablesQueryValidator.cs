using FluentValidation;

namespace Servora.Application.Tables.ListTables;

public sealed class ListTablesQueryValidator : AbstractValidator<ListTablesQuery>
{
    public ListTablesQueryValidator()
    {
        RuleFor(query => query.TenantId)
            .NotEmpty();

        RuleFor(query => query.BranchId)
            .NotEmpty();
    }
}
