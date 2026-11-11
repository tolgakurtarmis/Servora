using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Application.Branches.CreateBranch;
using Servora.Application.Branches.ListBranches;
using Servora.Application.Branches.UpdateBranch;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("branches")]
public sealed class BranchesController : BaseController
{
    private readonly ICurrentTenant _currentTenant;

    public BranchesController(ISender sender, ICurrentTenant currentTenant) : base(sender)
    {
        _currentTenant = currentTenant;
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)}")]
    [ProducesResponseType(typeof(IReadOnlyList<BranchResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<BranchResponse>>> List(
        [FromQuery] Guid? tenantId,
        CancellationToken cancellationToken)
    {
        return Ok(await Sender.Send(new ListBranchesQuery(ResolveTenantId(tenantId)), cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)}")]
    [ProducesResponseType(typeof(CreateBranchResponse), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<CreateBranchResponse>> Create(
        CreateBranchRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = ResolveTenantId(request.TenantId);
        var branchId = await Sender.Send(
            new CreateBranchCommand(tenantId, request.Name, request.UsesKitchen ?? true),
            cancellationToken);
        return Created($"/branches/{branchId}", new CreateBranchResponse(branchId));
    }

    [HttpPut("{branchId:guid}")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)}")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Guid>> Update(
        Guid branchId,
        UpdateBranchRequest request,
        CancellationToken cancellationToken)
    {
        var id = await Sender.Send(
            new UpdateBranchCommand(branchId, request.Name, request.UsesKitchen, request.IsActive),
            cancellationToken);
        return Ok(id);
    }

    private Guid ResolveTenantId(Guid? tenantId)
    {
        if (_currentTenant.IsPlatformAdmin)
        {
            return tenantId ?? Guid.Empty;
        }

        return _currentTenant.TenantId
            ?? throw new UnauthorizedException("Tenant context is missing.");
    }
}
