using Servora.Application.Catalog.CreateMenuItem;

namespace Servora.Application.Tests;

public sealed class CreateMenuItemCommandValidatorTests
{
    private readonly CreateMenuItemCommandValidator _validator = new();

    [Fact]
    public async Task Rejects_a_non_positive_price()
    {
        var result = await _validator.ValidateAsync(
            new CreateMenuItemCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Latte", 0));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_command()
    {
        var result = await _validator.ValidateAsync(
            new CreateMenuItemCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Latte", 120));

        Assert.True(result.IsValid);
    }
}
