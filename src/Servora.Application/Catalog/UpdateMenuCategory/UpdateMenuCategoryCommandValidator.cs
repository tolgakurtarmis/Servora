using FluentValidation;

namespace Servora.Application.Catalog.UpdateMenuCategory;

public sealed class UpdateMenuCategoryCommandValidator : AbstractValidator<UpdateMenuCategoryCommand>
{
    public UpdateMenuCategoryCommandValidator()
    {
        RuleFor(command => command.TenantId)
            .NotEmpty();

        RuleFor(command => command.BranchId)
            .NotEmpty();

        RuleFor(command => command.CategoryId)
            .NotEmpty();

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.SortOrder)
            .GreaterThanOrEqualTo(0);
    }
}
