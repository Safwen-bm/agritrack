// Built with help from Claude
namespace AgriTrack.Api.Calculations;

public record ProfitabilityResult(
    decimal TotalExpenses,
    decimal TotalRevenue,
    decimal NetProfit,
    decimal TotalHarvestKg,
    decimal CostPerHectare,
    decimal? CostPerKg,          // null when nothing was harvested
    decimal? ProfitMarginPercent); // null when there is no revenue

public static class ProfitabilityCalculator
{
    public static ProfitabilityResult Calculate(
        decimal areaHectares, decimal totalExpenses, decimal totalRevenue, decimal totalHarvestKg)
    {
        if (areaHectares <= 0)
            throw new ArgumentOutOfRangeException(nameof(areaHectares), "Area must be positive");
        if (totalExpenses < 0 || totalRevenue < 0 || totalHarvestKg < 0)
            throw new ArgumentOutOfRangeException(nameof(totalExpenses), "Values cannot be negative");

        var netProfit = totalRevenue - totalExpenses;

        // Dividing by zero would be meaningless, so we return null instead
        decimal? costPerKg = totalHarvestKg > 0 ? Round(totalExpenses / totalHarvestKg) : null;
        decimal? margin = totalRevenue > 0 ? Round(netProfit / totalRevenue * 100) : null;

        return new ProfitabilityResult(
            Round(totalExpenses), Round(totalRevenue), Round(netProfit), Round(totalHarvestKg),
            Round(totalExpenses / areaHectares), costPerKg, margin);
    }

    private static decimal Round(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}