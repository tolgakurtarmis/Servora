using FluentValidation;

namespace Servora.Application.Tables.UpdateTable;

public sealed class UpdateTableCommandValidator : AbstractValidator<UpdateTableCommand>
{
    public UpdateTableCommandValidator()
    {

        RuleFor(command => command.BranchId).NotEmpty();
        RuleFor(command => command.TableId).NotEmpty();
        RuleFor(command => command.Name).NotEmpty().MaximumLength(100);
    }
}
