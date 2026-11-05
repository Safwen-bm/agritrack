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
public class PlantingsController(AppDbContext db) : ControllerBase
{
    private static readonly string[] ValidStatuses = { "Growing", "Harvested" };

    [HttpGet("api/fields/{fieldId}/plantings")]
    public async Task<ActionResult<List<PlantingResponse>>> GetForField(int fieldId)
    {
        var userId = User.GetUserId();
        if (!await db.Fields.AnyAsync(f => f.Id == fieldId && f.Farm!.OwnerId == userId))
            return NotFound();

        return await db.Plantings
            .Where(p => p.FieldId == fieldId)
            .OrderByDescending(p => p.PlantingDate)
            .Select(p => new PlantingResponse(
                p.Id, p.FieldId, p.CropTypeId, p.CropType!.Name,
                p.PlantingDate, p.ExpectedHarvestDate, p.Status))
            .ToListAsync();
    }

    [HttpPost("api/fields/{fieldId}/plantings")]
    public async Task<ActionResult<PlantingResponse>> Create(int fieldId, PlantingRequest request)
    {
        var userId = User.GetUserId();
        if (!await db.Fields.AnyAsync(f => f.Id == fieldId && f.Farm!.OwnerId == userId))
            return NotFound();

        var crop = await db.CropTypes.FindAsync(request.CropTypeId);
        if (crop == null) return BadRequest(new { message = "Unknown crop type" });

        var status = request.Status ?? "Growing";
        if (!ValidStatuses.Contains(status))
            return BadRequest(new { message = "Status must be Growing or Harvested" });

        // Business rule: default harvest date = planting date + crop growth days
        var expected = request.ExpectedHarvestDate
            ?? request.PlantingDate.AddDays(crop.GrowthDays);
        if (expected < request.PlantingDate)
            return BadRequest(new { message = "Harvest date cannot be before planting date" });

        var planting = new Planting
        {
            FieldId = fieldId,
            CropTypeId = crop.Id,
            PlantingDate = request.PlantingDate,
            ExpectedHarvestDate = expected,
            Status = status
        };

        db.Plantings.Add(planting);
        await db.SaveChangesAsync();

        var response = new PlantingResponse(
            planting.Id, planting.FieldId, crop.Id, crop.Name,
            planting.PlantingDate, planting.ExpectedHarvestDate, planting.Status);
        return CreatedAtAction(nameof(GetOne), new { id = planting.Id }, response);
    }

    [HttpGet("api/plantings/{id}")]
    public async Task<ActionResult<PlantingResponse>> GetOne(int id)
    {
        var userId = User.GetUserId();

        var planting = await db.Plantings
            .Where(p => p.Id == id && p.Field!.Farm!.OwnerId == userId)
            .Select(p => new PlantingResponse(
                p.Id, p.FieldId, p.CropTypeId, p.CropType!.Name,
                p.PlantingDate, p.ExpectedHarvestDate, p.Status))
            .FirstOrDefaultAsync();

        if (planting == null) return NotFound();
        return planting;
    }

    [HttpPut("api/plantings/{id}")]
    public async Task<ActionResult> Update(int id, PlantingRequest request)
    {
        var userId = User.GetUserId();
        var planting = await db.Plantings
            .FirstOrDefaultAsync(p => p.Id == id && p.Field!.Farm!.OwnerId == userId);
        if (planting == null) return NotFound();

        var crop = await db.CropTypes.FindAsync(request.CropTypeId);
        if (crop == null) return BadRequest(new { message = "Unknown crop type" });

        var status = request.Status ?? planting.Status;
        if (!ValidStatuses.Contains(status))
            return BadRequest(new { message = "Status must be Growing or Harvested" });

        var expected = request.ExpectedHarvestDate
            ?? request.PlantingDate.AddDays(crop.GrowthDays);
        if (expected < request.PlantingDate)
            return BadRequest(new { message = "Harvest date cannot be before planting date" });

        planting.CropTypeId = crop.Id;
        planting.PlantingDate = request.PlantingDate;
        planting.ExpectedHarvestDate = expected;
        planting.Status = status;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("api/plantings/{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = User.GetUserId();

        // Include expenses so EF can detach them (an expense may outlive a planting)
        var planting = await db.Plantings
            .Include(p => p.Expenses)
            .FirstOrDefaultAsync(p => p.Id == id && p.Field!.Farm!.OwnerId == userId);
        if (planting == null) return NotFound();

        db.Plantings.Remove(planting);
        await db.SaveChangesAsync();
        return NoContent();
    }
}