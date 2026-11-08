using Servora.Application.Abstractions;
using Servora.Application.Behaviors;

namespace Servora.Application.Tests;

public sealed class UnitOfWorkBehaviorTests
{
    [Fact]
    public async Task Saves_after_command()
    {
        var unitOfWork = new FakeUnitOfWork();
        var behavior = new UnitOfWorkBehavior<SampleCommand, int>(unitOfWork);

        var result = await behavior.Handle(new SampleCommand(), _ => Task.FromResult(7), CancellationToken.None);

        Assert.Equal(7, result);
        Assert.Equal(1, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task Does_not_save_after_query()
    {
        var unitOfWork = new FakeUnitOfWork();
        var behavior = new UnitOfWorkBehavior<SampleQuery, int>(unitOfWork);

        await behavior.Handle(new SampleQuery(), _ => Task.FromResult(3), CancellationToken.None);

        Assert.Equal(0, unitOfWork.SaveCount);
    }

    private sealed record SampleCommand : ICommand<int>;

    private sealed record SampleQuery : IQuery<int>;

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveCount { get; private set; }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveCount++;
            return Task.FromResult(0);
        }
    }
}
