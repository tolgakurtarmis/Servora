using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Servora.Ai.Contracts;
using Servora.Ai.Options;
using Servora.Ai.Services;
using Servora.Ai.Tools;

namespace Servora.Ai.Controllers;

[ApiController]
[Route("ai")]
public sealed class AiController(
    IChatClient chatClient,
    IOptions<OllamaOptions> ollamaOptions,
    IEnumerable<AITool> tools,
    AiCallContext callContext,
    SalesSummaryTool salesSummaryTool) : ControllerBase
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
            Sen Servora cafe asistanısın. Bugünün UTC tarihi: {today}.
            Kurallar:
            - Sadece Türkçe yaz.
            - JSON alan adlarını cevapta kullanma; doğal dil kullan.
            - Rakamları tool sonucundan al; uydurma.
            - Kısa ve net ol.
            - Tek gün / bugün / dün soruları için get_daily_sales_summary çağır (bugün için date verme).
            - Bu hafta / en çok ne sattım / zayıf gün için get_weekly_sales_overview çağır.
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

    [HttpPost("manager-advice")]
    public async Task<ActionResult<ManagerAdviceResponse>> ManagerAdvice(
        [FromBody] ManagerAdviceRequest request,
        CancellationToken cancellationToken)
    {
        if (request.BranchId == Guid.Empty)
        {
            return BadRequest(new { error = "BranchId is required." });
        }

        callContext.BranchId = request.BranchId;

        var salesJson = await salesSummaryTool.GetWeeklySalesOverviewAsync(cancellationToken);
        var facts = ManagerAdviceComposer.TryParseFacts(salesJson);
        if (facts is null)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                error = "Could not load sales facts from Servora.Api.",
                detail = salesJson
            });
        }

        var focus = string.IsNullOrWhiteSpace(request.Focus)
            ? "Genel şube performansı için öner."
            : request.Focus.Trim();

        var userMessage =
            $"""
            Aşağıdaki gerçeklere göre yöneticiye 3 aksiyon önerisi yaz.
            Odak: {focus}

            Gerçekler:
            {ManagerAdviceComposer.BuildFactsBlock(facts)}
            """;

        var response = await chatClient.GetResponseAsync(
            [
                new ChatMessage(ChatRole.System, ManagerAdviceComposer.BuildSystemPrompt()),
                new ChatMessage(ChatRole.User, userMessage)
            ],
            cancellationToken: cancellationToken);

        return Ok(new ManagerAdviceResponse(response.Text, _options.ChatModel, facts));
    }
}
