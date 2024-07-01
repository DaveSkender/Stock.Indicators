namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    public static IEnumerable<AtrResult> RemoveWarmupPeriods(
        this IEnumerable<AtrResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Atr != null);

        return results.Remove(removePeriods);
    }
}
