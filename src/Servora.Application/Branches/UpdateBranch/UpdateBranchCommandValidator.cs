using FluentValidation;

namespace Servora.Application.Branches.UpdateBranch;

public sealed class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchCommandValidator()
    {
        RuleFor(command => command.TenantId).NotEmpty();
        RuleFor(command => command.BranchId).NotEmpty();
        RuleFor(command => command.Name).NotEmpty().MaximumLength(200);
    }
}
