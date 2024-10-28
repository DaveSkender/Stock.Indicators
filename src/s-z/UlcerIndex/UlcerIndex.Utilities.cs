namespace Skender.Stock.Indicators;

// ULCER INDEX (UTILITIES)

public static partial class UlcerIndex
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Ulcer Index.");
        }
    }
}
