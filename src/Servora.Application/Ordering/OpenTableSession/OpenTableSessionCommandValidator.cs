using FluentValidation;

namespace Servora.Application.Ordering.OpenTableSession;

public sealed class OpenTableSessionCommandValidator : AbstractValidator<OpenTableSessionCommand>
{
    public OpenTableSessionCommandValidator()
    {
        RuleFor(command => command.TenantId)
            .NotEmpty();

        RuleFor(command => command.BranchId)
            .NotEmpty();

        RuleFor(command => command.TableId)
            .NotEmpty();
    }
}
