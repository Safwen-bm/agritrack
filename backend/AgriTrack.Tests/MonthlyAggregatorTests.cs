using AgriTrack.Api.Calculations;
using Xunit;

namespace AgriTrack.Tests;

public class MonthlyAggregatorTests
{
    [Fact]
    public void Sums_values_by_month_in_order()
    {
        var items = new List<(DateOnly, decimal)>
        {
            (new DateOnly(2026, 12, 5), 10m),
            (new DateOnly(2026, 11, 20), 5m),
            (new DateOnly(2026, 11, 2), 7.5m)
        };

        var result = MonthlyAggregator.SumByMonth(items);

        Assert.Equal(2, result.Count);
        Assert.Equal("2026-11", result[0].Month);
        Assert.Equal(12.5m, result[0].Total);
        Assert.Equal("2026-12", result[1].Month);
        Assert.Equal(10m, result[1].Total);
    }

    [Fact]
    public void No_items_gives_empty_list()
    {
        Assert.Empty(MonthlyAggregator.SumByMonth(new List<(DateOnly, decimal)>()));
    }
}