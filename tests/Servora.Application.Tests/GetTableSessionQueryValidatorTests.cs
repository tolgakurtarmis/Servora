using Servora.Application.Ordering.GetTableSession;

namespace Servora.Application.Tests;

public sealed class GetTableSessionQueryValidatorTests
{
    private readonly GetTableSessionQueryValidator _validator = new();

    [Fact]
    public async Task Rejects_an_empty_session_id()
    {
        var result = await _validator.ValidateAsync(
            new GetTableSessionQuery(Guid.NewGuid(), Guid.Empty));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_query()
    {
        var result = await _validator.ValidateAsync(
            new GetTableSessionQuery(Guid.NewGuid(), Guid.NewGuid()));

        Assert.True(result.IsValid);
    }
}
