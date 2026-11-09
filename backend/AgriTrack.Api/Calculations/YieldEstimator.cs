// Built with help from Claude
namespace AgriTrack.Api.Calculations;

public record YieldEstimate(
    decimal EstimatedYieldKgPerHectare,
    decimal EstimatedTotalKg,
    int HistoryCount,
    decimal HistoryWeight); // how much we trusted the farmer's own history (0 to 1)

public record YieldComparison(decimal DifferenceKg, decimal? DifferencePercent);

public static class YieldEstimator
{
    public static YieldEstimate Estimate(
        decimal areaHectares,
        decimal typicalYieldKgPerHectare,
        IReadOnlyList<decimal> historicalYieldsKgPerHectare)
    {
        if (areaHectares <= 0)
            throw new ArgumentOutOfRangeException(nameof(areaHectares), "Area must be positive");
        if (typicalYieldKgPerHectare < 0)
            throw new ArgumentOutOfRangeException(nameof(typicalYieldKgPerHectare));

        var n = historicalYieldsKgPerHectare.Count;
        decimal weight = 0;
        var perHectare = typicalYieldKgPerHectare;

        if (n > 0)
        {
            // The more past harvests we have, the more we trust them: n / (n + 2)
            weight = (decimal)n / (n + 2);
            var average = historicalYieldsKgPerHectare.Average();
            perHectare = weight * average + (1 - weight) * typicalYieldKgPerHectare;
        }

        return new YieldEstimate(
            Round(perHectare), Round(perHectare * areaHectares), n, Math.Round(weight, 4));
    }

    // Compares the estimate with what was actually harvested
    public static YieldComparison Compare(decimal estimatedKg, decimal actualKg)
    {
        var difference = actualKg - estimatedKg;
        decimal? percent = estimatedKg > 0 ? Round(difference / estimatedKg * 100) : null;
        return new YieldComparison(Round(difference), percent);
    }

    private static decimal Round(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}