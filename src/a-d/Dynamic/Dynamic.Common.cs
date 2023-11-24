namespace Skender.Stock.Indicators;

// McGINLEY DYNAMIC (COMMON)

public static class MgDynamic
{
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
