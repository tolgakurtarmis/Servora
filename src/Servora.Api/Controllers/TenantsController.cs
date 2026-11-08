using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Tenants.CreateTenant;
using Servora.Application.Tenants.ListTenants;
using Servora.Application.Tenants.UpdateTenant;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("tenants")]
public sealed class TenantsController : BaseController
{
    public TenantsController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.PlatformAdmin))]
    [ProducesResponseType(typeof(IReadOnlyList<TenantResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<TenantResponse>>> List(CancellationToken cancellationToken)
    {
        return Ok(await Sender.Send(new ListTenantsQuery(), cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = nameof(Role.PlatformAdmin))]
    [ProducesResponseType(typeof(CreateTenantResponse), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<CreateTenantResponse>> Create(
        CreateTenantCommand command,
        CancellationToken cancellationToken)
    {
        var tenantId = await Sender.Send(command, cancellationToken);
        return Created($"/tenants/{tenantId}", new CreateTenantResponse(tenantId));
    }

    [HttpPut("{tenantId:guid}")]
    [Authorize(Roles = nameof(Role.PlatformAdmin))]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Guid>> Update(
        Guid tenantId,
        UpdateTenantRequest request,
        CancellationToken cancellationToken)
    {
        var id = await Sender.Send(
            new UpdateTenantCommand(tenantId, request.Name, request.IsActive),
            cancellationToken);
        return Ok(id);
    }
}
