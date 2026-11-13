using Servora.Application.Auth.Login;

namespace Servora.Application.Tests;

public sealed class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public async Task Rejects_an_empty_username()
    {
        var result = await _validator.ValidateAsync(new LoginCommand("", "secret"));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_command()
    {
        var result = await _validator.ValidateAsync(new LoginCommand("sahil.owner", "secret"));

        Assert.True(result.IsValid);
    }
}
