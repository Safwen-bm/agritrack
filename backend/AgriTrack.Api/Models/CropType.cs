// Built with help from Claude
namespace AgriTrack.Api.Models;

public class CropType
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    // Assumptions used by the calculators
    public decimal SeasonWaterNeedMm { get; set; }
    public int GrowthDays { get; set; }
    public decimal TypicalYieldKgPerHectare { get; set; }
}