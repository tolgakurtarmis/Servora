using FluentValidation;

namespace Servora.Application.Ordering.CancelTableSession;

public sealed class CancelTableSessionCommandValidator : AbstractValidator<CancelTableSessionCommand>
{
    public CancelTableSessionCommandValidator()
    {

        RuleFor(command => command.BranchId)
            .NotEmpty();

        RuleFor(command => command.TableSessionId)
            .NotEmpty();
    }
}
