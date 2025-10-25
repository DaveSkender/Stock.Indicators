namespace Skender.Stock.Indicators;

public static partial class Correlation
{
    /// <summary>
    /// parameter validation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sourceA"></param>
    /// <param name="sourceB"></param>
    /// <param name="lookbackPeriods"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="InvalidQuotesException"></exception>
    internal static void Validate<T>(
        IReadOnlyList<T> sourceA,
        IReadOnlyList<T> sourceB,
        int lookbackPeriods)
        where T : ISeries
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Correlation.");
        }

        // check quotes
        if (sourceA.Count != sourceB.Count)
        {
            throw new InvalidQuotesException(
                nameof(sourceB),
                "B quotes should have at least as many records as A quotes for Correlation.");
        }
    }
}
