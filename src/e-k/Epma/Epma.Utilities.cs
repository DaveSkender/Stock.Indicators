namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for Endpoint Moving Average (EPMA) calculations.
/// </summary>
public static partial class Epma
{
    /// <summary>
    /// Validates the lookback periods for EPMA calculations.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the lookback periods are less than or equal to 0.
    /// </exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Epma.");
        }
    }

    /// <summary>
    /// Calculates EPMA increment for the current position using linear regression.
    /// </summary>
    /// <typeparam name="T">The type of the source items, must implement IReusable.</typeparam>
    /// <param name="source">The source data provider cache.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="endIndex">The current index position to evaluate.</param>
    /// <returns>The EPMA value or double.NaN if incalculable.</returns>
    internal static double Increment<T>(
        IReadOnlyList<T> source,
        int lookbackPeriods,
        int endIndex)
        where T : IReusable
    {
        if (endIndex < lookbackPeriods - 1 || endIndex >= source.Count)
        {
            return double.NaN;
        }

        // Calculate linear regression for the lookback window using relative X values
        // X values are relative positions: 0, 1, 2, ..., (lookbackPeriods-1)
        // This makes calculations pruning-independent
        int startIndex = endIndex - lookbackPeriods + 1;

        // Calculate averages using relative position indices (0-based)
        double sumX = 0;
        double sumY = 0;

        for (int i = 0; i < lookbackPeriods; i++)
        {
            sumX += i; // Relative X values: 0, 1, 2, ..., n-1
            sumY += source[startIndex + i].Value;
        }

        double avgX = sumX / lookbackPeriods;
        double avgY = sumY / lookbackPeriods;

        // Least squares method
        double sumSqX = 0;
        double sumSqXy = 0;

        for (int i = 0; i < lookbackPeriods; i++)
        {
            double devX = i - avgX;
            double devY = source[startIndex + i].Value - avgY;

            sumSqX += devX * devX;
            sumSqXy += devX * devY;
        }

        if (sumSqX == 0)
        {
            return double.NaN;
        }

        double slope = sumSqXy / sumSqX;
        double intercept = avgY - (slope * avgX);

        // EPMA calculation: slope * (endpoint_relative_index) + intercept
        // The endpoint is at relative position (lookbackPeriods - 1)
        return (slope * (lookbackPeriods - 1)) + intercept;
    }

    /// <summary>
    /// Calculates EPMA increment for the current position using linear regression.
    /// </summary>
    /// <typeparam name="T">The type of the source items, must implement IReusable.</typeparam>
    /// <param name="source">The source data provider cache.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="cacheIndex">The index position in the cache to evaluate.</param>
    /// <param name="globalIndex">The actual global position in the full dataset (accounting for pruning) - DEPRECATED, use single-parameter overload.</param>
    /// <returns>The EPMA value or double.NaN if incalculable.</returns>
    [Obsolete("Use single-parameter Increment method instead. Global index is no longer needed.")]
    internal static double Increment<T>(
        IReadOnlyList<T> source,
        int lookbackPeriods,
        int cacheIndex,
        int globalIndex)
        where T : IReusable
        => Increment(source, lookbackPeriods, cacheIndex);
}
