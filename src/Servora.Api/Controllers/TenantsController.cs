using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Tenants.CreateTenant;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("tenants")]
public sealed class TenantsController : BaseController
{
    public TenantsController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    [Authorize(Roles = nameof(Role.PlatformAdmin))]
    [ProducesResponseType(typeof(CreateTenantResponse), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<CreateTenantResponse>> Create(
        CreateTenantCommand command,
        CancellationToken cancellationToken)
    {
        var tenantId = await Sender.Send(command, cancellationToken);
        var response = new CreateTenantResponse(tenantId);
        return Created($"/tenants/{tenantId}", response);
    }
}
