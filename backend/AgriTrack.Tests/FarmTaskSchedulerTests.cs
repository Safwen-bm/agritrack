using AgriTrack.Api.Calculations;
using Xunit;

namespace AgriTrack.Tests;

public class FarmTaskSchedulerTests
{
    private static readonly DateOnly Start = new(2026, 1, 1);

    [Fact]
    public void Last_task_is_the_harvest_at_the_end_of_the_cycle()
    {
        var last = FarmTaskScheduler.Generate(Start, 120)[^1];

        Assert.Equal("Harvest", last.Type);
        Assert.Equal(new DateOnly(2026, 5, 1), last.Date);
    }

    [Fact]
    public void Fertilizer_doses_are_at_15_and_45_percent_of_the_cycle()
    {
        var dates = FarmTaskScheduler.Generate(Start, 120)
            .Where(t => t.Type == "Fertilization").Select(t => t.Date).ToList();

        Assert.Equal(new[] { new DateOnly(2026, 1, 19), new DateOnly(2026, 2, 24) }, dates);
    }

    [Fact]
    public void Irrigation_is_weekly_and_stops_at_90_percent_of_the_cycle()
    {
        var irrigation = FarmTaskScheduler.Generate(Start, 120)
            .Where(t => t.Type == "Irrigation").ToList();

        // days 7, 14, ... 105 (the next one, day 112, is past day 108)
        Assert.Equal(15, irrigation.Count);
        Assert.Equal(new DateOnly(2026, 1, 8), irrigation[0].Date);
        Assert.Equal(new DateOnly(2026, 4, 16), irrigation[^1].Date);
    }

    [Fact]
    public void Tasks_are_sorted_by_date()
    {
        var tasks = FarmTaskScheduler.Generate(Start, 120);

        Assert.Equal(tasks.OrderBy(t => t.Date).ToList(), tasks);
    }

    [Fact]
    public void Stage_follows_the_progress_of_the_cycle()
    {
        Assert.Equal("Not planted yet", FarmTaskScheduler.StageAt(Start, 120, new DateOnly(2025, 12, 31)));
        Assert.Equal("Establishment", FarmTaskScheduler.StageAt(Start, 120, Start));
        Assert.Equal("Flowering and fruiting", FarmTaskScheduler.StageAt(Start, 120, new DateOnly(2026, 3, 1)));
        Assert.Equal("Maturity", FarmTaskScheduler.StageAt(Start, 120, new DateOnly(2026, 6, 1)));
    }

    [Fact]
    public void Cycle_uses_expected_harvest_or_falls_back_to_growth_days()
    {
        Assert.Equal(30, FarmTaskScheduler.CycleDays(Start, new DateOnly(2026, 1, 31), 120));
        Assert.Equal(120, FarmTaskScheduler.CycleDays(Start, null, 120));
    }

    [Fact]
    public void Zero_cycle_is_rejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FarmTaskScheduler.Generate(Start, 0));
    }
}