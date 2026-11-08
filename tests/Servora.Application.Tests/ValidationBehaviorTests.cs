using FluentValidation;
using Servora.Application.Abstractions;
using Servora.Application.Behaviors;

namespace Servora.Application.Tests;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Rejects_invalid_command_before_handler()
    {
        var behavior = new ValidationBehavior<SampleCommand, int>([new SampleCommandValidator()]);
        var handlerCalled = false;

        await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(
                new SampleCommand(""),
                _ =>
                {
                    handlerCalled = true;
                    return Task.FromResult(1);
                },
                CancellationToken.None));

        Assert.False(handlerCalled);
    }

    private sealed record SampleCommand(string Name) : ICommand<int>;

    private sealed class SampleCommandValidator : AbstractValidator<SampleCommand>
    {
        public SampleCommandValidator()
        {
            RuleFor(command => command.Name).NotEmpty();
        }
    }
}
