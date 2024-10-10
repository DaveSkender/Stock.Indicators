namespace Skender.Stock.Indicators;

// McGINLEY DYNAMIC (UTILITIES)

public static partial class MgDynamic
{
    // increment calculation
    public static double Increment(
        int lookbackPeriods,
        double kFactor,
        double newVal,
        double prevDyn)
        => prevDyn + (
            (newVal - prevDyn)
          / (kFactor * lookbackPeriods * Math.Pow(newVal / prevDyn, 4)));

    // parameter validation
    internal static void Validate(
        int lookbackPeriods,
        double kFactor)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for DYNAMIC.");
        }

        if (kFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(kFactor), kFactor,
                "K-Factor range adjustment must be greater than 0 for DYNAMIC.");
        }
    }

}
