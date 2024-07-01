namespace Skender.Stock.Indicators;

// CHOPPINESS INDEX (COMMON)

public static class Chop
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for CHOP.");
        }
    }

}
