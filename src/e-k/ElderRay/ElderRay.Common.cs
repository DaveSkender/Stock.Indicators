namespace Skender.Stock.Indicators;

// ELDER-RAY (COMMON)

public static class ElderRay
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Elder-ray Index.");
        }
    }

}
