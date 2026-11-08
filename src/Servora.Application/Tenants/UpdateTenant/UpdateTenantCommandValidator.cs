using FluentValidation;

namespace Servora.Application.Tenants.UpdateTenant;

public sealed class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator()
    {
        RuleFor(command => command.TenantId).NotEmpty();
        RuleFor(command => command.Name).NotEmpty().MaximumLength(200);
    }
}
