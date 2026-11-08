namespace Servora.Application.Abstractions;

public interface IBranchRealtime
{
    void KitchenQueueChanged(Guid branchId);

    void TablesChanged(Guid branchId);

    Task FlushAsync(CancellationToken cancellationToken);
}
