using FluentValidation;

namespace Servora.Application.Ordering.ListKitchenQueue;

public sealed class ListKitchenQueueQueryValidator : AbstractValidator<ListKitchenQueueQuery>
{
    public ListKitchenQueueQueryValidator()
    {

        RuleFor(query => query.BranchId)
            .NotEmpty();
    }
}
