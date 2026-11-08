using Servora.Application.Ordering.UpdateKitchenStatus;
using Servora.Domain.Enums;

namespace Servora.Application.Tests;

public sealed class UpdateKitchenStatusCommandValidatorTests
{
    private readonly UpdateKitchenStatusCommandValidator _validator = new();

    [Fact]
    public async Task Rejects_an_unknown_status()
    {
        var result = await _validator.ValidateAsync(
            new UpdateKitchenStatusCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), (KitchenStatus)0));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_command()
    {
        var result = await _validator.ValidateAsync(
            new UpdateKitchenStatusCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                KitchenStatus.Preparing));

        Assert.True(result.IsValid);
    }
}
