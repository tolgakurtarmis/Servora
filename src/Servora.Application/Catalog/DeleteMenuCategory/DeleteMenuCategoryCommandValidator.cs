using FluentValidation;

namespace Servora.Application.Catalog.DeleteMenuCategory;

public sealed class DeleteMenuCategoryCommandValidator : AbstractValidator<DeleteMenuCategoryCommand>
{
    public DeleteMenuCategoryCommandValidator()
    {
        RuleFor(command => command.TenantId)
            .NotEmpty();

        RuleFor(command => command.BranchId)
            .NotEmpty();

        RuleFor(command => command.CategoryId)
            .NotEmpty();
    }
}
