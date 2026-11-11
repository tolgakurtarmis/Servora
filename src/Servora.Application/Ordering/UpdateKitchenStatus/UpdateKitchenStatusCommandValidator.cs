using FluentValidation;
using Servora.Domain.Enums;

namespace Servora.Application.Ordering.UpdateKitchenStatus;

public sealed class UpdateKitchenStatusCommandValidator : AbstractValidator<UpdateKitchenStatusCommand>
{
    public UpdateKitchenStatusCommandValidator()
    {

        RuleFor(command => command.BranchId)
            .NotEmpty();

        RuleFor(command => command.OrderLineId)
            .NotEmpty();

        RuleFor(command => command.Status)
            .IsInEnum();
    }
}
