using Servora.Application.Catalog.CreateMenuCategory;

namespace Servora.Application.Tests;

public sealed class CreateMenuCategoryCommandValidatorTests
{
    private readonly CreateMenuCategoryCommandValidator _validator = new();

    [Fact]
    public async Task Rejects_an_empty_name()
    {
        var result = await _validator.ValidateAsync(
            new CreateMenuCategoryCommand(Guid.NewGuid(), Guid.NewGuid(), "", 0));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_command()
    {
        var result = await _validator.ValidateAsync(
            new CreateMenuCategoryCommand(Guid.NewGuid(), Guid.NewGuid(), "Drinks", 1));

        Assert.True(result.IsValid);
    }
}
