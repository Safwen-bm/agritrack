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
public class RevenuesController(AppDbContext db) : ControllerBase
{
    [HttpGet("api/plantings/{plantingId}/revenues")]
    public async Task<ActionResult<List<RevenueResponse>>> GetForPlanting(int plantingId)
    {
        var userId = User.GetUserId();
        if (!await db.Plantings.AnyAsync(p => p.Id == plantingId && p.Field!.Farm!.OwnerId == userId))
            return NotFound();

        return await db.Revenues
            .Where(r => r.PlantingId == plantingId)
            .OrderByDescending(r => r.Date)
            .Select(r => new RevenueResponse(r.Id, r.PlantingId, r.Date, r.Amount, r.Description))
            .ToListAsync();
    }

    [HttpPost("api/plantings/{plantingId}/revenues")]
    public async Task<ActionResult<RevenueResponse>> Create(int plantingId, RevenueRequest request)
    {
        var userId = User.GetUserId();
        var planting = await db.Plantings
            .FirstOrDefaultAsync(p => p.Id == plantingId && p.Field!.Farm!.OwnerId == userId);
        if (planting == null) return NotFound();

        if (request.Date < planting.PlantingDate)
            return BadRequest(new { message = "Revenue date cannot be before planting date" });

        var revenue = new Revenue
        {
            PlantingId = plantingId,
            Date = request.Date,
            Amount = request.Amount,
            Description = request.Description
        };
        db.Revenues.Add(revenue);
        await db.SaveChangesAsync();

        return Ok(new RevenueResponse(revenue.Id, revenue.PlantingId, revenue.Date, revenue.Amount, revenue.Description));
    }

    [HttpPut("api/revenues/{id}")]
    public async Task<ActionResult> Update(int id, RevenueRequest request)
    {
        var userId = User.GetUserId();
        var revenue = await db.Revenues
            .Include(r => r.Planting)
            .FirstOrDefaultAsync(r => r.Id == id && r.Planting!.Field!.Farm!.OwnerId == userId);
        if (revenue == null) return NotFound();

        if (request.Date < revenue.Planting!.PlantingDate)
            return BadRequest(new { message = "Revenue date cannot be before planting date" });

        revenue.Date = request.Date;
        revenue.Amount = request.Amount;
        revenue.Description = request.Description;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("api/revenues/{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var revenue = await db.Revenues
            .FirstOrDefaultAsync(r => r.Id == id && r.Planting!.Field!.Farm!.OwnerId == userId);
        if (revenue == null) return NotFound();

        db.Revenues.Remove(revenue);
        await db.SaveChangesAsync();
        return NoContent();
    }
}