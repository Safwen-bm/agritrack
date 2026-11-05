// Built with help from Claude
namespace AgriTrack.Api.Models;

public class Harvest
{
    public int Id { get; set; }
    public int PlantingId { get; set; }
    public Planting? Planting { get; set; }

    public DateOnly HarvestDate { get; set; }
    public decimal QuantityKg { get; set; }
}