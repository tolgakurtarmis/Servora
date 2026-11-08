using Servora.Application.Auth.Refresh;

namespace Servora.Application.Tests;

public sealed class RefreshTokenCommandValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator = new();

    [Fact]
    public async Task Rejects_an_empty_refresh_token()
    {
        var result = await _validator.ValidateAsync(new RefreshTokenCommand(""));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_command()
    {
        var result = await _validator.ValidateAsync(new RefreshTokenCommand("token-value"));

        Assert.True(result.IsValid);
    }
}
