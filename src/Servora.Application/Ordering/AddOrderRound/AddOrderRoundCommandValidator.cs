using FluentValidation;

namespace Servora.Application.Ordering.AddOrderRound;

public sealed class AddOrderRoundCommandValidator : AbstractValidator<AddOrderRoundCommand>
{
    public AddOrderRoundCommandValidator()
    {
        RuleFor(command => command.TenantId)
            .NotEmpty();

        RuleFor(command => command.BranchId)
            .NotEmpty();

        RuleFor(command => command.TableSessionId)
            .NotEmpty();

        RuleFor(command => command.Lines)
            .NotEmpty();

        RuleForEach(command => command.Lines).ChildRules(line =>
        {
            line.RuleFor(item => item.MenuItemId).NotEmpty();
            line.RuleFor(item => item.Quantity).GreaterThan(0);
        });
    }
}
