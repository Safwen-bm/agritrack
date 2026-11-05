// Built with help from Claude
using AgriTrack.Api.Data;
using AgriTrack.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/crop-types")]
public class CropTypesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CropTypeResponse>>> GetAll()
    {
        return await db.CropTypes
            .OrderBy(c => c.Name)
            .Select(c => new CropTypeResponse(
                c.Id, c.Name, c.SeasonWaterNeedMm, c.GrowthDays, c.TypicalYieldKgPerHectare))
            .ToListAsync();
    }
}