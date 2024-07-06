namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (UTILITIES)

public static partial class Ema
{
    public static double Increment(
        double k,
        double lastEma,
        double newPrice)
        => lastEma + k * (newPrice - lastEma);

    public static double Increment(
        int lookbackPeriods,
        double lastEma,
        double newPrice)
    {
        double k = 2d / (lookbackPeriods + 1);
        return Increment(k, lastEma, newPrice);
    }

    // remove recommended periods
    public static IEnumerable<EmaResult> RemoveWarmupPeriods(
        this IEnumerable<EmaResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.Ema != null) + 1;

        return results.Remove(n + 100);
    }

    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for EMA.");
        }
    }
}
