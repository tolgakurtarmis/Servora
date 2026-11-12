using FluentValidation;

namespace Servora.Application.Auth.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.UserName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(command => command.Password)
            .NotEmpty();
    }
}
