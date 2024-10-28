namespace Skender.Stock.Indicators;

// COMMODITY CHANNEL INDEX (UTILITIES)

public static partial class Cci
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Commodity Channel Index.");
        }
    }

}
