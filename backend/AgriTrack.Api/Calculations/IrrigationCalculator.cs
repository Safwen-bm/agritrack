// Built with help from Claude
namespace AgriTrack.Api.Calculations;

public record IrrigationEstimate(
    decimal NetRequirementMm,
    decimal RecommendedVolumeM3,
    decimal AppliedVolumeM3,
    decimal DifferenceM3); // applied - recommended: positive = over-irrigated

public static class IrrigationCalculator
{
    // 1 mm of water on 1 hectare = 10 cubic meters
    private const decimal M3PerMmPerHectare = 10m;

    public static IrrigationEstimate Calculate(
        decimal seasonWaterNeedMm,
        decimal effectiveRainfallMm,
        decimal areaHectares,
        decimal efficiency,       // 0 < efficiency <= 1
        decimal appliedVolumeM3)
    {
        if (seasonWaterNeedMm < 0 || effectiveRainfallMm < 0 || appliedVolumeM3 < 0)
            throw new ArgumentOutOfRangeException(nameof(seasonWaterNeedMm), "Values cannot be negative");
        if (areaHectares <= 0)
            throw new ArgumentOutOfRangeException(nameof(areaHectares), "Area must be positive");
        if (efficiency <= 0 || efficiency > 1)
            throw new ArgumentOutOfRangeException(nameof(efficiency), "Efficiency must be between 0 and 1");

        // Rain that already covers the need means no irrigation (never negative)
        var netMm = Math.Max(0, seasonWaterNeedMm - effectiveRainfallMm);

        // Divide by efficiency because some water is lost before reaching the roots
        var recommended = netMm * M3PerMmPerHectare * areaHectares / efficiency;

        return new IrrigationEstimate(
            Round(netMm), Round(recommended), Round(appliedVolumeM3),
            Round(appliedVolumeM3 - recommended));
    }

    private static decimal Round(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}