using AgriTrack.Api.Calculations;
using Xunit;

namespace AgriTrack.Tests;

public class WeatherAdvisorTests
{
    private static readonly DateOnly Day = new(2026, 1, 8);

    private static SuggestedTask Task(string type) => new(Day, type, "test", "Establishment");

    [Fact]
    public void Dry_mild_weather_gives_no_warnings()
    {
        var forecast = new List<DailyForecast> { new(Day, 25m, 12m, 0m, 10m) };

        Assert.Empty(WeatherAdvisor.Analyze(forecast, new List<SuggestedTask> { Task("Irrigation") }));
    }

    [Fact]
    public void Frost_gives_a_high_severity_warning()
    {
        var forecast = new List<DailyForecast> { new(Day, 10m, 1m, 0m, 5m) };

        var warnings = WeatherAdvisor.Analyze(forecast, new List<SuggestedTask>());

        Assert.Single(warnings);
        Assert.Equal("High", warnings[0].Severity);
    }

    [Fact]
    public void Rain_on_an_irrigation_day_suggests_postponing()
    {
        var forecast = new List<DailyForecast> { new(Day, 20m, 10m, 8m, 10m) };

        var warnings = WeatherAdvisor.Analyze(forecast, new List<SuggestedTask> { Task("Irrigation") });

        Assert.Single(warnings);
        Assert.Contains("postponing irrigation", warnings[0].Message);
    }

    [Fact]
    public void Heavy_rain_on_a_fertilizer_day_warns_about_washing_away()
    {
        var forecast = new List<DailyForecast> { new(Day, 20m, 10m, 12m, 10m) };

        var warnings = WeatherAdvisor.Analyze(forecast, new List<SuggestedTask> { Task("Fertilization") });

        Assert.Contains(warnings, w => w.Message.Contains("fertilizer may wash away"));
    }
}