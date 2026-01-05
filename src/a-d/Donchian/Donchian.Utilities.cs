namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for Donchian Channel calculations.
/// </summary>
public static partial class Donchian
{
    /// <summary>
    /// Converts Donchian Channel results to a chainable list using the specified field.
    /// </summary>
    /// <param name="results">The list of Donchian Channel results.</param>
    /// <param name="field">The field to use for chaining.</param>
    /// <returns>A list of chainable values.</returns>
    public static IReadOnlyList<QuotePart> Use(
        this IReadOnlyList<DonchianResult> results,
        DonchianField field = DonchianField.Centerline)
    {
        ArgumentNullException.ThrowIfNull(results);
        int length = results.Count;
        List<QuotePart> list = new(length);

        for (int i = 0; i < length; i++)
        {
            DonchianResult r = results[i];

            double value = field switch {
                DonchianField.UpperBand => (double?)r.UpperBand ?? double.NaN,
                DonchianField.Centerline => (double?)r.Centerline ?? double.NaN,
                DonchianField.LowerBand => (double?)r.LowerBand ?? double.NaN,
                DonchianField.Width => (double?)r.Width ?? double.NaN,
                _ => throw new ArgumentOutOfRangeException(nameof(field), field, "Invalid field provided.")
            };

            list.Add(new QuotePart(r.Timestamp, value));
        }

        return list;
    }

    /// <summary>
    /// Removes empty (null) periods from the Donchian Channel results.
    /// </summary>
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<DonchianResult> Condense(
        this IReadOnlyList<DonchianResult> results)
    {
        List<DonchianResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                static x => x.UpperBand is null && x.LowerBand is null && x.Centerline is null);

        return resultsList.ToSortedList();
    }

    /// <summary>
    /// Removes the recommended warmup periods from the Donchian Channel results.
    /// </summary>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<DonchianResult> RemoveWarmupPeriods(
        this IReadOnlyList<DonchianResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        int removePeriods = results
          .FindIndex(static x => x.Width != null);

        return results.Remove(removePeriods);
    }

    /// <summary>
    /// Validates the lookback periods for Donchian Channel calculations.
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
                "Lookback periods must be greater than 0 for Donchian Channel.");
        }
    }
}
