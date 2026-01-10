namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the Pivot Points indicator.
/// </summary>
public static partial class Pivots
{
    /// <summary>
    /// Converts Pivots results to a chainable list using the specified field.
    /// </summary>
    /// <param name="results">The list of Pivots results.</param>
    /// <param name="field">The field to use for chaining.</param>
    /// <returns>A list of chainable values.</returns>
    public static IReadOnlyList<QuotePart> Use(
        this IReadOnlyList<PivotsResult> results,
        PivotPointField field = PivotPointField.HighPoint)
    {
        ArgumentNullException.ThrowIfNull(results);
        int length = results.Count;
        List<QuotePart> list = new(length);

        for (int i = 0; i < length; i++)
        {
            PivotsResult r = results[i];

            double value = field switch {
                PivotPointField.HighPoint => (double?)r.HighPoint ?? double.NaN,
                PivotPointField.LowPoint => (double?)r.LowPoint ?? double.NaN,
                PivotPointField.HighLine => (double?)r.HighLine ?? double.NaN,
                PivotPointField.LowLine => (double?)r.LowLine ?? double.NaN,
                _ => throw new ArgumentOutOfRangeException(nameof(field), field, "Invalid field provided.")
            };

            list.Add(new QuotePart(r.Timestamp, value));
        }

        return list;
    }

    /// <summary>
    /// Removes empty (null) periods from the pivot points results.
    /// </summary>
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<PivotsResult> Condense(
        this IReadOnlyList<PivotsResult> results)
    {
        List<PivotsResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                static x => x.HighPoint is null && x.LowPoint is null);

        return resultsList.ToSortedList();
    }

    /// <summary>
    /// Validates the parameters for pivot points calculations.
    /// </summary>
    /// <param name="leftSpan">The number of periods to the left of the pivot point.</param>
    /// <param name="rightSpan">The number of periods to the right of the pivot point.</param>
    /// <param name="maxTrendPeriods">The maximum number of periods for trend calculation.</param>
    /// <param name="caller">The name of the calling method.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any parameter is out of range.</exception>
    internal static void Validate(
        int leftSpan,
        int rightSpan,
        int maxTrendPeriods,
        string caller = "Pivots")
    {
        // check parameter arguments
        if (rightSpan < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(rightSpan), rightSpan,
                $"Right span must be at least 2 for {caller}.");
        }

        if (leftSpan < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(leftSpan), leftSpan,
                $"Left span must be at least 2 for {caller}.");
        }

        if (maxTrendPeriods <= leftSpan)
        {
            throw new ArgumentOutOfRangeException(nameof(leftSpan), leftSpan,
                $"Lookback periods must be greater than the Left window span for {caller}.");
        }
    }
}
