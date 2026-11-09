// Built with help from Claude
namespace AgriTrack.Api.Dtos;

public record DashboardSummary(
    int FarmCount, int FieldCount, decimal TotalAreaHectares,
    decimal TotalExpenses, decimal TotalRevenue, decimal NetProfit,
    decimal TotalWaterM3, decimal TotalHarvestKg);

public record ProfitGroup(string Label, decimal Expenses, decimal Revenue, decimal Profit);
public record CategoryTotal(string Category, decimal Amount);
public record MonthTotal(string Month, decimal VolumeM3);
public record YieldPoint(string Label, decimal YieldKgPerHectare, decimal TotalKg);

public record DashboardResponse(
    DashboardSummary Summary,
    List<ProfitGroup> ProfitByCrop,
    List<ProfitGroup> ProfitBySeason,
    List<CategoryTotal> ExpensesByCategory,
    List<MonthTotal> WaterByMonth,
    List<YieldPoint> Yields);