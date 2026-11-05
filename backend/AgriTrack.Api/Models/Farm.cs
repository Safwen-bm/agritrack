// Built with help from Claude
namespace AgriTrack.Api.Models;

public class Farm
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Location { get; set; }

    // Every farm belongs to exactly one user
    public string OwnerId { get; set; } = "";
    public ApplicationUser? Owner { get; set; }

    public List<Field> Fields { get; set; } = new();
}