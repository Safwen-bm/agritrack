// Built with help from Claude
using AgriTrack.Api.Calculations;
using AgriTrack.Api.Data;
using AgriTrack.Api.Dtos;
using AgriTrack.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriTrack.Api.Controllers;

[ApiController]
[Authorize]
public class DashboardController(AppDbContext db) : ControllerBase
{
    private static decimal R(decimal v) => Math.Round(v, 2, MidpointRounding.AwayFromZero);

    [HttpGet("api/dashboard")]
    public async Task<ActionResult<DashboardResponse>> Get()
    {
        var userId = User.GetUserId();

        var farmCount = await db.Farms.CountAsync(f => f.OwnerId == userId);
        var fieldCount = await db.Fields.CountAsync(f => f.Farm!.OwnerId == userId);
        var area = await db.Fields
            .Where(f => f.Farm!.OwnerId == userId)
            .SumAsync(f => (decimal?)f.AreaHectares) ?? 0;

        // All expenses of this user, including farm-wide ones
        var expenses = await db.Expenses
            .Where(e => e.Farm!.OwnerId == userId)
            .Select(e => new { e.Category, e.Amount })
            .ToListAsync();

        // One row per planting with its totals
        var plantings = await db.Plantings
            .Where(p => p.Field!.Farm!.OwnerId == userId)
            .Select(p => new
            {
                CropName = p.CropType!.Name,
                p.PlantingDate,
                FieldName = p.Field!.Name,
                Area = p.Field!.AreaHectares,
                Expenses = p.Expenses.Sum(e => (decimal?)e.Amount) ?? 0,
                Revenue = p.Revenues.Sum(r => (decimal?)r.Amount) ?? 0,
                HarvestKg = p.Harvests.Sum(h => (decimal?)h.QuantityKg) ?? 0
            })
            .ToListAsync();

        var irrigation = await db.IrrigationLogs
            .Where(i => i.Planting!.Field!.Farm!.OwnerId == userId)
            .Select(i => new { i.Date, i.VolumeM3 })
            .ToListAsync();

        var totalExpenses = expenses.Sum(e => e.Amount);
        var totalRevenue = plantings.Sum(p => p.Revenue);

        var summary = new DashboardSummary(
            farmCount, fieldCount, R(area),
            R(totalExpenses), R(totalRevenue), R(totalRevenue - totalExpenses),
            R(irrigation.Sum(i => i.VolumeM3)), R(plantings.Sum(p => p.HarvestKg)));

        var profitByCrop = plantings
            .GroupBy(p => p.CropName)
            .Select(g => new ProfitGroup(
                g.Key, R(g.Sum(p => p.Expenses)), R(g.Sum(p => p.Revenue)),
                R(g.Sum(p => p.Revenue - p.Expenses))))
            .OrderBy(x => x.Label)
            .ToList();

        // A "season" is the year of planting
        var profitBySeason = plantings
            .GroupBy(p => p.PlantingDate.Year)
            .OrderBy(g => g.Key)
            .Select(g => new ProfitGroup(
                g.Key.ToString(), R(g.Sum(p => p.Expenses)), R(g.Sum(p => p.Revenue)),
                R(g.Sum(p => p.Revenue - p.Expenses))))
            .ToList();

        // "Seeds" and "seeds" count as the same category
        var expensesByCategory = expenses
            .GroupBy(e => e.Category.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => new CategoryTotal(g.Key, R(g.Sum(e => e.Amount))))
            .OrderByDescending(c => c.Amount)
            .ToList();

        var waterByMonth = MonthlyAggregator
            .SumByMonth(irrigation.Select(i => (i.Date, i.VolumeM3)))
            .Select(m => new MonthTotal(m.Month, m.Total))
            .ToList();

        // Only plantings that have a harvest can show a yield
        var yields = plantings
            .Where(p => p.HarvestKg > 0)
            .OrderBy(p => p.PlantingDate)
            .Select(p => new YieldPoint(
                $"{p.CropName} {p.PlantingDate.Year} ({p.FieldName})",
                R(p.HarvestKg / p.Area), R(p.HarvestKg)))
            .ToList();

        return new DashboardResponse(
            summary, profitByCrop, profitBySeason, expensesByCategory, waterByMonth, yields);
    }
}