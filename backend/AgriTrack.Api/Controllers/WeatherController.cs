// Built with help from Claude
using AgriTrack.Api.Calculations;
using AgriTrack.Api.Data;
using AgriTrack.Api.Dtos;
using AgriTrack.Api.Extensions;
using AgriTrack.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriTrack.Api.Controllers;

[ApiController]
[Authorize]
public class WeatherController(AppDbContext db, WeatherService weather) : ControllerBase
{
    [HttpGet("api/fields/{id}/weather")]
    public async Task<ActionResult<WeatherResponse>> Get(int id)
    {
        var userId = User.GetUserId();

        var field = await db.Fields
            .Where(f => f.Id == id && f.Farm!.OwnerId == userId)
            .Select(f => new { f.Name, f.Latitude, f.Longitude })
            .FirstOrDefaultAsync();
        if (field == null) return NotFound();

        if (field.Latitude == null || field.Longitude == null)
            return BadRequest(new { message = "This field has no coordinates. Add latitude and longitude to see the forecast." });

        List<DailyForecast> forecast;
        try
        {
            forecast = await weather.GetForecastAsync(field.Latitude.Value, field.Longitude.Value);
        }
        catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException or TaskCanceledException)
        {
            return StatusCode(502, new { message = "The weather service is not available right now" });
        }

        // The scheduled tasks of growing plantings in this field, so warnings can mention them
        var plantings = await db.Plantings
            .Where(p => p.FieldId == id && p.Status == "Growing")
            .Select(p => new { p.PlantingDate, p.ExpectedHarvestDate, GrowthDays = p.CropType!.GrowthDays })
            .ToListAsync();

        var tasks = plantings
            .SelectMany(p => FarmTaskScheduler.Generate(
                p.PlantingDate,
                FarmTaskScheduler.CycleDays(p.PlantingDate, p.ExpectedHarvestDate, p.GrowthDays)))
            .ToList();

        return new WeatherResponse(field.Name, forecast, WeatherAdvisor.Analyze(forecast, tasks));
    }
}