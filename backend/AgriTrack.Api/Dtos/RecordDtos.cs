// Built with help from Claude
using System.ComponentModel.DataAnnotations;

namespace AgriTrack.Api.Dtos;

public record HarvestRequest(
    DateOnly HarvestDate,
    [Range(0.01, 100000000)] decimal QuantityKg);

public record HarvestResponse(int Id, int PlantingId, DateOnly HarvestDate, decimal QuantityKg);

public record IrrigationRequest(
    DateOnly Date,
    [Range(0.01, 100000000)] decimal VolumeM3);

public record IrrigationResponse(int Id, int PlantingId, DateOnly Date, decimal VolumeM3);

public record RevenueRequest(
    DateOnly Date,
    [Range(0.01, 100000000)] decimal Amount,
    [StringLength(200)] string? Description);

public record RevenueResponse(int Id, int PlantingId, DateOnly Date, decimal Amount, string? Description);

public record ExpenseRequest(
    int? PlantingId,
    [Required, StringLength(50)] string Category,
    [Range(0.01, 100000000)] decimal Amount,
    DateOnly Date,
    [StringLength(200)] string? Description);

public record ExpenseResponse(
    int Id, int FarmId, int? PlantingId, string Category,
    decimal Amount, DateOnly Date, string? Description);