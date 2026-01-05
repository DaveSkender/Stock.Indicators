namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the Vortex indicator.
/// </summary>
public static partial class Vortex
{
    /// <summary>
    /// Converts Vortex results to a chainable list using the specified field.
    /// </summary>
    /// <param name="results">The list of Vortex results.</param>
    /// <param name="field">The field to use for chaining.</param>
    /// <returns>A list of chainable values.</returns>
    public static IReadOnlyList<QuotePart> Use(
        this IReadOnlyList<VortexResult> results,
        VortexField field = VortexField.Pvi)
    {
        ArgumentNullException.ThrowIfNull(results);
        int length = results.Count;
        List<QuotePart> list = new(length);

        for (int i = 0; i < length; i++)
        {
            VortexResult r = results[i];

            double value = field switch {
                VortexField.Pvi => r.Pvi.Null2NaN(),
                VortexField.Nvi => r.Nvi.Null2NaN(),
                _ => throw new ArgumentOutOfRangeException(nameof(field), field, "Invalid field provided.")
            };

            list.Add(new QuotePart(r.Timestamp, value));
        }

        return list;
    }

    // remove empty (null) periods
    /// <summary>
    /// Condenses the Vortex results by removing periods with null values.
    /// </summary>
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<VortexResult> Condense(
        this IReadOnlyList<VortexResult> results)
    {
        List<VortexResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                static x => x.Pvi is null && x.Nvi is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <summary>
    /// Removes the warmup periods from the Vortex results.
    /// </summary>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<VortexResult> RemoveWarmupPeriods(
        this IReadOnlyList<VortexResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        int removePeriods = results
            .FindIndex(static x => x.Pvi != null || x.Nvi != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    /// <summary>
    /// Validates the parameters for the Vortex calculation.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the lookback periods are less than or equal to 1.
    /// </exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for VI.");
        }
    }
}
