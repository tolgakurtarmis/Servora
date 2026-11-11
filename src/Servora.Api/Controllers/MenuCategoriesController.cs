using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Catalog.CreateMenuCategory;
using Servora.Application.Catalog.DeleteMenuCategory;
using Servora.Application.Catalog.ListMenuCategories;
using Servora.Application.Catalog.UpdateMenuCategory;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("branches/{branchId:guid}/menu-categories")]
public sealed class MenuCategoriesController : BaseController
{
    public MenuCategoriesController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)},{nameof(Role.Waiter)},{nameof(Role.Cashier)},{nameof(Role.Kitchen)}")]
    [ProducesResponseType(typeof(IReadOnlyList<MenuCategoryResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<MenuCategoryResponse>>> List(
        Guid branchId,
        CancellationToken cancellationToken)
    {
        return Ok(await Sender.Send(new ListMenuCategoriesQuery(branchId), cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)}")]
    [ProducesResponseType(typeof(CreateMenuCategoryResponse), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<CreateMenuCategoryResponse>> Create(
        Guid branchId,
        CreateMenuCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var categoryId = await Sender.Send(
            new CreateMenuCategoryCommand(branchId, request.Name, request.SortOrder),
            cancellationToken);
        return Created(
            $"/branches/{branchId}/menu-categories/{categoryId}",
            new CreateMenuCategoryResponse(categoryId));
    }

    [HttpPut("{categoryId:guid}")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)}")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Guid>> Update(
        Guid branchId,
        Guid categoryId,
        UpdateMenuCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var id = await Sender.Send(
            new UpdateMenuCategoryCommand(
                branchId,
                categoryId,
                request.Name,
                request.SortOrder,
                request.IsActive),
            cancellationToken);
        return Ok(id);
    }

    [HttpDelete("{categoryId:guid}")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)},{nameof(Role.Manager)}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Delete(
        Guid branchId,
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        await Sender.Send(new DeleteMenuCategoryCommand(branchId, categoryId), cancellationToken);
        return NoContent();
    }
}
