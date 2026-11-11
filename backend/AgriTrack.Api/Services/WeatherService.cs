// Built with help from Claude
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AgriTrack.Api.Calculations;

namespace AgriTrack.Api.Services;

// Shape of the Open-Meteo JSON response (only the parts we use)
public record OpenMeteoResponse([property: JsonPropertyName("daily")] OpenMeteoDaily? Daily);

public record OpenMeteoDaily(
    [property: JsonPropertyName("time")] List<string> Time,
    [property: JsonPropertyName("temperature_2m_max")] List<decimal?> TempMax,
    [property: JsonPropertyName("temperature_2m_min")] List<decimal?> TempMin,
    [property: JsonPropertyName("precipitation_sum")] List<decimal?> Precipitation,
    [property: JsonPropertyName("wind_speed_10m_max")] List<decimal?> WindMax);

public class WeatherService(HttpClient http)
{
    public async Task<List<DailyForecast>> GetForecastAsync(double latitude, double longitude, int days = 7)
    {
        // Invariant culture, so coordinates always use a dot even on a French PC
        var url = FormattableString.Invariant(
            $"v1/forecast?latitude={latitude}&longitude={longitude}&daily=temperature_2m_max,temperature_2m_min,precipitation_sum,wind_speed_10m_max&timezone=auto&forecast_days={days}");

        var data = await http.GetFromJsonAsync<OpenMeteoResponse>(url);
        var daily = data?.Daily ?? throw new InvalidOperationException("Weather service returned no data");

        var result = new List<DailyForecast>();
        for (var i = 0; i < daily.Time.Count; i++)
        {
            result.Add(new DailyForecast(
                DateOnly.Parse(daily.Time[i], CultureInfo.InvariantCulture),
                daily.TempMax[i] ?? 0,
                daily.TempMin[i] ?? 0,
                daily.Precipitation[i] ?? 0,
                daily.WindMax[i] ?? 0));
        }
        return result;
    }
}