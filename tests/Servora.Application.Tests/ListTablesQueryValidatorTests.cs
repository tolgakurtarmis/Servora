using Servora.Application.Tables.ListTables;

namespace Servora.Application.Tests;

public sealed class ListTablesQueryValidatorTests
{
    private readonly ListTablesQueryValidator _validator = new();

    [Fact]
    public async Task Rejects_an_empty_branch_id()
    {
        var result = await _validator.ValidateAsync(new ListTablesQuery(Guid.NewGuid(), Guid.Empty));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_query()
    {
        var result = await _validator.ValidateAsync(new ListTablesQuery(Guid.NewGuid(), Guid.NewGuid()));

        Assert.True(result.IsValid);
    }
}
