// Built with help from Claude
namespace AgriTrack.Api.Models;

public class IrrigationLog
{
    public int Id { get; set; }
    public int PlantingId { get; set; }
    public Planting? Planting { get; set; }

    public DateOnly Date { get; set; }
    public decimal VolumeM3 { get; set; }
}