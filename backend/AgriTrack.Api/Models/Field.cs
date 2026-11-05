// Built with help from Claude
namespace AgriTrack.Api.Models;

public class Field
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal AreaHectares { get; set; }
    public string? SoilType { get; set; }

    // Used later for the map
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public int FarmId { get; set; }
    public Farm? Farm { get; set; }

    public List<Planting> Plantings { get; set; } = new();
}