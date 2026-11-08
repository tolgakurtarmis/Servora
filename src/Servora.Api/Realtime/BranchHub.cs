using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Servora.Application.Abstractions;
using Servora.Domain.Models;

namespace Servora.Api.Realtime;

[Authorize]
public sealed class BranchHub : Hub
{
    private readonly IRepository<Branch> _branches;

    public BranchHub(IRepository<Branch> branches) => _branches = branches;

    public async Task JoinBranch(Guid branchId)
    {
        var branch = await _branches.GetByIdReadOnlyAsync(branchId, Context.ConnectionAborted) ?? throw new HubException("Branch was not found.");
        var tenantId = ReadGuid("tenant_id");
        var isPlatformAdmin = Context.User?.FindAll("role").Any(claim => claim.Value == "PlatformAdmin") == true;
        if (!isPlatformAdmin && tenantId != branch.TenantId)
        {
            throw new HubException("You can only join your own tenant.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, branch.Id.ToString());
    }

    private Guid? ReadGuid(string claimType)
    {
        var value = Context.User?.FindFirstValue(claimType);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
