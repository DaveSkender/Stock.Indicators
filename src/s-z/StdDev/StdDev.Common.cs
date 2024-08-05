namespace Skender.Stock.Indicators;

// STANDARD DEVIATION (COMMON)

public static class StdDev
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Standard Deviation.");
        }
    }

}
