using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Ordering.ListOpenTables;
using Servora.Application.Tables.CreateTable;
using Servora.Application.Tables.DeleteTable;
using Servora.Application.Tables.ListTables;
using Servora.Application.Tables.UpdateTable;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("branches/{branchId:guid}/tables")]
public sealed class TablesController : BaseController
{
    public TablesController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)},{nameof(Role.Waiter)},{nameof(Role.Cashier)},{nameof(Role.Kitchen)}")]
    [ProducesResponseType(typeof(IReadOnlyList<TableResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<TableResponse>>> List(
        Guid branchId,
        CancellationToken cancellationToken)
    {
        return Ok(await Sender.Send(new ListTablesQuery(branchId), cancellationToken));
    }

    [HttpGet("open")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)},{nameof(Role.Waiter)},{nameof(Role.Cashier)},{nameof(Role.Kitchen)}")]
    [ProducesResponseType(typeof(IReadOnlyList<OpenTableResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<OpenTableResponse>>> ListOpen(
        Guid branchId,
        CancellationToken cancellationToken)
    {
        return Ok(await Sender.Send(new ListOpenTablesQuery(branchId), cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)}")]
    [ProducesResponseType(typeof(CreateTableResponse), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<CreateTableResponse>> Create(
        Guid branchId,
        CreateTableRequest request,
        CancellationToken cancellationToken)
    {
        var tableId = await Sender.Send(new CreateTableCommand(branchId, request.Name), cancellationToken);
        return Created(
            $"/branches/{branchId}/tables/{tableId}",
            new CreateTableResponse(tableId));
    }

    [HttpPut("{tableId:guid}")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)}")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Guid>> Update(
        Guid branchId,
        Guid tableId,
        UpdateTableRequest request,
        CancellationToken cancellationToken)
    {
        var id = await Sender.Send(
            new UpdateTableCommand(branchId, tableId, request.Name, request.IsActive),
            cancellationToken);
        return Ok(id);
    }

    [HttpDelete("{tableId:guid}")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Delete(
        Guid branchId,
        Guid tableId,
        CancellationToken cancellationToken)
    {
        await Sender.Send(new DeleteTableCommand(branchId, tableId), cancellationToken);
        return NoContent();
    }
}
