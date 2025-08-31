namespace Skender.Stock.Indicators;

/// <summary>
/// Provides common utility methods for indicators.
/// </summary>
public static class IndicatorUtilities
{
    /// <summary>
    /// Validates that the lookback periods are greater than 0.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="indicatorName">The name of the indicator for error messages.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the lookback periods are less than or equal to 0.
    /// </exception>
    public static void ValidateLookbackPeriods(int lookbackPeriods, string indicatorName)
    {
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lookbackPeriods), lookbackPeriods,
                $"Lookback periods must be greater than 0 for {indicatorName}.");
        }
    }

    /// <summary>
    /// Validates multiple periods for multi-period indicators like MACD.
    /// </summary>
    /// <param name="fastPeriods">The fast period.</param>
    /// <param name="slowPeriods">The slow period.</param>
    /// <param name="signalPeriods">The signal period.</param>
    /// <param name="indicatorName">The name of the indicator for error messages.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when any of the parameters are out of their valid range.
    /// </exception>
    public static void ValidateMultiPeriods(
        int fastPeriods, 
        int slowPeriods, 
        int signalPeriods, 
        string indicatorName)
    {
        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                $"Fast periods must be greater than 0 for {indicatorName}.");
        }

        if (signalPeriods < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                $"Signal periods must be greater than or equal to 0 for {indicatorName}.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                $"Slow periods must be greater than the fast period for {indicatorName}.");
        }
    }

    /// <summary>
    /// Generic method to remove warmup periods based on a predicate.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="results">The list of results.</param>
    /// <param name="firstValidIndex">Function to find the first valid result index.</param>
    /// <param name="additionalWarmup">Additional warmup periods to remove.</param>
    /// <returns>A list of results with warmup periods removed.</returns>
    public static IReadOnlyList<T> RemoveWarmupPeriods<T>(
        this IReadOnlyList<T> results,
        Func<IReadOnlyList<T>, int> firstValidIndex,
        int additionalWarmup = 0)
        where T : ISeries
    {
        ArgumentNullException.ThrowIfNull(firstValidIndex);
        int n = firstValidIndex(results) + 1;
        return results.Remove(n + additionalWarmup);
    }
}