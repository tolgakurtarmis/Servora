using Servora.Application.Ordering.TransferTableSession;

namespace Servora.Application.Tests;

public sealed class TransferTableSessionCommandValidatorTests
{
    private readonly TransferTableSessionCommandValidator _validator = new();

    [Fact]
    public async Task Rejects_an_empty_target_table_id()
    {
        var result = await _validator.ValidateAsync(
            new TransferTableSessionCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_command()
    {
        var result = await _validator.ValidateAsync(
            new TransferTableSessionCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));

        Assert.True(result.IsValid);
    }
}
