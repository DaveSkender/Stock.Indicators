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
        => Increment(source, lookbackPeriods, endIndex, endIndex);

    /// <summary>
    /// Calculates EPMA increment for the current position using linear regression.
    /// </summary>
    /// <typeparam name="T">The type of the source items, must implement IReusable.</typeparam>
    /// <param name="source">The source data provider cache.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="cacheIndex">The index position in the cache to evaluate.</param>
    /// <param name="globalIndex">The actual global position in the full dataset (accounting for pruning).</param>
    /// <returns>The EPMA value or double.NaN if incalculable.</returns>
    internal static double Increment<T>(
        IReadOnlyList<T> source,
        int lookbackPeriods,
        int cacheIndex,
        int globalIndex)
        where T : IReusable
    {
        if (cacheIndex < lookbackPeriods - 1 || cacheIndex >= source.Count)
        {
            return double.NaN;
        }

        // Calculate linear regression for the lookback window using cache indices
        int startCacheIndex = cacheIndex - lookbackPeriods + 1;
        int startGlobalIndex = globalIndex - lookbackPeriods + 1;

        // Calculate averages using global position indices
        double sumX = 0;
        double sumY = 0;

        for (int i = 0; i < lookbackPeriods; i++)
        {
            sumX += startGlobalIndex + i + 1d; // X values are global positions (1-based)
            sumY += source[startCacheIndex + i].Value;
        }

        double avgX = sumX / lookbackPeriods;
        double avgY = sumY / lookbackPeriods;

        // Least squares method
        double sumSqX = 0;
        double sumSqXy = 0;

        for (int i = 0; i < lookbackPeriods; i++)
        {
            double devX = startGlobalIndex + i + 1d - avgX;
            double devY = source[startCacheIndex + i].Value - avgY;

            sumSqX += devX * devX;
            sumSqXy += devX * devY;
        }

        if (sumSqX == 0)
        {
            return double.NaN;
        }

        double slope = sumSqXy / sumSqX;
        double intercept = avgY - (slope * avgX);

        // EPMA calculation: slope * (endpoint_index + 1) + intercept
        // The endpoint index is the actual global position (1-based)
        return (slope * (globalIndex + 1)) + intercept;
    }
}
