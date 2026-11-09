// Built with help from Claude
using AgriTrack.Api.Calculations;
using Xunit;

namespace AgriTrack.Tests;

public class ProfitabilityCalculatorTests
{
    [Fact]
    public void Profit_case_is_calculated_correctly()
    {
        // 2 ha, spent 4000, earned 5000, harvested 10000 kg
        var r = ProfitabilityCalculator.Calculate(2, 4000, 5000, 10000);

        Assert.Equal(1000m, r.NetProfit);
        Assert.Equal(2000m, r.CostPerHectare);   // 4000 / 2
        Assert.Equal(0.40m, r.CostPerKg);        // 4000 / 10000
        Assert.Equal(20m, r.ProfitMarginPercent); // 1000 / 5000 * 100
    }

    [Fact]
    public void Loss_gives_negative_profit_and_margin()
    {
        var r = ProfitabilityCalculator.Calculate(2, 6000, 5000, 10000);

        Assert.Equal(-1000m, r.NetProfit);
        Assert.Equal(-20m, r.ProfitMarginPercent);
    }

    [Fact]
    public void No_harvest_means_no_cost_per_kg()
    {
        var r = ProfitabilityCalculator.Calculate(2, 4000, 0, 0);

        Assert.Null(r.CostPerKg);
        Assert.Null(r.ProfitMarginPercent);
    }

    [Fact]
    public void Zero_area_is_rejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => ProfitabilityCalculator.Calculate(0, 100, 100, 100));
    }
}

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

public class IrrigationCalculatorTests
{
    [Fact]
    public void Recommended_volume_accounts_for_rain_and_efficiency()
    {
        // need 500 mm, rain 100 mm -> 400 mm net
        // 400 * 10 * 2 ha = 8000 m3, / 0.8 efficiency = 10000 m3
        var r = IrrigationCalculator.Calculate(500, 100, 2, 0.8m, 12000);

        Assert.Equal(400m, r.NetRequirementMm);
        Assert.Equal(10000m, r.RecommendedVolumeM3);
        Assert.Equal(2000m, r.DifferenceM3); // 2000 m3 over-irrigated
    }

    [Fact]
    public void Heavy_rain_means_no_irrigation_needed()
    {
        var r = IrrigationCalculator.Calculate(450, 600, 2, 0.75m, 0);

        Assert.Equal(0m, r.NetRequirementMm);
        Assert.Equal(0m, r.RecommendedVolumeM3);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.5)]
    [InlineData(1.5)]
    public void Invalid_efficiency_is_rejected(double efficiency)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => IrrigationCalculator.Calculate(500, 0, 2, (decimal)efficiency, 0));
    }
}

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