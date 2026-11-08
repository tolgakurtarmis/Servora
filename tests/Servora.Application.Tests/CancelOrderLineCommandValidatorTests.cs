using Servora.Application.Ordering.CancelOrderLine;

namespace Servora.Application.Tests;

public sealed class CancelOrderLineCommandValidatorTests
{
    private readonly CancelOrderLineCommandValidator _validator = new();

    [Fact]
    public async Task Rejects_an_empty_order_line_id()
    {
        var result = await _validator.ValidateAsync(
            new CancelOrderLineCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_command()
    {
        var result = await _validator.ValidateAsync(
            new CancelOrderLineCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));

        Assert.True(result.IsValid);
    }
}
