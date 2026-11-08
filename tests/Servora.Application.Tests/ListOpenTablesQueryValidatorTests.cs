using Servora.Application.Ordering.ListOpenTables;

namespace Servora.Application.Tests;

public sealed class ListOpenTablesQueryValidatorTests
{
    private readonly ListOpenTablesQueryValidator _validator = new();

    [Fact]
    public async Task Rejects_an_empty_branch_id()
    {
        var result = await _validator.ValidateAsync(new ListOpenTablesQuery(Guid.NewGuid(), Guid.Empty));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_query()
    {
        var result = await _validator.ValidateAsync(new ListOpenTablesQuery(Guid.NewGuid(), Guid.NewGuid()));

        Assert.True(result.IsValid);
    }
}
