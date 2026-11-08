using FluentValidation;
using Servora.Domain.Enums;

namespace Servora.Application.Tenants.CreateTenant;

public sealed class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.MaxBranches)
            .GreaterThan(0);

        RuleFor(command => command.MaxUsers)
            .GreaterThan(0);

        RuleFor(command => command.Status)
            .IsInEnum()
            .When(command => command.Status.HasValue);
    }
}
