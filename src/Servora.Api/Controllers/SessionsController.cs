using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Ordering.CancelTableSession;
using Servora.Application.Ordering.GetTableSession;
using Servora.Application.Ordering.TransferTableSession;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("branches/{branchId:guid}/sessions")]
public sealed class SessionsController(ISender sender) : BaseController(sender)
{
    [HttpGet("{sessionId:guid}")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)},{nameof(Role.Waiter)},{nameof(Role.Cashier)},{nameof(Role.Kitchen)}")]
    [ProducesResponseType(typeof(TableSessionDetailResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<TableSessionDetailResponse>> Get(
        Guid branchId,
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        var response = await Sender.Send(
            new GetTableSessionQuery(branchId, sessionId),
            cancellationToken);
        return Ok(response);
    }

    [HttpPost("{sessionId:guid}/cancel")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)},{nameof(Role.Waiter)}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Cancel(
        Guid branchId,
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        await Sender.Send(
            new CancelTableSessionCommand(branchId, sessionId),
            cancellationToken);
        return NoContent();
    }

    [HttpPost("{sessionId:guid}/transfer")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)},{nameof(Role.Waiter)}")]
    [ProducesResponseType(typeof(TransferTableSessionResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<TransferTableSessionResponse>> Transfer(
        Guid branchId,
        Guid sessionId,
        TransferTableSessionRequest request,
        CancellationToken cancellationToken)
    {
        await Sender.Send(
            new TransferTableSessionCommand(branchId, sessionId, request.TargetTableId),
            cancellationToken);
        return Ok(new TransferTableSessionResponse(sessionId, request.TargetTableId));
    }
}
