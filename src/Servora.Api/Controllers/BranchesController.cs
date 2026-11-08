using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Branches.CreateBranch;
using Servora.Application.Branches.ListBranches;
using Servora.Application.Branches.UpdateBranch;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("tenants/{tenantId:guid}/branches")]
public sealed class BranchesController : BaseController
{
    public BranchesController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)}")]
    [ProducesResponseType(typeof(IReadOnlyList<BranchResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<BranchResponse>>> List(
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        return Ok(await Sender.Send(new ListBranchesQuery(tenantId), cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)}")]
    [ProducesResponseType(typeof(CreateBranchResponse), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<CreateBranchResponse>> Create(
        Guid tenantId,
        CreateBranchRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await Sender.Send(
            new CreateBranchCommand(tenantId, request.Name, request.UsesKitchen ?? true),
            cancellationToken);
        return Created($"/tenants/{tenantId}/branches/{branchId}", new CreateBranchResponse(branchId));
    }

    [HttpPut("{branchId:guid}")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)}")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Guid>> Update(
        Guid tenantId,
        Guid branchId,
        UpdateBranchRequest request,
        CancellationToken cancellationToken)
    {
        var id = await Sender.Send(
            new UpdateBranchCommand(tenantId, branchId, request.Name, request.UsesKitchen, request.IsActive),
            cancellationToken);
        return Ok(id);
    }
}
