using FluentValidation;

namespace Servora.Application.Ordering.TransferTableSession;

public sealed class TransferTableSessionCommandValidator : AbstractValidator<TransferTableSessionCommand>
{
    public TransferTableSessionCommandValidator()
    {

        RuleFor(command => command.BranchId)
            .NotEmpty();

        RuleFor(command => command.TableSessionId)
            .NotEmpty();

        RuleFor(command => command.TargetTableId)
            .NotEmpty();
    }
}
