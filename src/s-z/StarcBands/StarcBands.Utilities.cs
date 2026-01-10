namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for STARC Bands calculations.
/// </summary>
public static partial class StarcBands
{
    /// <summary>
    /// Converts STARC Bands results to a chainable list using the specified field.
    /// </summary>
    /// <param name="results">The list of STARC Bands results.</param>
    /// <param name="field">The field to use for chaining.</param>
    /// <returns>A list of chainable values.</returns>
    public static IReadOnlyList<QuotePart> Use(
        this IReadOnlyList<StarcBandsResult> results,
        StarcBandsField field = StarcBandsField.Centerline)
    {
        ArgumentNullException.ThrowIfNull(results);
        int length = results.Count;
        List<QuotePart> list = new(length);

        for (int i = 0; i < length; i++)
        {
            StarcBandsResult r = results[i];

            double value = field switch {
                StarcBandsField.UpperBand => r.UpperBand.Null2NaN(),
                StarcBandsField.Centerline => r.Centerline.Null2NaN(),
                StarcBandsField.LowerBand => r.LowerBand.Null2NaN(),
                _ => throw new ArgumentOutOfRangeException(nameof(field), field, "Invalid field provided.")
            };

            list.Add(new QuotePart(r.Timestamp, value));
        }

        return list;
    }

    /// <summary>
    /// Removes empty (null) periods from the results.
    /// </summary>
    /// <param name="results">The list of STARC Bands results.</param>
    /// <returns>A condensed list of STARC Bands results.</returns>
    public static IReadOnlyList<StarcBandsResult> Condense(
        this IReadOnlyList<StarcBandsResult> results)
    {
        List<StarcBandsResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                static x => x.UpperBand is null && x.LowerBand is null && x.Centerline is null);

        return resultsList.ToSortedList();
    }

    /// <summary>
    /// Removes the recommended warmup periods from the results.
    /// </summary>
    /// <param name="results">The list of STARC Bands results.</param>
    /// <returns>A list of STARC Bands results with warmup periods removed.</returns>
    public static IReadOnlyList<StarcBandsResult> RemoveWarmupPeriods(
        this IReadOnlyList<StarcBandsResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        int n = results
            .FindIndex(static x => x.UpperBand != null || x.LowerBand != null) + 1;

        return results.Remove(n + 150);
    }

    /// <summary>
    /// Validates the parameters for STARC Bands calculation.
    /// </summary>
    /// <param name="smaPeriods">The number of periods for the simple moving average.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <param name="atrPeriods">The number of periods for the average true range.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a parameter is out of range.</exception>
    internal static void Validate(
        int smaPeriods,
        double multiplier,
        int atrPeriods)
    {
        // check parameter arguments
        if (smaPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "EMA periods must be greater than 1 for STARC Bands.");
        }

        if (atrPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(atrPeriods), atrPeriods,
                "ATR periods must be greater than 1 for STARC Bands.");
        }

        if (multiplier <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                "Multiplier must be greater than 0 for STARC Bands.");
        }
    }
}
