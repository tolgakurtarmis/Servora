using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Ordering.GetTableSession;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("tenants/{tenantId:guid}/branches/{branchId:guid}/sessions")]
public sealed class SessionsController(ISender sender) : BaseController(sender)
{
    [HttpGet("{sessionId:guid}")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)},{nameof(Role.Waiter)},{nameof(Role.Cashier)},{nameof(Role.Kitchen)}")]
    [ProducesResponseType(typeof(TableSessionDetailResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<TableSessionDetailResponse>> Get(
        Guid tenantId,
        Guid branchId,
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        var response = await Sender.Send(
            new GetTableSessionQuery(tenantId, branchId, sessionId),
            cancellationToken);
        return Ok(response);
    }
}
