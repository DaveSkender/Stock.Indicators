namespace Skender.Stock.Indicators;

// FISHER TRANSFORM (COMMON)

public static class FisherTransform
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Fisher Transform.");
        }
    }

}
