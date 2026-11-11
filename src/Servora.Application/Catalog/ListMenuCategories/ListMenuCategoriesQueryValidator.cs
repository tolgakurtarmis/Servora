using FluentValidation;

namespace Servora.Application.Catalog.ListMenuCategories;

public sealed class ListMenuCategoriesQueryValidator : AbstractValidator<ListMenuCategoriesQuery>
{
    public ListMenuCategoriesQueryValidator()
    {

        RuleFor(query => query.BranchId)
            .NotEmpty();
    }
}
