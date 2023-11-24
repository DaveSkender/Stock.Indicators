namespace Skender.Stock.Indicators;

// DOJI (COMMON)

public static class Doji
{
    // parameter validation
    internal static void Validate(
        double maxPriceChangePercent)
    {
        // check parameter arguments
        if (maxPriceChangePercent is < 0 or > 0.5)
        {
            throw new ArgumentOutOfRangeException(nameof(maxPriceChangePercent), maxPriceChangePercent,
                "Maximum Percent Change must be between 0 and 0.5 for Doji (0% to 0.5%).");
        }
    }

}
