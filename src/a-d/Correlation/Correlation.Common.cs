namespace Skender.Stock.Indicators;

// CORRELATION COEFFICIENT (COMMON)

public static class Correlation
{
    // parameter validation
    internal static void Validate(
        List<(DateTime, double)> quotesA,
        List<(DateTime, double)> quotesB,
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Correlation.");
        }

        // check quotes
        if (quotesA.Count != quotesB.Count)
        {
            throw new InvalidQuotesException(
                nameof(quotesB),
                "B quotes should have at least as many records as A quotes for Correlation.");
        }
    }

}
