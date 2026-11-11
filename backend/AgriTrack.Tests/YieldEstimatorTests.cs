using AgriTrack.Api.Calculations;
using Xunit;

namespace AgriTrack.Tests;

public class YieldEstimatorTests
{
    [Fact]
    public void No_history_uses_typical_yield()
    {
        var e = YieldEstimator.Estimate(2, 3500, new List<decimal>());

        Assert.Equal(3500m, e.EstimatedYieldKgPerHectare);
        Assert.Equal(7000m, e.EstimatedTotalKg);
        Assert.Equal(0m, e.HistoryWeight);
    }

    [Fact]
    public void One_past_harvest_has_weight_one_third()
    {
        // 1/3 * 4000 + 2/3 * 3500 = 3666.67 per ha
        var e = YieldEstimator.Estimate(2, 3500, new List<decimal> { 4000 });

        Assert.Equal(3666.67m, e.EstimatedYieldKgPerHectare);
        Assert.Equal(7333.33m, e.EstimatedTotalKg);
    }

    [Fact]
    public void Two_past_harvests_have_weight_one_half()
    {
        // history average 4000, typical 3500, weight 2/4 -> 3750 per ha
        var e = YieldEstimator.Estimate(2, 3500, new List<decimal> { 4500, 3500 });

        Assert.Equal(3750m, e.EstimatedYieldKgPerHectare);
        Assert.Equal(7500m, e.EstimatedTotalKg);
        Assert.Equal(0.5m, e.HistoryWeight);
    }

    [Fact]
    public void Comparison_shows_difference_and_percent()
    {
        var c = YieldEstimator.Compare(7500, 8250);

        Assert.Equal(750m, c.DifferenceKg);
        Assert.Equal(10m, c.DifferencePercent);
    }
}