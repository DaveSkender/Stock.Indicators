namespace Skender.Stock.Indicators;

// BETA COEFFICIENT (COMMON)

public static class Beta
{
    // parameter validation
    internal static void Validate(
        List<(DateTime, double)> tpListEval,
        List<(DateTime, double)> tpListMrkt,
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Beta.");
        }

        // check quotes
        if (tpListEval.Count != tpListMrkt.Count)
        {
            throw new InvalidQuotesException(
                nameof(tpListEval),
                "Eval quotes should have the same number of Market quotes for Beta.");
        }
    }

}
