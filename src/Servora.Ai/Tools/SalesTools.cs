namespace Servora.Ai.Tools;

/// <summary>
/// Mock sales tools for the AI skeleton. Later these will call Servora.Api.
/// </summary>
public static class SalesTools
{
    /// <summary>
    /// Returns a mock daily sales summary for the given date (yyyy-MM-dd).
    /// </summary>
    public static object GetDailySalesSummary(string date)
    {
        return new
        {
            date,
            currency = "TRY",
            totalRevenue = 18450.00m,
            orderCount = 86,
            topProducts =
            new[]
            {
                new { name = "Latte", quantity = 42, revenue = 5040.00m },
                new { name = "Cheesecake", quantity = 28, revenue = 4480.00m },
                new { name = "Cold Brew", quantity = 35, revenue = 4200.00m }
            },
            source = "mock"
        };
    }
}
