// Built with help from Claude
using System.Globalization;

namespace AgriTrack.Api.Calculations;

public static class MonthlyAggregator
{
    // Sums values per calendar month, sorted oldest to newest. Month label: "2026-11"
    public static List<(string Month, decimal Total)> SumByMonth(
        IEnumerable<(DateOnly Date, decimal Value)> items)
    {
        return items
            .GroupBy(i => new DateOnly(i.Date.Year, i.Date.Month, 1))
            .OrderBy(g => g.Key)
            .Select(g => (
                g.Key.ToString("yyyy-MM", CultureInfo.InvariantCulture),
                Math.Round(g.Sum(i => i.Value), 2, MidpointRounding.AwayFromZero)))
            .ToList();
    }
}