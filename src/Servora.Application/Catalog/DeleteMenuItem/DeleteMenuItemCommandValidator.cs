using FluentValidation;

namespace Servora.Application.Catalog.DeleteMenuItem;

public sealed class DeleteMenuItemCommandValidator : AbstractValidator<DeleteMenuItemCommand>
{
    public DeleteMenuItemCommandValidator()
    {

        RuleFor(command => command.BranchId).NotEmpty();
        RuleFor(command => command.MenuItemId).NotEmpty();
    }
}
