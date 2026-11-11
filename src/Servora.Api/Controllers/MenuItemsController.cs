using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Catalog.CreateMenuItem;
using Servora.Application.Catalog.DeleteMenuItem;
using Servora.Application.Catalog.ListMenuItems;
using Servora.Application.Catalog.UpdateMenuItem;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("branches/{branchId:guid}/menu-items")]
public sealed class MenuItemsController : BaseController
{
    public MenuItemsController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)},{nameof(Role.Waiter)},{nameof(Role.Cashier)},{nameof(Role.Kitchen)}")]
    [ProducesResponseType(typeof(IReadOnlyList<MenuItemResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<MenuItemResponse>>> List(
        Guid branchId,
        CancellationToken cancellationToken)
    {
        return Ok(await Sender.Send(new ListMenuItemsQuery(branchId), cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)}")]
    [ProducesResponseType(typeof(CreateMenuItemResponse), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<CreateMenuItemResponse>> Create(
        Guid branchId,
        CreateMenuItemRequest request,
        CancellationToken cancellationToken)
    {
        var menuItemId = await Sender.Send(
            new CreateMenuItemCommand(branchId, request.CategoryId, request.Name, request.Price),
            cancellationToken);
        return Created(
            $"/branches/{branchId}/menu-items/{menuItemId}",
            new CreateMenuItemResponse(menuItemId));
    }

    [HttpPut("{menuItemId:guid}")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)}")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Guid>> Update(
        Guid branchId,
        Guid menuItemId,
        UpdateMenuItemRequest request,
        CancellationToken cancellationToken)
    {
        var id = await Sender.Send(
            new UpdateMenuItemCommand(
                branchId,
                menuItemId,
                request.CategoryId,
                request.Name,
                request.Price,
                request.IsActive),
            cancellationToken);
        return Ok(id);
    }

    [HttpDelete("{menuItemId:guid}")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Delete(
        Guid branchId,
        Guid menuItemId,
        CancellationToken cancellationToken)
    {
        await Sender.Send(new DeleteMenuItemCommand(branchId, menuItemId), cancellationToken);
        return NoContent();
    }
}
