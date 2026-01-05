namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the SuperTrend indicator.
/// </summary>
public static partial class SuperTrend
{
    /// <summary>
    /// Converts SuperTrend results to a chainable list using the specified field.
    /// </summary>
    /// <param name="results">The list of SuperTrend results.</param>
    /// <param name="field">The field to use for chaining.</param>
    /// <returns>A list of chainable values.</returns>
    public static IReadOnlyList<QuotePart> Use(
        this IReadOnlyList<SuperTrendResult> results,
        SuperTrendField field = SuperTrendField.SuperTrend)
    {
        ArgumentNullException.ThrowIfNull(results);
        int length = results.Count;
        List<QuotePart> list = new(length);

        for (int i = 0; i < length; i++)
        {
            SuperTrendResult r = results[i];

            double value = field switch {
                SuperTrendField.SuperTrend => (double?)r.SuperTrend ?? double.NaN,
                SuperTrendField.UpperBand => (double?)r.UpperBand ?? double.NaN,
                SuperTrendField.LowerBand => (double?)r.LowerBand ?? double.NaN,
                _ => throw new ArgumentOutOfRangeException(nameof(field), field, "Invalid field provided.")
            };

            list.Add(new QuotePart(r.Timestamp, value));
        }

        return list;
    }

    /// <summary>
    /// Removes empty (null) periods from the results.
    /// </summary>
    /// <param name="results">The list of SuperTrend results.</param>
    /// <returns>A condensed list of SuperTrend results without null periods.</returns>
    public static IReadOnlyList<SuperTrendResult> Condense(
        this IReadOnlyList<SuperTrendResult> results)
    {
        List<SuperTrendResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                static x => x.SuperTrend is null);

        return resultsList.ToSortedList();
    }

    /// <summary>
    /// Removes the recommended warmup periods from the results.
    /// </summary>
    /// <param name="results">The list of SuperTrend results.</param>
    /// <returns>A list of SuperTrend results with warmup periods removed.</returns>
    public static IReadOnlyList<SuperTrendResult> RemoveWarmupPeriods(
        this IReadOnlyList<SuperTrendResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        int removePeriods = results
            .FindIndex(static x => x.SuperTrend != null);

        return results.Remove(removePeriods);
    }

    /// <summary>
    /// Validates the parameters for SuperTrend calculation.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods for the lookback.</param>
    /// <param name="multiplier">The multiplier for the SuperTrend calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a parameter is out of range.</exception>
    internal static void Validate(
        int lookbackPeriods,
        double multiplier)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for SuperTrend.");
        }

        if (multiplier <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                "Multiplier must be greater than 0 for SuperTrend.");
        }
    }
}
