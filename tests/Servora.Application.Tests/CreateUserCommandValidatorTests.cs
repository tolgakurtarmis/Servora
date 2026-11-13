using Servora.Application.Users.CreateUser;
using Servora.Domain.Enums;

namespace Servora.Application.Tests;

public sealed class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator = new();

    [Fact]
    public async Task Rejects_a_short_password()
    {
        var result = await _validator.ValidateAsync(
            new CreateUserCommand("Ada", "ada.waiter", "Aa1", Role.Waiter, Guid.NewGuid(), null));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Rejects_a_tenant_on_platform_admin()
    {
        var result = await _validator.ValidateAsync(
            new CreateUserCommand("Ada", "ada.admin", "Secret12", Role.PlatformAdmin, Guid.NewGuid(), null));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_waiter()
    {
        var result = await _validator.ValidateAsync(
            new CreateUserCommand("Ada", "ada.waiter", "Secret12", Role.Waiter, Guid.NewGuid(), null));

        Assert.True(result.IsValid);
    }
}
