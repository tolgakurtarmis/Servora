using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Auth;
using Servora.Application.Auth.Login;
using Servora.Application.Auth.Logout;
using Servora.Application.Auth.Refresh;

namespace Servora.Api.Controllers;

[Route("auth")]
public sealed class AuthController : BaseController
{
    public AuthController(ISender sender) : base(sender)
    {
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<AuthResponse>> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command, cancellationToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<AuthResponse>> Refresh(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command, cancellationToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Logout(LogoutCommand command, CancellationToken cancellationToken)
    {
        await Sender.Send(command, cancellationToken);
        return NoContent();
    }
}
