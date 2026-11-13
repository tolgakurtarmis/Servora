namespace Servora.Ai.Tools;

public sealed class SalesSummaryTool(
    HttpClient httpClient,
    AiCallContext callContext,
    IHttpContextAccessor httpContextAccessor)
{
    public async Task<string> GetWeeklySalesOverviewAsync(CancellationToken cancellationToken = default)
    {
        if (callContext.BranchId == Guid.Empty)
        {
            return """{"error":"BranchId is required."}""";
        }

        var to = DateOnly.FromDateTime(DateTime.UtcNow);
        var from = to.AddDays(-6);
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"branches/{callContext.BranchId}/analytics/sales-range?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");

        var authorization = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(authorization))
        {
            request.Headers.TryAddWithoutValidation("Authorization", authorization);
        }

        using var response = await httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return $"{{\"error\":\"Servora.Api returned {(int)response.StatusCode}\",\"body\":{System.Text.Json.JsonSerializer.Serialize(body)}}}";
        }

        return body;
    }
}
