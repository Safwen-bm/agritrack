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
public class IrrigationController(AppDbContext db) : ControllerBase
{
    [HttpGet("api/plantings/{plantingId}/irrigation")]
    public async Task<ActionResult<List<IrrigationResponse>>> GetForPlanting(int plantingId)
    {
        var userId = User.GetUserId();
        if (!await db.Plantings.AnyAsync(p => p.Id == plantingId && p.Field!.Farm!.OwnerId == userId))
            return NotFound();

        return await db.IrrigationLogs
            .Where(i => i.PlantingId == plantingId)
            .OrderByDescending(i => i.Date)
            .Select(i => new IrrigationResponse(i.Id, i.PlantingId, i.Date, i.VolumeM3))
            .ToListAsync();
    }

    [HttpPost("api/plantings/{plantingId}/irrigation")]
    public async Task<ActionResult<IrrigationResponse>> Create(int plantingId, IrrigationRequest request)
    {
        var userId = User.GetUserId();
        var planting = await db.Plantings
            .FirstOrDefaultAsync(p => p.Id == plantingId && p.Field!.Farm!.OwnerId == userId);
        if (planting == null) return NotFound();

        if (request.Date < planting.PlantingDate)
            return BadRequest(new { message = "Irrigation date cannot be before planting date" });

        var log = new IrrigationLog
        {
            PlantingId = plantingId,
            Date = request.Date,
            VolumeM3 = request.VolumeM3
        };
        db.IrrigationLogs.Add(log);
        await db.SaveChangesAsync();

        return Ok(new IrrigationResponse(log.Id, log.PlantingId, log.Date, log.VolumeM3));
    }

    [HttpPut("api/irrigation/{id}")]
    public async Task<ActionResult> Update(int id, IrrigationRequest request)
    {
        var userId = User.GetUserId();
        var log = await db.IrrigationLogs
            .Include(i => i.Planting)
            .FirstOrDefaultAsync(i => i.Id == id && i.Planting!.Field!.Farm!.OwnerId == userId);
        if (log == null) return NotFound();

        if (request.Date < log.Planting!.PlantingDate)
            return BadRequest(new { message = "Irrigation date cannot be before planting date" });

        log.Date = request.Date;
        log.VolumeM3 = request.VolumeM3;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("api/irrigation/{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var log = await db.IrrigationLogs
            .FirstOrDefaultAsync(i => i.Id == id && i.Planting!.Field!.Farm!.OwnerId == userId);
        if (log == null) return NotFound();

        db.IrrigationLogs.Remove(log);
        await db.SaveChangesAsync();
        return NoContent();
    }
}