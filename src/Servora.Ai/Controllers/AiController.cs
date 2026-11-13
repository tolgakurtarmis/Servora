using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Servora.Ai.Contracts;
using Servora.Ai.Options;
using Servora.Ai.Tools;

namespace Servora.Ai.Controllers;

[ApiController]
[Route("ai")]
public sealed class AiController(
    IChatClient chatClient,
    IOptions<OllamaOptions> ollamaOptions,
    IEnumerable<AITool> tools,
    AiCallContext callContext) : ControllerBase
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

        if (request.BranchId == Guid.Empty)
        {
            return BadRequest(new { error = "BranchId is required." });
        }

        callContext.BranchId = request.BranchId;

        var today = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
        var systemPrompt =
            $"""
            Sen Servora cafe sahibi asistanısın. Bugünün UTC tarihi: {today}.
            Kurallar:
            - Sadece Türkçe yaz.
            - JSON alan adlarını (totalRevenue, topProducts, weakestDay vb.) cevapta kullanma; doğal dil kullan (ciro, sipariş, en çok satan ürün, en zayıf gün).
            - Rakamları tool sonucundan al; uydurma.
            - Kısa ve net ol: 2-4 cümle.
            - "bugün/dün" veya tek gün için get_daily_sales_summary çağır (bugün için date verme).
            - "bu hafta", "zayıf gün", "en çok satan ürünler" için get_weekly_sales_overview çağır.
            """;

        var response = await chatClient.GetResponseAsync(
            [
                new ChatMessage(ChatRole.System, systemPrompt),
                new ChatMessage(ChatRole.User, request.Prompt.Trim())
            ],
            new ChatOptions { Tools = _tools },
            cancellationToken);

        return Ok(new CompleteResponse(response.Text, _options.ChatModel));
    }
}
