using FluentValidation;

namespace Servora.Application.Tables.DeleteTable;

public sealed class DeleteTableCommandValidator : AbstractValidator<DeleteTableCommand>
{
    public DeleteTableCommandValidator()
    {

        RuleFor(command => command.BranchId).NotEmpty();
        RuleFor(command => command.TableId).NotEmpty();
    }
}
