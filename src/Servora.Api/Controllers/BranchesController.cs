using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Branches.CreateBranch;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("tenants/{tenantId:guid}/branches")]
public sealed class BranchesController : BaseController
{
    public BranchesController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)}")]
    [ProducesResponseType(typeof(CreateBranchResponse), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<CreateBranchResponse>> Create(
        Guid tenantId,
        CreateBranchRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await Sender.Send(new CreateBranchCommand(tenantId, request.Name), cancellationToken);
        var response = new CreateBranchResponse(branchId);
        return Created($"/tenants/{tenantId}/branches/{branchId}", response);
    }
}
