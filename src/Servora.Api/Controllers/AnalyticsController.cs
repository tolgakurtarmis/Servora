using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Analytics.GetDailySalesSummary;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("branches/{branchId:guid}/analytics")]
public sealed class AnalyticsController : BaseController
{
    public AnalyticsController(ISender sender) : base(sender)
    {
    }

    [HttpGet("daily-sales")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)}")]
    [ProducesResponseType(typeof(GetDailySalesSummaryResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<GetDailySalesSummaryResponse>> GetDailySales(
        Guid branchId,
        [FromQuery] DateOnly date,
        CancellationToken cancellationToken)
    {
        return Ok(await Sender.Send(new GetDailySalesSummaryQuery(branchId, date), cancellationToken));
    }
}
