namespace Skender.Stock.Indicators;

// VOLUME WEIGHTED MOVING AVERAGE (UTILITIES)

public static partial class Vwma
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Vwma.");
        }
    }
}
