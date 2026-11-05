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
[Route("api/farms")]
public class FarmsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<FarmResponse>>> GetAll()
    {
        var userId = User.GetUserId();

        // Only this user's farms
        var farms = await db.Farms
            .Where(f => f.OwnerId == userId)
            .Select(f => new FarmResponse(
                f.Id, f.Name, f.Location,
                f.Fields.Count,
                f.Fields.Sum(x => (decimal?)x.AreaHectares) ?? 0))
            .ToListAsync();

        return farms;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FarmResponse>> GetOne(int id)
    {
        var userId = User.GetUserId();

        var farm = await db.Farms
            .Where(f => f.Id == id && f.OwnerId == userId)
            .Select(f => new FarmResponse(
                f.Id, f.Name, f.Location,
                f.Fields.Count,
                f.Fields.Sum(x => (decimal?)x.AreaHectares) ?? 0))
            .FirstOrDefaultAsync();

        if (farm == null) return NotFound();
        return farm;
    }

    [HttpPost]
    public async Task<ActionResult<FarmResponse>> Create(FarmRequest request)
    {
        var farm = new Farm
        {
            Name = request.Name,
            Location = request.Location,
            OwnerId = User.GetUserId() // always the logged-in user, never from the request
        };

        db.Farms.Add(farm);
        await db.SaveChangesAsync();

        var response = new FarmResponse(farm.Id, farm.Name, farm.Location, 0, 0);
        return CreatedAtAction(nameof(GetOne), new { id = farm.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, FarmRequest request)
    {
        var userId = User.GetUserId();
        var farm = await db.Farms.FirstOrDefaultAsync(f => f.Id == id && f.OwnerId == userId);
        if (farm == null) return NotFound();

        farm.Name = request.Name;
        farm.Location = request.Location;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var farm = await db.Farms.FirstOrDefaultAsync(f => f.Id == id && f.OwnerId == userId);
        if (farm == null) return NotFound();

        // Fields, plantings, harvests, etc. are deleted too (cascade)
        db.Farms.Remove(farm);
        await db.SaveChangesAsync();
        return NoContent();
    }
}