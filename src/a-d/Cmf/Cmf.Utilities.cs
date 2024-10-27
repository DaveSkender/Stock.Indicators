namespace Skender.Stock.Indicators;

// CHAIKIN MONEY FLOW (UTILITIES)

public static partial class Cmf
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Chaikin Money Flow.");
        }
    }

}
