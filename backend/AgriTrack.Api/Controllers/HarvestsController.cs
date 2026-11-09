// Built with help from Claude
using AgriTrack.Api.Data;
using AgriTrack.Api.Dtos;
using AgriTrack.Api.Extensions;
using AgriTrack.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriTrack.Api.Controllers;

[ApiController]
[Authorize]
public class HarvestsController(AppDbContext db) : ControllerBase
{
    [HttpGet("api/plantings/{plantingId}/harvests")]
    public async Task<ActionResult<List<HarvestResponse>>> GetForPlanting(int plantingId)
    {
        var userId = User.GetUserId();
        if (!await db.Plantings.AnyAsync(p => p.Id == plantingId && p.Field!.Farm!.OwnerId == userId))
            return NotFound();

        return await db.Harvests
            .Where(h => h.PlantingId == plantingId)
            .OrderByDescending(h => h.HarvestDate)
            .Select(h => new HarvestResponse(h.Id, h.PlantingId, h.HarvestDate, h.QuantityKg))
            .ToListAsync();
    }

    [HttpPost("api/plantings/{plantingId}/harvests")]
    public async Task<ActionResult<HarvestResponse>> Create(int plantingId, HarvestRequest request)
    {
        var userId = User.GetUserId();
        var planting = await db.Plantings
            .FirstOrDefaultAsync(p => p.Id == plantingId && p.Field!.Farm!.OwnerId == userId);
        if (planting == null) return NotFound();

        // Business rule: you can't harvest before planting
        if (request.HarvestDate < planting.PlantingDate)
            return BadRequest(new { message = "Harvest date cannot be before planting date" });

        var harvest = new Harvest
        {
            PlantingId = plantingId,
            HarvestDate = request.HarvestDate,
            QuantityKg = request.QuantityKg
        };
        db.Harvests.Add(harvest);
        await db.SaveChangesAsync();

        return Ok(new HarvestResponse(harvest.Id, harvest.PlantingId, harvest.HarvestDate, harvest.QuantityKg));
    }

    [HttpPut("api/harvests/{id}")]
    public async Task<ActionResult> Update(int id, HarvestRequest request)
    {
        var userId = User.GetUserId();
        var harvest = await db.Harvests
            .Include(h => h.Planting)
            .FirstOrDefaultAsync(h => h.Id == id && h.Planting!.Field!.Farm!.OwnerId == userId);
        if (harvest == null) return NotFound();

        if (request.HarvestDate < harvest.Planting!.PlantingDate)
            return BadRequest(new { message = "Harvest date cannot be before planting date" });

        harvest.HarvestDate = request.HarvestDate;
        harvest.QuantityKg = request.QuantityKg;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("api/harvests/{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var harvest = await db.Harvests
            .FirstOrDefaultAsync(h => h.Id == id && h.Planting!.Field!.Farm!.OwnerId == userId);
        if (harvest == null) return NotFound();

        db.Harvests.Remove(harvest);
        await db.SaveChangesAsync();
        return NoContent();
    }
}