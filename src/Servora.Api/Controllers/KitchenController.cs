using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Ordering.ListKitchenQueue;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("tenants/{tenantId:guid}/branches/{branchId:guid}/kitchen")]
public sealed class KitchenController : BaseController
{
    public KitchenController(ISender sender) : base(sender)
    {
    }

    [HttpGet("queue")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)},{nameof(Role.Kitchen)},{nameof(Role.Waiter)}")]
    [ProducesResponseType(typeof(IReadOnlyList<KitchenQueueItemResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<KitchenQueueItemResponse>>> ListQueue(
        Guid tenantId,
        Guid branchId,
        CancellationToken cancellationToken)
    {
        return Ok(await Sender.Send(new ListKitchenQueueQuery(tenantId, branchId), cancellationToken));
    }
}
