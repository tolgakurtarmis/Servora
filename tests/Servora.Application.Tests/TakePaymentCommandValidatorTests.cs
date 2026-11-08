using Servora.Application.Ordering.TakePayment;
using Servora.Domain.Enums;

namespace Servora.Application.Tests;

public sealed class TakePaymentCommandValidatorTests
{
    private readonly TakePaymentCommandValidator _validator = new();

    [Fact]
    public async Task Rejects_an_unknown_method()
    {
        var result = await _validator.ValidateAsync(
            new TakePaymentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), (PaymentMethod)0));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_command()
    {
        var result = await _validator.ValidateAsync(
            new TakePaymentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PaymentMethod.Cash));

        Assert.True(result.IsValid);
    }
}
