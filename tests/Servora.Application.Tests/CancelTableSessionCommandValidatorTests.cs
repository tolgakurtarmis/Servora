using Servora.Application.Ordering.CancelTableSession;

namespace Servora.Application.Tests;

public sealed class CancelTableSessionCommandValidatorTests
{
    private readonly CancelTableSessionCommandValidator _validator = new();

    [Fact]
    public async Task Rejects_an_empty_session_id()
    {
        var result = await _validator.ValidateAsync(
            new CancelTableSessionCommand(Guid.NewGuid(), Guid.Empty));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_command()
    {
        var result = await _validator.ValidateAsync(
            new CancelTableSessionCommand(Guid.NewGuid(), Guid.NewGuid()));

        Assert.True(result.IsValid);
    }
}
