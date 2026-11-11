// Built with help from Claude
namespace AgriTrack.Api.Calculations;

public record SuggestedTask(DateOnly Date, string Type, string Title, string Stage);

public static class FarmTaskScheduler
{
    public const int IrrigationIntervalDays = 7;
    public const int MonitoringIntervalDays = 14;

    // Cycle length: the farmer's expected harvest date if set, otherwise the crop's growth days
    public static int CycleDays(DateOnly plantingDate, DateOnly? expectedHarvest, int growthDays)
    {
        if (expectedHarvest.HasValue)
            return Math.Max(1, expectedHarvest.Value.DayNumber - plantingDate.DayNumber);
        return Math.Max(1, growthDays);
    }

    // Growth stage = how far we are through the cycle
    public static string StageAt(DateOnly plantingDate, int cycleDays, DateOnly date)
    {
        if (cycleDays <= 0)
            throw new ArgumentOutOfRangeException(nameof(cycleDays), "Cycle must be positive");
        if (date < plantingDate) return "Not planted yet";

        var fraction = (decimal)(date.DayNumber - plantingDate.DayNumber) / cycleDays;
        if (fraction < 0.15m) return "Establishment";
        if (fraction < 0.45m) return "Vegetative growth";
        if (fraction < 0.75m) return "Flowering and fruiting";
        return "Maturity";
    }

    public static List<SuggestedTask> Generate(DateOnly plantingDate, int cycleDays)
    {
        if (cycleDays <= 0)
            throw new ArgumentOutOfRangeException(nameof(cycleDays), "Cycle must be positive");

        var tasks = new List<SuggestedTask>();

        void Add(int dayOffset, string type, string title)
        {
            var date = plantingDate.AddDays(dayOffset);
            tasks.Add(new SuggestedTask(date, type, title, StageAt(plantingDate, cycleDays, date)));
        }

        // Day number at a given fraction of the cycle
        int Day(decimal fraction) =>
            (int)Math.Round(cycleDays * fraction, MidpointRounding.AwayFromZero);

        Add(Day(0.15m), "Fertilization", "Apply first fertilizer dose");
        Add(Day(0.45m), "Fertilization", "Apply second fertilizer dose");

        // Regular care stops at 90% of the cycle, just before harvest
        var lastCareDay = cycleDays * 0.9m;
        for (var d = IrrigationIntervalDays; d <= lastCareDay; d += IrrigationIntervalDays)
            Add(d, "Irrigation", "Irrigate the field");
        for (var d = MonitoringIntervalDays; d <= lastCareDay; d += MonitoringIntervalDays)
            Add(d, "Monitoring", "Inspect for pests and disease");

        Add(Day(0.9m), "Harvest", "Prepare equipment and labour for harvest");
        Add(cycleDays, "Harvest", "Expected harvest");

        return tasks.OrderBy(t => t.Date).ThenBy(t => t.Type).ToList();
    }
}