using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Users.CreateUser;
using Servora.Domain.Enums;

namespace Servora.Api.Controllers;

[Route("users")]
public sealed class UsersController : BaseController
{
    public UsersController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Role.PlatformAdmin)},{nameof(Role.Owner)}")]
    [ProducesResponseType(typeof(CreateUserResponse), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<CreateUserResponse>> Create(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var userId = await Sender.Send(command, cancellationToken);
        var response = new CreateUserResponse(userId);
        return Created($"/users/{userId}", response);
    }
}
