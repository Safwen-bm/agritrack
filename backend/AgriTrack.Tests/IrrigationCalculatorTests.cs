using AgriTrack.Api.Calculations;
using Xunit;

namespace AgriTrack.Tests;

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