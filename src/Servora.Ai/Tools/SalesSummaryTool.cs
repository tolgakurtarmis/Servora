namespace Servora.Ai.Tools;

public sealed class SalesSummaryTool(
    HttpClient httpClient,
    AiCallContext callContext,
    IHttpContextAccessor httpContextAccessor)
{
    public async Task<string> GetDailySalesSummaryAsync(
        string? date = null,
        CancellationToken cancellationToken = default)
    {
        if (callContext.BranchId == Guid.Empty)
        {
            return """{"error":"BranchId is required."}""";
        }

        var resolvedDate = string.IsNullOrWhiteSpace(date)
            ? DateOnly.FromDateTime(DateTime.UtcNow)
            : DateOnly.TryParse(date, out var parsed)
                ? parsed
                : (DateOnly?)null;

        if (resolvedDate is null)
        {
            return """{"error":"date must be yyyy-MM-dd when provided."}""";
        }

        return await SendAsync(
            $"branches/{callContext.BranchId}/analytics/daily-sales?date={resolvedDate.Value:yyyy-MM-dd}",
            cancellationToken);
    }

    public async Task<string> GetWeeklySalesOverviewAsync(CancellationToken cancellationToken = default)
    {
        if (callContext.BranchId == Guid.Empty)
        {
            return """{"error":"BranchId is required."}""";
        }

        var to = DateOnly.FromDateTime(DateTime.UtcNow);
        var from = to.AddDays(-6);
        return await SendAsync(
            $"branches/{callContext.BranchId}/analytics/sales-range?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}",
            cancellationToken);
    }

    private async Task<string> SendAsync(string relativeUrl, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);

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
