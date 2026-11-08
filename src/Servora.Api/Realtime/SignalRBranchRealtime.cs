using Microsoft.AspNetCore.SignalR;
using Servora.Application.Abstractions;

namespace Servora.Api.Realtime;

internal sealed class SignalRBranchRealtime : IBranchRealtime
{
    private readonly IHubContext<BranchHub> _hub;
    private readonly HashSet<Guid> _kitchenBranches = [];
    private readonly HashSet<Guid> _tableBranches = [];

    public SignalRBranchRealtime(IHubContext<BranchHub> hub) => _hub = hub;

    public void KitchenQueueChanged(Guid branchId)
    {
        _kitchenBranches.Add(branchId);
    }

    public void TablesChanged(Guid branchId)
    {
        _tableBranches.Add(branchId);
    }

    public async Task FlushAsync(CancellationToken cancellationToken)
    {
        foreach (var branchId in _kitchenBranches)
        {
            await _hub.Clients.Group(branchId.ToString())
                .SendAsync("kitchenQueueChanged", cancellationToken);
        }

        foreach (var branchId in _tableBranches)
        {
            await _hub.Clients.Group(branchId.ToString())
                .SendAsync("tablesChanged", cancellationToken);
        }

        _kitchenBranches.Clear();
        _tableBranches.Clear();
    }
}
