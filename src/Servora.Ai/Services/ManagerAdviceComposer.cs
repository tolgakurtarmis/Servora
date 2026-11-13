using System.Text.Json;
using Servora.Ai.Contracts;

namespace Servora.Ai.Services;

public static class ManagerAdviceComposer
{
    public static ManagerAdviceFacts? TryParseFacts(string salesRangeJson)
    {
        using var document = JsonDocument.Parse(salesRangeJson);
        var root = document.RootElement;
        if (root.TryGetProperty("error", out _))
        {
            return null;
        }

        var totalRevenue = root.GetProperty("totalRevenue").GetDecimal();
        var orderCount = root.GetProperty("orderCount").GetInt32();
        var fromDate = root.GetProperty("fromDate").GetString() ?? string.Empty;
        var toDate = root.GetProperty("toDate").GetString() ?? string.Empty;

        var dayCount = 7;
        if (root.TryGetProperty("days", out var days) && days.ValueKind == JsonValueKind.Array)
        {
            dayCount = Math.Max(days.GetArrayLength(), 1);
        }

        string? topName = null;
        decimal? topShare = null;
        if (root.TryGetProperty("topProducts", out var products)
            && products.ValueKind == JsonValueKind.Array
            && products.GetArrayLength() > 0)
        {
            var top = products[0];
            topName = top.GetProperty("name").GetString();
            var topRevenue = top.GetProperty("revenue").GetDecimal();
            if (totalRevenue > 0)
            {
                topShare = Math.Round(topRevenue / totalRevenue * 100m, 1);
            }
        }

        string? weakestDay = null;
        decimal? weakestRevenue = null;
        if (root.TryGetProperty("weakestDay", out var weak)
            && weak.ValueKind == JsonValueKind.Object)
        {
            weakestDay = weak.GetProperty("date").GetString();
            weakestRevenue = weak.GetProperty("totalRevenue").GetDecimal();
        }

        return new ManagerAdviceFacts(
            fromDate,
            toDate,
            totalRevenue,
            orderCount,
            Math.Round(totalRevenue / dayCount, 2),
            topName,
            topShare,
            weakestDay,
            weakestRevenue);
    }

    public static string BuildFactsBlock(ManagerAdviceFacts facts)
    {
        return
            $"""
            Dönem: {facts.FromDate} — {facts.ToDate}
            Toplam ciro: {facts.TotalRevenue} TRY
            Sipariş sayısı: {facts.OrderCount}
            Günlük ortalama ciro: {facts.AverageDailyRevenue} TRY
            En çok satan ürün: {(facts.TopProductName is null ? "yok" : $"{facts.TopProductName} (ciro payı %{facts.TopProductSharePercent})")}
            En zayıf gün: {(facts.WeakestDay is null ? "yok" : $"{facts.WeakestDay} ({facts.WeakestDayRevenue} TRY)")}
            """;
    }

    public static string BuildSystemPrompt()
    {
        return
            """
            Sen bir cafe şube müdürü danışmanısın. Görevin raporu tekrar etmek değil; yöneticiye uygulanabilir öneri vermek.
            Kurallar:
            - Sadece Türkçe.
            - Rakamları verilen gerçeklerden al; büyütme veya uydurma.
            - Özetleme yapma. Doğrudan 3 madde öneri yaz (1) (2) (3).
            - Her madde somut aksiyon olsun: kampanya, stok/hazırlık, personel, menü kombinasyonu, zayıf güne özel hamle.
            - "ciro X oldu" diye bitirme; "bu yüzden şunu yap" de.
            - İngilizce alan adı kullanma.
            """;
    }
}
