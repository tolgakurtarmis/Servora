using FluentValidation;

namespace Servora.Application.Ordering.CancelOrderLine;

public sealed class CancelOrderLineCommandValidator : AbstractValidator<CancelOrderLineCommand>
{
    public CancelOrderLineCommandValidator()
    {

        RuleFor(command => command.BranchId)
            .NotEmpty();

        RuleFor(command => command.OrderLineId)
            .NotEmpty();
    }
}
