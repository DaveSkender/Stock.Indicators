namespace Skender.Stock.Indicators;

// FORCE INDEX (COMMON)

public static class ForceIndex
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Force Index.");
        }
    }

}
