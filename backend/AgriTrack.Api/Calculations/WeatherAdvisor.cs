// Built with help from Claude
using System.Globalization;

namespace AgriTrack.Api.Calculations;

public record DailyForecast(
    DateOnly Date, decimal TempMaxC, decimal TempMinC, decimal PrecipitationMm, decimal WindMaxKmh);

public record WeatherWarning(DateOnly Date, string Severity, string Message);

public static class WeatherAdvisor
{
    // Thresholds (assumptions, documented in the README)
    public const decimal FrostTempC = 2m;
    public const decimal HeatTempC = 38m;
    public const decimal RainDelaysIrrigationMm = 5m;
    public const decimal HeavyRainMm = 10m;
    public const decimal StrongWindKmh = 40m;

    // Format numbers the same way on every computer (no decimal commas)
    private static string F(decimal v) => v.ToString("0.#", CultureInfo.InvariantCulture);

    public static List<WeatherWarning> Analyze(
        IReadOnlyList<DailyForecast> forecast, IReadOnlyList<SuggestedTask> tasks)
    {
        var warnings = new List<WeatherWarning>();

        foreach (var day in forecast)
        {
            // Warnings that apply to the whole field
            if (day.TempMinC <= FrostTempC)
                warnings.Add(new WeatherWarning(day.Date, "High",
                    $"Frost risk (minimum {F(day.TempMinC)} °C): protect sensitive crops"));
            if (day.TempMaxC >= HeatTempC)
                warnings.Add(new WeatherWarning(day.Date, "Medium",
                    $"Heat stress risk (maximum {F(day.TempMaxC)} °C): irrigate early in the morning"));

            // Warnings that depend on what is scheduled that day
            var types = tasks.Where(t => t.Date == day.Date).Select(t => t.Type).ToHashSet();

            if (types.Contains("Irrigation") && day.PrecipitationMm >= RainDelaysIrrigationMm)
                warnings.Add(new WeatherWarning(day.Date, "Medium",
                    $"Rain forecast ({F(day.PrecipitationMm)} mm): consider postponing irrigation"));

            if (types.Contains("Fertilization") && day.PrecipitationMm >= HeavyRainMm)
                warnings.Add(new WeatherWarning(day.Date, "Medium",
                    $"Heavy rain forecast ({F(day.PrecipitationMm)} mm): fertilizer may wash away, consider postponing"));

            if (types.Contains("Fertilization") && day.WindMaxKmh >= StrongWindKmh)
                warnings.Add(new WeatherWarning(day.Date, "Medium",
                    $"Strong wind ({F(day.WindMaxKmh)} km/h): fertilizer spreading may drift"));

            if (types.Contains("Harvest") && day.PrecipitationMm >= HeavyRainMm)
                warnings.Add(new WeatherWarning(day.Date, "Medium",
                    $"Heavy rain forecast ({F(day.PrecipitationMm)} mm): harvest work may be delayed"));
        }

        return warnings.OrderBy(w => w.Date).ToList();
    }
}