using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servora.Application.Auth.Login;

namespace Servora.Api.Controllers;

[Route("auth")]
public sealed class AuthController : BaseController
{
    public AuthController(ISender sender) : base(sender)
    {
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<LoginResponse>> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command, cancellationToken);
        return Ok(response);
    }
}
