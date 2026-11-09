// Built with help from Claude
using AgriTrack.Api.Calculations;
using AgriTrack.Api.Data;
using AgriTrack.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriTrack.Api.Controllers;

[ApiController]
[Authorize]
public class AnalyticsController(AppDbContext db) : ControllerBase
{
    [HttpGet("api/plantings/{id}/profitability")]
    public async Task<ActionResult<ProfitabilityResult>> Profitability(int id)
    {
        var userId = User.GetUserId();

        var data = await db.Plantings
            .Where(p => p.Id == id && p.Field!.Farm!.OwnerId == userId)
            .Select(p => new
            {
                Area = p.Field!.AreaHectares,
                Expenses = p.Expenses.Sum(e => (decimal?)e.Amount) ?? 0,
                Revenue = p.Revenues.Sum(r => (decimal?)r.Amount) ?? 0,
                HarvestKg = p.Harvests.Sum(h => (decimal?)h.QuantityKg) ?? 0
            })
            .FirstOrDefaultAsync();

        if (data == null) return NotFound();

        return ProfitabilityCalculator.Calculate(
            data.Area, data.Expenses, data.Revenue, data.HarvestKg);
    }

    [HttpGet("api/plantings/{id}/yield-estimate")]
    public async Task<ActionResult> YieldEstimate(int id)
    {
        var userId = User.GetUserId();

        var planting = await db.Plantings
            .Where(p => p.Id == id && p.Field!.Farm!.OwnerId == userId)
            .Select(p => new
            {
                p.CropTypeId,
                Area = p.Field!.AreaHectares,
                Typical = p.CropType!.TypicalYieldKgPerHectare,
                ActualKg = p.Harvests.Sum(h => (decimal?)h.QuantityKg) ?? 0
            })
            .FirstOrDefaultAsync();

        if (planting == null) return NotFound();

        // History: this user's OTHER plantings of the same crop that have harvests
        var past = await db.Plantings
            .Where(p => p.CropTypeId == planting.CropTypeId
                        && p.Id != id
                        && p.Field!.Farm!.OwnerId == userId
                        && p.Harvests.Any())
            .Select(p => new
            {
                Kg = p.Harvests.Sum(h => h.QuantityKg),
                Area = p.Field!.AreaHectares
            })
            .ToListAsync();

        var historicalYields = past.Select(h => h.Kg / h.Area).ToList();

        var estimate = YieldEstimator.Estimate(planting.Area, planting.Typical, historicalYields);
        var comparison = YieldEstimator.Compare(estimate.EstimatedTotalKg, planting.ActualKg);

        return Ok(new { estimate, actualKg = planting.ActualKg, comparison });
    }

    // Example: /api/plantings/1/irrigation-estimate?rainfallMm=100&efficiency=0.9
    [HttpGet("api/plantings/{id}/irrigation-estimate")]
    public async Task<ActionResult<IrrigationEstimate>> IrrigationEstimate(
        int id, decimal rainfallMm = 0, decimal efficiency = 0.75m)
    {
        if (rainfallMm < 0)
            return BadRequest(new { message = "Rainfall cannot be negative" });
        if (efficiency <= 0 || efficiency > 1)
            return BadRequest(new { message = "Efficiency must be between 0 and 1" });

        var userId = User.GetUserId();

        var data = await db.Plantings
            .Where(p => p.Id == id && p.Field!.Farm!.OwnerId == userId)
            .Select(p => new
            {
                Need = p.CropType!.SeasonWaterNeedMm,
                Area = p.Field!.AreaHectares,
                Applied = p.IrrigationLogs.Sum(i => (decimal?)i.VolumeM3) ?? 0
            })
            .FirstOrDefaultAsync();

        if (data == null) return NotFound();

        return IrrigationCalculator.Calculate(
            data.Need, rainfallMm, data.Area, efficiency, data.Applied);
    }
}