using FluentValidation;
using Servora.Domain.Enums;

namespace Servora.Application.Ordering.TakePayment;

public sealed class TakePaymentCommandValidator : AbstractValidator<TakePaymentCommand>
{
    public TakePaymentCommandValidator()
    {
        RuleFor(command => command.TenantId)
            .NotEmpty();

        RuleFor(command => command.BranchId)
            .NotEmpty();

        RuleFor(command => command.TableSessionId)
            .NotEmpty();

        RuleFor(command => command.Method)
            .IsInEnum();
    }
}
