// Built with help from Claude
namespace AgriTrack.Api.Models;

public class Expense
{
    public int Id { get; set; }

    public int FarmId { get; set; }
    public Farm? Farm { get; set; }

    // Optional: an expense can belong to one planting, or to the whole farm
    public int? PlantingId { get; set; }
    public Planting? Planting { get; set; }

    public string Category { get; set; } = ""; // Seeds, Fertilizer, Labor, Water...
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Description { get; set; }
}