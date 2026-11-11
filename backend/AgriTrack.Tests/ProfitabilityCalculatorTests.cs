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