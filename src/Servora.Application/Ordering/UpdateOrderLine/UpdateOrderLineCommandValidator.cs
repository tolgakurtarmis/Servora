using FluentValidation;

namespace Servora.Application.Ordering.UpdateOrderLine;

public sealed class UpdateOrderLineCommandValidator : AbstractValidator<UpdateOrderLineCommand>
{
    public UpdateOrderLineCommandValidator()
    {

        RuleFor(command => command.BranchId)
            .NotEmpty();

        RuleFor(command => command.OrderLineId)
            .NotEmpty();

        RuleFor(command => command.Quantity)
            .GreaterThan(0);

        RuleFor(command => command.Note)
            .MaximumLength(500)
            .When(command => command.Note is not null);
    }
}
