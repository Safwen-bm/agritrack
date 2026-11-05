// Built with help from Claude
using System.ComponentModel.DataAnnotations;

namespace AgriTrack.Api.Dtos;

public record FarmRequest(
    [Required, StringLength(100)] string Name,
    [StringLength(200)] string? Location);

public record FarmResponse(
    int Id, string Name, string? Location, int FieldCount, decimal TotalAreaHectares);

public record FieldRequest(
    [Required, StringLength(100)] string Name,
    [Range(0.01, 100000)] decimal AreaHectares,
    [StringLength(100)] string? SoilType,
    [Range(-90, 90)] double? Latitude,
    [Range(-180, 180)] double? Longitude);

public record FieldResponse(
    int Id, int FarmId, string Name, decimal AreaHectares,
    string? SoilType, double? Latitude, double? Longitude);

public record CropTypeResponse(
    int Id, string Name, decimal SeasonWaterNeedMm, int GrowthDays, decimal TypicalYieldKgPerHectare);

public record PlantingRequest(
    int CropTypeId,
    DateOnly PlantingDate,
    DateOnly? ExpectedHarvestDate,
    string? Status);

public record PlantingResponse(
    int Id, int FieldId, int CropTypeId, string CropName,
    DateOnly PlantingDate, DateOnly? ExpectedHarvestDate, string Status);