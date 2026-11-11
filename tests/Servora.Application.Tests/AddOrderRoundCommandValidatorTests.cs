using Servora.Application.Ordering.AddOrderRound;

namespace Servora.Application.Tests;

public sealed class AddOrderRoundCommandValidatorTests
{
    private readonly AddOrderRoundCommandValidator _validator = new();

    [Fact]
    public async Task Rejects_an_empty_round()
    {
        var result = await _validator.ValidateAsync(
            new AddOrderRoundCommand(Guid.NewGuid(), Guid.NewGuid(), []));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Rejects_a_non_positive_quantity()
    {
        var result = await _validator.ValidateAsync(
            new AddOrderRoundCommand(Guid.NewGuid(),
                Guid.NewGuid(),
                [new OrderLineInput(Guid.NewGuid(), 0)]));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_command()
    {
        var result = await _validator.ValidateAsync(
            new AddOrderRoundCommand(Guid.NewGuid(),
                Guid.NewGuid(),
                [new OrderLineInput(Guid.NewGuid(), 2)]));

        Assert.True(result.IsValid);
    }
}
