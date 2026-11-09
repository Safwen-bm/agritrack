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
public class ExpensesController(AppDbContext db) : ControllerBase
{
    // Optional filter: /api/farms/1/expenses?plantingId=3
    [HttpGet("api/farms/{farmId}/expenses")]
    public async Task<ActionResult<List<ExpenseResponse>>> GetForFarm(int farmId, int? plantingId)
    {
        var userId = User.GetUserId();
        if (!await db.Farms.AnyAsync(f => f.Id == farmId && f.OwnerId == userId))
            return NotFound();

        var query = db.Expenses.Where(e => e.FarmId == farmId);
        if (plantingId != null)
            query = query.Where(e => e.PlantingId == plantingId);

        return await query
            .OrderByDescending(e => e.Date)
            .Select(e => new ExpenseResponse(
                e.Id, e.FarmId, e.PlantingId, e.Category, e.Amount, e.Date, e.Description))
            .ToListAsync();
    }

    [HttpPost("api/farms/{farmId}/expenses")]
    public async Task<ActionResult<ExpenseResponse>> Create(int farmId, ExpenseRequest request)
    {
        var userId = User.GetUserId();
        if (!await db.Farms.AnyAsync(f => f.Id == farmId && f.OwnerId == userId))
            return NotFound();

        // If linked to a planting, it must belong to this same farm
        if (request.PlantingId != null &&
            !await db.Plantings.AnyAsync(p => p.Id == request.PlantingId && p.Field!.FarmId == farmId))
            return BadRequest(new { message = "Planting does not belong to this farm" });

        var expense = new Expense
        {
            FarmId = farmId,
            PlantingId = request.PlantingId,
            Category = request.Category,
            Amount = request.Amount,
            Date = request.Date,
            Description = request.Description
        };
        db.Expenses.Add(expense);
        await db.SaveChangesAsync();

        return Ok(new ExpenseResponse(
            expense.Id, expense.FarmId, expense.PlantingId, expense.Category,
            expense.Amount, expense.Date, expense.Description));
    }

    [HttpPut("api/expenses/{id}")]
    public async Task<ActionResult> Update(int id, ExpenseRequest request)
    {
        var userId = User.GetUserId();
        var expense = await db.Expenses
            .FirstOrDefaultAsync(e => e.Id == id && e.Farm!.OwnerId == userId);
        if (expense == null) return NotFound();

        if (request.PlantingId != null &&
            !await db.Plantings.AnyAsync(p => p.Id == request.PlantingId && p.Field!.FarmId == expense.FarmId))
            return BadRequest(new { message = "Planting does not belong to this farm" });

        expense.PlantingId = request.PlantingId;
        expense.Category = request.Category;
        expense.Amount = request.Amount;
        expense.Date = request.Date;
        expense.Description = request.Description;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("api/expenses/{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var expense = await db.Expenses
            .FirstOrDefaultAsync(e => e.Id == id && e.Farm!.OwnerId == userId);
        if (expense == null) return NotFound();

        db.Expenses.Remove(expense);
        await db.SaveChangesAsync();
        return NoContent();
    }
}