using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Catalog.CreateMenuItem;
using Servora.Application.Catalog.ListMenuItems;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("tenants/{tenantId:guid}/branches/{branchId:guid}/menu-items")]
public sealed class MenuItemsController : BaseController
{
    public MenuItemsController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)},{nameof(Role.Waiter)},{nameof(Role.Cashier)},{nameof(Role.Kitchen)}")]
    [ProducesResponseType(typeof(IReadOnlyList<MenuItemResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<MenuItemResponse>>> List(
        Guid tenantId,
        Guid branchId,
        CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new ListMenuItemsQuery(tenantId, branchId), cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)}")]
    [ProducesResponseType(typeof(CreateMenuItemResponse), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<CreateMenuItemResponse>> Create(
        Guid tenantId,
        Guid branchId,
        CreateMenuItemRequest request,
        CancellationToken cancellationToken)
    {
        var menuItemId = await Sender.Send(
            new CreateMenuItemCommand(tenantId, branchId, request.Name, request.Price),
            cancellationToken);
        var response = new CreateMenuItemResponse(menuItemId);
        return Created($"/tenants/{tenantId}/branches/{branchId}/menu-items/{menuItemId}", response);
    }
}
