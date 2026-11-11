using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Ordering.CancelOrderLine;
using Servora.Application.Ordering.UpdateOrderLine;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("branches/{branchId:guid}/order-lines/{orderLineId:guid}")]
public sealed class OrderLinesController : BaseController
{
    public OrderLinesController(ISender sender) : base(sender)
    {
    }

    [HttpPut]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)},{nameof(Role.Waiter)}")]
    [ProducesResponseType(typeof(UpdateOrderLineResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<UpdateOrderLineResponse>> Update(
        Guid branchId,
        Guid orderLineId,
        UpdateOrderLineRequest request,
        CancellationToken cancellationToken)
    {
        var id = await Sender.Send(
            new UpdateOrderLineCommand(branchId, orderLineId, request.Quantity, request.Note),
            cancellationToken);
        return Ok(new UpdateOrderLineResponse(id));
    }

    [HttpDelete]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Cancel(
        Guid branchId,
        Guid orderLineId,
        CancellationToken cancellationToken)
    {
        await Sender.Send(new CancelOrderLineCommand(branchId, orderLineId), cancellationToken);
        return NoContent();
    }
}
