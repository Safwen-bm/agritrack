// Built with help from Claude
namespace AgriTrack.Api.Models;

public class Planting
{
    public int Id { get; set; }

    public int FieldId { get; set; }
    public Field? Field { get; set; }

    public int CropTypeId { get; set; }
    public CropType? CropType { get; set; }

    public DateOnly PlantingDate { get; set; }
    public DateOnly? ExpectedHarvestDate { get; set; }
    public string Status { get; set; } = "Growing"; // Growing, Harvested

    public List<Harvest> Harvests { get; set; } = new();
    public List<IrrigationLog> IrrigationLogs { get; set; } = new();
    public List<Expense> Expenses { get; set; } = new();
    public List<Revenue> Revenues { get; set; } = new();
}