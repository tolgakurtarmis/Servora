using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Ordering.UpdateKitchenStatus;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("branches/{branchId:guid}/order-lines/{orderLineId:guid}/kitchen-status")]
public sealed class KitchenStatusController : BaseController
{
    public KitchenStatusController(ISender sender) : base(sender)
    {
    }

    [HttpPut]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)},{nameof(Role.Kitchen)},{nameof(Role.Waiter)}")]
    [ProducesResponseType(typeof(UpdateKitchenStatusResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<UpdateKitchenStatusResponse>> Update(
        Guid branchId,
        Guid orderLineId,
        UpdateKitchenStatusRequest request,
        CancellationToken cancellationToken)
    {
        var id = await Sender.Send(
            new UpdateKitchenStatusCommand(branchId, orderLineId, request.Status),
            cancellationToken);
        return Ok(new UpdateKitchenStatusResponse(id));
    }
}
