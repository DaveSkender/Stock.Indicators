namespace Skender.Stock.Indicators;

// SLOPE AND LINEAR REGRESSION (UTILITIES)

public static partial class Slope
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Slope/Linear Regression.");
        }
    }
}
