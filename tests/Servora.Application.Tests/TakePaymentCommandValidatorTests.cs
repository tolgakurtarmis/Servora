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
    public async Task Rejects_a_non_positive_amount()
    {
        var result = await _validator.ValidateAsync(
            new TakePaymentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PaymentMethod.Cash, 0));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_command_without_amount()
    {
        var result = await _validator.ValidateAsync(
            new TakePaymentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PaymentMethod.Cash));

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Accepts_a_valid_partial_amount()
    {
        var result = await _validator.ValidateAsync(
            new TakePaymentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PaymentMethod.Card, 25.50m));

        Assert.True(result.IsValid);
    }
}
