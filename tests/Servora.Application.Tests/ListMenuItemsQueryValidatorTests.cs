using Servora.Application.Catalog.ListMenuItems;

namespace Servora.Application.Tests;

public sealed class ListMenuItemsQueryValidatorTests
{
    private readonly ListMenuItemsQueryValidator _validator = new();

    [Fact]
    public async Task Rejects_an_empty_branch_id()
    {
        var result = await _validator.ValidateAsync(new ListMenuItemsQuery(Guid.NewGuid(), Guid.Empty));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_query()
    {
        var result = await _validator.ValidateAsync(new ListMenuItemsQuery(Guid.NewGuid(), Guid.NewGuid()));

        Assert.True(result.IsValid);
    }
}
