using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Users.CreateUser;
using Servora.Application.Users.ListUsers;
using Servora.Application.Users.UpdateUser;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("users")]
public sealed class UsersController : BaseController
{
    public UsersController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)}")]
    [ProducesResponseType(typeof(IReadOnlyList<UserResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> List(
        [FromQuery] Guid? tenantId,
        CancellationToken cancellationToken)
    {
        return Ok(await Sender.Send(new ListUsersQuery(tenantId), cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)}")]
    [ProducesResponseType(typeof(CreateUserResponse), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<CreateUserResponse>> Create(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var userId = await Sender.Send(command, cancellationToken);
        return Created($"/users/{userId}", new CreateUserResponse(userId));
    }

    [HttpPut("{userId:guid}")]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)}")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Guid>> Update(
        Guid userId,
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var id = await Sender.Send(
            new UpdateUserCommand(userId, request.Name, request.IsActive),
            cancellationToken);
        return Ok(id);
    }
}
