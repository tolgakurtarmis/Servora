using Servora.Application.Branches.CreateBranch;

namespace Servora.Application.Tests;

public sealed class CreateBranchCommandValidatorTests
{
    private readonly CreateBranchCommandValidator _validator = new();

    [Fact]
    public async Task Rejects_empty_name()
    {
        var result = await _validator.ValidateAsync(new CreateBranchCommand(Guid.NewGuid(), ""));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_command()
    {
        var result = await _validator.ValidateAsync(new CreateBranchCommand(Guid.NewGuid(), "Kadıköy"));

        Assert.True(result.IsValid);
    }
}
