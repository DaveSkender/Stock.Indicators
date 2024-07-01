namespace Skender.Stock.Indicators;

public static partial class Atr
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

    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Average True Range.");
        }
    }
}
