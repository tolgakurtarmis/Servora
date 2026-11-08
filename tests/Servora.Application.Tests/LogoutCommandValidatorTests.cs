using Servora.Application.Auth.Logout;

namespace Servora.Application.Tests;

public sealed class LogoutCommandValidatorTests
{
    private readonly LogoutCommandValidator _validator = new();

    [Fact]
    public async Task Rejects_an_empty_refresh_token()
    {
        var result = await _validator.ValidateAsync(new LogoutCommand(""));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_command()
    {
        var result = await _validator.ValidateAsync(new LogoutCommand("token-value"));

        Assert.True(result.IsValid);
    }
}
