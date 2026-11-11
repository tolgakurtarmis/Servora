using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Ordering.TakePayment;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("branches/{branchId:guid}/sessions/{sessionId:guid}/payments")]
public sealed class PaymentsController : BaseController
{
    public PaymentsController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)},{nameof(Role.Cashier)}")]
    [ProducesResponseType(typeof(TakePaymentResponse), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<TakePaymentResponse>> Take(
        Guid branchId,
        Guid sessionId,
        TakePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new TakePaymentCommand(branchId, sessionId, request.Method, request.Amount),
            cancellationToken);
        var response = new TakePaymentResponse(
            result.Id,
            result.Amount,
            result.Remaining,
            result.SessionClosed);
        return Created(
            $"/branches/{branchId}/sessions/{sessionId}/payments/{result.Id}",
            response);
    }
}
