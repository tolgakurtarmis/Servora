using FluentValidation;

namespace Servora.Application.Catalog.ListMenuItems;

public sealed class ListMenuItemsQueryValidator : AbstractValidator<ListMenuItemsQuery>
{
    public ListMenuItemsQueryValidator()
    {
        RuleFor(query => query.TenantId)
            .NotEmpty();

        RuleFor(query => query.BranchId)
            .NotEmpty();
    }
}
