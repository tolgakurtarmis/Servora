using Servora.Application.Abstractions;
using Servora.Application.Behaviors;

namespace Servora.Application.Tests;

public sealed class UnitOfWorkBehaviorTests
{
    [Fact]
    public async Task Saves_after_command()
    {
        var unitOfWork = new FakeUnitOfWork();
        var realtime = new FakeBranchRealtime();
        var behavior = new UnitOfWorkBehavior<SampleCommand, int>(unitOfWork, realtime);

        var result = await behavior.Handle(new SampleCommand(), _ => Task.FromResult(7), CancellationToken.None);

        Assert.Equal(7, result);
        Assert.Equal(1, unitOfWork.SaveCount);
        Assert.Equal(1, realtime.FlushCount);
    }

    [Fact]
    public async Task Does_not_save_after_query()
    {
        var unitOfWork = new FakeUnitOfWork();
        var realtime = new FakeBranchRealtime();
        var behavior = new UnitOfWorkBehavior<SampleQuery, int>(unitOfWork, realtime);

        await behavior.Handle(new SampleQuery(), _ => Task.FromResult(3), CancellationToken.None);

        Assert.Equal(0, unitOfWork.SaveCount);
        Assert.Equal(0, realtime.FlushCount);
    }

    private sealed record SampleCommand : ICommand<int>;

    private sealed record SampleQuery : IQuery<int>;

    private sealed class FakeBranchRealtime : IBranchRealtime
    {
        public int FlushCount { get; private set; }

        public void KitchenQueueChanged(Guid branchId)
        {
        }

        public void TablesChanged(Guid branchId)
        {
        }

        public Task FlushAsync(CancellationToken cancellationToken)
        {
            FlushCount++;
            return Task.CompletedTask;
        }
    }

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
