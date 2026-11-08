using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Ordering.OpenTableSession;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("tenants/{tenantId:guid}/branches/{branchId:guid}/tables/{tableId:guid}/sessions")]
public sealed class TableSessionsController : BaseController
{
    public TableSessionsController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)},{nameof(Role.Waiter)}")]
    [ProducesResponseType(typeof(OpenTableSessionResponse), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<OpenTableSessionResponse>> Open(
        Guid tenantId,
        Guid branchId,
        Guid tableId,
        CancellationToken cancellationToken)
    {
        var sessionId = await Sender.Send(
            new OpenTableSessionCommand(tenantId, branchId, tableId),
            cancellationToken);
        var response = new OpenTableSessionResponse(sessionId);
        return Created(
            $"/tenants/{tenantId}/branches/{branchId}/tables/{tableId}/sessions/{sessionId}",
            response);
    }
}
