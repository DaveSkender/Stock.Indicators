namespace Skender.Stock.Indicators;

// DETRENDED PRICE OSCILLATOR (UTILITIES)

public static partial class Dpo
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for DPO.");
        }
    }

}
