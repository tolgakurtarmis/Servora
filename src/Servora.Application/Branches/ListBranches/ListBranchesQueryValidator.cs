using FluentValidation;

namespace Servora.Application.Branches.ListBranches;

public sealed class ListBranchesQueryValidator : AbstractValidator<ListBranchesQuery>
{
    public ListBranchesQueryValidator()
    {
        RuleFor(query => query.TenantId).NotEmpty();
    }
}
