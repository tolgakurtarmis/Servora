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
            return """{"error":"BranchId is required on the complete request."}""";
        }

        // CreateDate in Api is UTC; "today" must use the UTC calendar day.
        var resolvedDate = string.IsNullOrWhiteSpace(date)
            ? DateOnly.FromDateTime(DateTime.UtcNow)
            : DateOnly.TryParse(date, out var parsed)
                ? parsed
                : (DateOnly?)null;

        if (resolvedDate is null)
        {
            return """{"error":"date must be yyyy-MM-dd when provided."}""";
        }

        var dateValue = resolvedDate.Value.ToString("yyyy-MM-dd");
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"branches/{callContext.BranchId}/analytics/daily-sales?date={dateValue}");

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
