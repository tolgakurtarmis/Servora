using FluentValidation;

namespace Servora.Application.Ordering.AddOrderRound;

public sealed class AddOrderRoundCommandValidator : AbstractValidator<AddOrderRoundCommand>
{
    public AddOrderRoundCommandValidator()
    {

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
            line.RuleFor(item => item.Note)
                .MaximumLength(500)
                .When(item => item.Note is not null);
        });
    }
}
