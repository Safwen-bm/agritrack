// Built with help from Claude
using AgriTrack.Api.Calculations;

namespace AgriTrack.Api.Dtos;

public record UpcomingTask(
    DateOnly Date, string Type, string Title, string Stage,
    int PlantingId, string CropName, string FieldName);

public record FieldMapPoint(
    int Id, string Name, string FarmName, decimal AreaHectares,
    double Latitude, double Longitude, List<string> CurrentCrops);

public record WeatherResponse(
    string FieldName, List<DailyForecast> Forecast, List<WeatherWarning> Warnings);