using FluentValidation;
using Servora.Domain.Enums;

namespace Servora.Application.Users.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(command => command.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain a digit.");

        RuleFor(command => command.Role)
            .IsInEnum();

        When(command => command.Role == Role.PlatformAdmin, () =>
        {
            RuleFor(command => command.TenantId).Null();
            RuleFor(command => command.BranchId).Null();
        });
    }
}
