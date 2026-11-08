using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Servora.Api.Controllers;

[ApiController]
[Authorize]
[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Conflict)]
public abstract class BaseController : ControllerBase
{
    protected readonly ISender Sender;

    protected BaseController(ISender sender)
    {
        Sender = sender;
    }
}
