using Servora.Application.Tenants.CreateTenant;
using Servora.Domain.Enums;

namespace Servora.Application.Tests;

public sealed class CreateTenantCommandValidatorTests
{
    private readonly CreateTenantCommandValidator _validator = new();

    [Fact]
    public async Task Rejects_empty_name()
    {
        var result = await _validator.ValidateAsync(new CreateTenantCommand("", 1, 1, SubscriptionStatus.Trial));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_command()
    {
        var result = await _validator.ValidateAsync(new CreateTenantCommand("Sahil Pastanesi", 3, 20, null));

        Assert.True(result.IsValid);
    }
}
