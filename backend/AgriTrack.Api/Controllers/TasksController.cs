// Built with help from Claude
using AgriTrack.Api.Calculations;
using AgriTrack.Api.Data;
using AgriTrack.Api.Dtos;
using AgriTrack.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriTrack.Api.Controllers;

[ApiController]
[Authorize]
public class TasksController(AppDbContext db) : ControllerBase
{
    // Example: /api/tasks?days=14
    [HttpGet("api/tasks")]
    public async Task<ActionResult<List<UpcomingTask>>> Upcoming(int days = 30)
    {
        if (days < 1 || days > 365)
            return BadRequest(new { message = "Days must be between 1 and 365" });

        var userId = User.GetUserId();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var end = today.AddDays(days);

        // Only plantings that are still growing get suggestions
        var plantings = await db.Plantings
            .Where(p => p.Status == "Growing" && p.Field!.Farm!.OwnerId == userId)
            .Select(p => new
            {
                p.Id,
                CropName = p.CropType!.Name,
                FieldName = p.Field!.Name,
                p.PlantingDate,
                p.ExpectedHarvestDate,
                GrowthDays = p.CropType!.GrowthDays
            })
            .ToListAsync();

        var result = new List<UpcomingTask>();
        foreach (var p in plantings)
        {
            var cycle = FarmTaskScheduler.CycleDays(p.PlantingDate, p.ExpectedHarvestDate, p.GrowthDays);
            foreach (var t in FarmTaskScheduler.Generate(p.PlantingDate, cycle))
            {
                if (t.Date >= today && t.Date <= end)
                    result.Add(new UpcomingTask(t.Date, t.Type, t.Title, t.Stage, p.Id, p.CropName, p.FieldName));
            }
        }

        return result.OrderBy(t => t.Date).ThenBy(t => t.CropName).ToList();
    }
}