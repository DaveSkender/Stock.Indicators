namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods Standard Deviation Channels calculations.
/// </summary>
public static partial class StdDevChannels
{
    /// <summary>
    /// Removes empty (null) periods from the results.
    /// </summary>
    /// <param name="results">The list of results to condense.</param>
    /// <returns>A condensed list of results.</returns>
    public static IReadOnlyList<StdDevChannelsResult> Condense(
        this IReadOnlyList<StdDevChannelsResult> results)
    {
        List<StdDevChannelsResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match: static x =>
               x.UpperChannel is null
            && x.LowerChannel is null
            && x.Centerline is null
            && !x.BreakPoint);

        return resultsList.ToSortedList();
    }

    /// <summary>
    /// Removes recommended warmup periods from the results.
    /// </summary>
    /// <param name="results">The list of results to process.</param>
    /// <returns>A list of results with warmup periods removed.</returns>
    public static IReadOnlyList<StdDevChannelsResult> RemoveWarmupPeriods(
        this IReadOnlyList<StdDevChannelsResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        int removePeriods = results
            .FindIndex(static x => x.UpperChannel != null || x.LowerChannel != null);

        return results.Remove(removePeriods);
    }

    /// <summary>
    /// Standard Deviation Channels calculation for streaming scenarios.
    /// </summary>
    /// <param name="source">List of chainable values.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="standardDeviations">Number of standard deviations for bands.</param>
    /// <param name="endIndex">Index position to evaluate.</param>
    /// <typeparam name="T">IReusable (chainable) type.</typeparam>
    /// <returns>Standard Deviation Channels result or null result when insufficient data.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a parameter is out of the valid range</exception>
    internal static StdDevChannelsResult Increment<T>(
        IReadOnlyList<T> source,
        int lookbackPeriods,
        double standardDeviations,
        int endIndex)
        where T : IReusable
    {
        ArgumentNullException.ThrowIfNull(source);

        if ((uint)endIndex >= (uint)source.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(endIndex), endIndex,
                "End index must refer to an existing element in the source cache.");
        }

        DateTime timestamp = source[endIndex].Timestamp;

        // Not enough data yet
        if (endIndex < lookbackPeriods - 1)
        {
            return new StdDevChannelsResult(timestamp);
        }

        // Determine which window this index belongs to based on TOTAL cache size
        // not the index being processed, because windows are anchored from the end
        int totalCacheSize = source.Count;  // Total items in cache
        int windowEndIndex = -1;
        bool isBreakPoint = false;

        // Find which window this index falls into
        // Start from most recent possible window endpoint and work backwards
        for (int i = totalCacheSize - 1; i >= lookbackPeriods - 1; i -= lookbackPeriods)
        {
            int winStart = i - lookbackPeriods + 1;
            if (endIndex >= winStart && endIndex <= i)
            {
                windowEndIndex = i;
                isBreakPoint = (endIndex == winStart);
                break;
            }
        }

        // If no window covers this index, return empty result
        if (windowEndIndex == -1)
        {
            return new StdDevChannelsResult(timestamp);
        }

        // Calculate regression for the window ending at windowEndIndex
        int windowStart = windowEndIndex - lookbackPeriods + 1;

        // Calculate sums for regression
        double sumX = 0;
        double sumY = 0;

        for (int p = windowStart; p <= windowEndIndex; p++)
        {
            sumX += p + 1d;
            sumY += source[p].Value;
        }

        double avgX = sumX / lookbackPeriods;
        double avgY = sumY / lookbackPeriods;

        // Least squares method
        double sumSqX = 0;
        double sumSqY = 0;
        double sumSqXy = 0;

        for (int p = windowStart; p <= windowEndIndex; p++)
        {
            double devX = p + 1d - avgX;
            double devY = source[p].Value - avgY;

            sumSqX += devX * devX;
            sumSqY += devY * devY;
            sumSqXy += devX * devY;
        }

        double slope = sumSqXy / sumSqX;
        double intercept = avgY - (slope * avgX);

        // Calculate standard deviation
        double stdDevY = Math.Sqrt(sumSqY / lookbackPeriods);

        // Calculate centerline and channels for this specific index
        double? centerline = (slope * (endIndex + 1)) + intercept;
        double? width = standardDeviations * stdDevY;

        return new StdDevChannelsResult(
            Timestamp: timestamp,
            Centerline: centerline,
            UpperChannel: centerline + width,
            LowerChannel: centerline - width,
            BreakPoint: isBreakPoint
        );
    }

    /// <summary>
    /// Validates the parameters for Standard Deviation Channels.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="stdDeviations">The number of standard deviations.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are out of range.</exception>
    internal static void Validate(
        int? lookbackPeriods,
        double stdDeviations)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Standard Deviation Channels.");
        }

        if (stdDeviations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stdDeviations), stdDeviations,
                "Standard Deviations must be greater than 0 for Standard Deviation Channels.");
        }
    }
}
