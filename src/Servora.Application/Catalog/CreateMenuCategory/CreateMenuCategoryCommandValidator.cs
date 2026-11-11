using FluentValidation;

namespace Servora.Application.Catalog.CreateMenuCategory;

public sealed class CreateMenuCategoryCommandValidator : AbstractValidator<CreateMenuCategoryCommand>
{
    public CreateMenuCategoryCommandValidator()
    {

        RuleFor(command => command.BranchId)
            .NotEmpty();

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.SortOrder)
            .GreaterThanOrEqualTo(0);
    }
}
