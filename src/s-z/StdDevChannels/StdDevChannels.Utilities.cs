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
            .RemoveAll(match: x =>
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
        int removePeriods = results
            .FindIndex(x => x.UpperChannel != null || x.LowerChannel != null);

        return results.Remove(removePeriods);
    }

    /// <summary>
    /// Validates the parameters for Standard Deviation Channels.
    /// </summary>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
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
