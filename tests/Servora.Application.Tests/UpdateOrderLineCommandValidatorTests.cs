using Servora.Application.Ordering.UpdateOrderLine;

namespace Servora.Application.Tests;

public sealed class UpdateOrderLineCommandValidatorTests
{
    private readonly UpdateOrderLineCommandValidator _validator = new();

    [Fact]
    public async Task Rejects_a_non_positive_quantity()
    {
        var result = await _validator.ValidateAsync(
            new UpdateOrderLineCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0, null));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_command()
    {
        var result = await _validator.ValidateAsync(
            new UpdateOrderLineCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 2, "az şeker"));

        Assert.True(result.IsValid);
    }
}
