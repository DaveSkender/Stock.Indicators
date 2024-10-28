namespace Skender.Stock.Indicators;

// TILLSON T3 MOVING AVERAGE (UTILITIES)

public static partial class T3
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods,
        double volumeFactor)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for T3.");
        }

        if (volumeFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(volumeFactor), volumeFactor,
                "Volume Factor must be greater than 0 for T3.");
        }
    }
}
