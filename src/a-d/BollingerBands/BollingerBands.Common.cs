namespace Skender.Stock.Indicators;

// BOLLINGER BANDS (COMMON)

public static class BollingerBands
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods,
        double standardDeviations)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Bollinger Bands.");
        }

        if (standardDeviations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(standardDeviations), standardDeviations,
                "Standard Deviations must be greater than 0 for Bollinger Bands.");
        }
    }

}
