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
public class FieldsController(AppDbContext db) : ControllerBase
{
    [HttpGet("api/farms/{farmId}/fields")]
    public async Task<ActionResult<List<FieldResponse>>> GetForFarm(int farmId)
    {
        var userId = User.GetUserId();

        // Make sure the farm exists AND belongs to this user
        if (!await db.Farms.AnyAsync(f => f.Id == farmId && f.OwnerId == userId))
            return NotFound();

        return await db.Fields
            .Where(f => f.FarmId == farmId)
            .Select(f => new FieldResponse(
                f.Id, f.FarmId, f.Name, f.AreaHectares, f.SoilType, f.Latitude, f.Longitude))
            .ToListAsync();
    }

    [HttpPost("api/farms/{farmId}/fields")]
    public async Task<ActionResult<FieldResponse>> Create(int farmId, FieldRequest request)
    {
        var userId = User.GetUserId();
        if (!await db.Farms.AnyAsync(f => f.Id == farmId && f.OwnerId == userId))
            return NotFound();

        var field = new Field
        {
            FarmId = farmId,
            Name = request.Name,
            AreaHectares = request.AreaHectares,
            SoilType = request.SoilType,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        db.Fields.Add(field);
        await db.SaveChangesAsync();

        var response = new FieldResponse(
            field.Id, field.FarmId, field.Name, field.AreaHectares,
            field.SoilType, field.Latitude, field.Longitude);
        return CreatedAtAction(nameof(GetOne), new { id = field.Id }, response);
    }

    [HttpGet("api/fields/{id}")]
    public async Task<ActionResult<FieldResponse>> GetOne(int id)
    {
        var userId = User.GetUserId();

        var field = await db.Fields
            .Where(f => f.Id == id && f.Farm!.OwnerId == userId)
            .Select(f => new FieldResponse(
                f.Id, f.FarmId, f.Name, f.AreaHectares, f.SoilType, f.Latitude, f.Longitude))
            .FirstOrDefaultAsync();

        if (field == null) return NotFound();
        return field;
    }

    [HttpPut("api/fields/{id}")]
    public async Task<ActionResult> Update(int id, FieldRequest request)
    {
        var userId = User.GetUserId();
        var field = await db.Fields
            .FirstOrDefaultAsync(f => f.Id == id && f.Farm!.OwnerId == userId);
        if (field == null) return NotFound();

        field.Name = request.Name;
        field.AreaHectares = request.AreaHectares;
        field.SoilType = request.SoilType;
        field.Latitude = request.Latitude;
        field.Longitude = request.Longitude;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("api/fields/{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = User.GetUserId();

        // Include expenses so EF can detach them from the plantings being deleted
        var field = await db.Fields
            .Include(f => f.Plantings).ThenInclude(p => p.Expenses)
            .FirstOrDefaultAsync(f => f.Id == id && f.Farm!.OwnerId == userId);
        if (field == null) return NotFound();

        db.Fields.Remove(field);
        await db.SaveChangesAsync();
        return NoContent();
    }
}