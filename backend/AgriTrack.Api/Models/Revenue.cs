// Built with help from Claude
namespace AgriTrack.Api.Models;

public class Revenue
{
    public int Id { get; set; }
    public int PlantingId { get; set; }
    public Planting? Planting { get; set; }

    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Description { get; set; }
}