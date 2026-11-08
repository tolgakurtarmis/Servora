using Servora.Application.Tables.CreateTable;

namespace Servora.Application.Tests;

public sealed class CreateTableCommandValidatorTests
{
    private readonly CreateTableCommandValidator _validator = new();

    [Fact]
    public async Task Rejects_empty_name()
    {
        var result = await _validator.ValidateAsync(new CreateTableCommand(Guid.NewGuid(), Guid.NewGuid(), ""));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_command()
    {
        var result = await _validator.ValidateAsync(new CreateTableCommand(Guid.NewGuid(), Guid.NewGuid(), "Masa 1"));

        Assert.True(result.IsValid);
    }
}
