using Servora.Application.Ordering.ListKitchenQueue;

namespace Servora.Application.Tests;

public sealed class ListKitchenQueueQueryValidatorTests
{
    private readonly ListKitchenQueueQueryValidator _validator = new();

    [Fact]
    public async Task Rejects_an_empty_branch_id()
    {
        var result = await _validator.ValidateAsync(new ListKitchenQueueQuery(Guid.Empty));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_query()
    {
        var result = await _validator.ValidateAsync(new ListKitchenQueueQuery(Guid.NewGuid()));

        Assert.True(result.IsValid);
    }
}
