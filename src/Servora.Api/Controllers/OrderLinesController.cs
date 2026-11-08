using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Ordering.CancelOrderLine;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("tenants/{tenantId:guid}/branches/{branchId:guid}/order-lines/{orderLineId:guid}")]
public sealed class OrderLinesController : BaseController
{
    public OrderLinesController(ISender sender) : base(sender)
    {
    }

    [HttpDelete]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Cancel(
        Guid tenantId,
        Guid branchId,
        Guid orderLineId,
        CancellationToken cancellationToken)
    {
        await Sender.Send(new CancelOrderLineCommand(tenantId, branchId, orderLineId), cancellationToken);
        return NoContent();
    }
}
