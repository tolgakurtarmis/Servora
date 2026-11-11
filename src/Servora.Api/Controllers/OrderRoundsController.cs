using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Ordering.AddOrderRound;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("branches/{branchId:guid}/sessions/{sessionId:guid}/rounds")]
public sealed class OrderRoundsController : BaseController
{
    public OrderRoundsController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)},{nameof(Role.Waiter)}")]
    [ProducesResponseType(typeof(AddOrderRoundResponse), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<AddOrderRoundResponse>> Add(
        Guid branchId,
        Guid sessionId,
        AddOrderRoundRequest request,
        CancellationToken cancellationToken)
    {
        var roundId = await Sender.Send(
            new AddOrderRoundCommand(branchId, sessionId, request.Lines),
            cancellationToken);
        var response = new AddOrderRoundResponse(roundId);
        return Created(
            $"/branches/{branchId}/sessions/{sessionId}/rounds/{roundId}",
            response);
    }
}
