// Built with help from Claude
using AgriTrack.Api.Data;
using AgriTrack.Api.Dtos;
using AgriTrack.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriTrack.Api.Controllers;

[ApiController]
[Authorize]
public class MapController(AppDbContext db) : ControllerBase
{
    // All of this user's fields that have coordinates
    [HttpGet("api/map/fields")]
    public async Task<ActionResult<List<FieldMapPoint>>> Fields()
    {
        var userId = User.GetUserId();

        var rows = await db.Fields
            .Where(f => f.Farm!.OwnerId == userId && f.Latitude != null && f.Longitude != null)
            .Select(f => new
            {
                f.Id,
                f.Name,
                FarmName = f.Farm!.Name,
                f.AreaHectares,
                Latitude = f.Latitude!.Value,
                Longitude = f.Longitude!.Value,
                Crops = f.Plantings
                    .Where(p => p.Status == "Growing")
                    .Select(p => p.CropType!.Name)
                    .ToList()
            })
            .ToListAsync();

        return rows
            .Select(r => new FieldMapPoint(
                r.Id, r.Name, r.FarmName, r.AreaHectares,
                r.Latitude, r.Longitude, r.Crops.Distinct().ToList()))
            .ToList();
    }
}