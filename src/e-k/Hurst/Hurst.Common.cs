namespace Skender.Stock.Indicators;

// HURST EXPONENT (COMMON)

public static class Hurst
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods < 20)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be at least 20 for Hurst Exponent.");
        }
    }

}
