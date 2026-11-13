using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Analytics.GetSalesRangeSummary;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("branches/{branchId:guid}/analytics")]
public sealed class AnalyticsController : BaseController
{
    public AnalyticsController(ISender sender) : base(sender)
    {
    }

    [HttpGet("sales-range")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)}")]
    [ProducesResponseType(typeof(GetSalesRangeSummaryResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<GetSalesRangeSummaryResponse>> GetSalesRange(
        Guid branchId,
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken cancellationToken)
    {
        return Ok(await Sender.Send(new GetSalesRangeSummaryQuery(branchId, from, to), cancellationToken));
    }
}
