namespace Skender.Stock.Indicators;

public static partial class Correlation
{
    /// <summary>
    /// parameter validation
    /// </summary>
    /// <typeparam name="T">Type of record</typeparam>
    /// <param name="sourceA">First series of values</param>
    /// <param name="sourceB">Second series of values</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a parameter is out of the valid range</exception>
    /// <exception cref="InvalidQuotesException">Thrown when quotes are invalid or insufficient</exception>
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
