using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Servora.Ai.Contracts;
using Servora.Ai.Options;

namespace Servora.Ai.Controllers;

[ApiController]
[Route("ai")]
public sealed class AiController(
    IChatClient chatClient,
    IOptions<OllamaOptions> ollamaOptions,
    IEnumerable<AITool> tools) : ControllerBase
{
    private readonly OllamaOptions _options = ollamaOptions.Value;
    private readonly List<AITool> _tools = tools.ToList();

    [HttpGet("status")]
    public async Task<IActionResult> Status(CancellationToken cancellationToken)
    {
        try
        {
            _ = await chatClient.GetResponseAsync(
                [new ChatMessage(ChatRole.User, "Reply with OK.")],
                cancellationToken: cancellationToken);

            return Ok(new
            {
                status = "ok",
                provider = "ollama",
                model = _options.ChatModel,
                baseUrl = _options.BaseUrl
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                status = "unavailable",
                provider = "ollama",
                model = _options.ChatModel,
                baseUrl = _options.BaseUrl,
                error = ex.Message
            });
        }
    }

    [HttpPost("complete")]
    public async Task<ActionResult<CompleteResponse>> Complete(
        [FromBody] CompleteRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            return BadRequest(new { error = "Prompt is required." });
        }

        var response = await chatClient.GetResponseAsync(
            [new ChatMessage(ChatRole.User, request.Prompt.Trim())],
            new ChatOptions { Tools = _tools },
            cancellationToken);

        return Ok(new CompleteResponse(response.Text, _options.ChatModel));
    }
}
