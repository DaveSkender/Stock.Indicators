namespace Skender.Stock.Indicators;

// BETA COEFFICIENT (COMMON)

public static class Beta
{
    // parameter validation
    internal static void Validate<T>(
        List<T> sourceEval,
        List<T> sourceMrkt,
        int lookbackPeriods)
        where T : ISeries
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Beta.");
        }

        // check quotes
        if (sourceEval.Count != sourceMrkt.Count)
        {
            throw new InvalidQuotesException(
                nameof(sourceEval),
                "Eval quotes should have the same number of Market quotes for Beta.");
        }
    }

}
