namespace Skender.Stock.Indicators;

// ARNAUD LEGOUX MOVING AVERAGE (COMMON)

public static class Alma
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods,
        double offset,
        double sigma)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for ALMA.");
        }

        if (offset is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), offset,
                "Offset must be between 0 and 1 for ALMA.");
        }

        if (sigma <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sigma), sigma,
                "Sigma must be greater than 0 for ALMA.");
        }
    }

}
