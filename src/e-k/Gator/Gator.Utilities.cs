namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for Gator Oscillator calculations.
/// </summary>
public static partial class Gator
{
    /// <summary>
    /// Converts Gator Oscillator results to a chainable list using the specified side.
    /// </summary>
    /// <param name="results">The list of Gator Oscillator results.</param>
    /// <param name="side">The histogram side to use for chaining.</param>
    /// <returns>A list of chainable values.</returns>
    public static IReadOnlyList<QuotePart> Use(
        this IReadOnlyList<GatorResult> results,
        GatorSide side = GatorSide.Upper)
    {
        ArgumentNullException.ThrowIfNull(results);
        int length = results.Count;
        List<QuotePart> list = new(length);

        for (int i = 0; i < length; i++)
        {
            GatorResult r = results[i];

            double value = side switch {
                GatorSide.Upper => r.Upper.Null2NaN(),
                GatorSide.Lower => r.Lower.Null2NaN(),
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, "Invalid side provided.")
            };

            list.Add(new QuotePart(r.Timestamp, value));
        }

        return list;
    }

    /// <summary>
    /// Removes empty (null) periods from the Gator Oscillator results.
    /// </summary>
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<GatorResult> Condense(
        this IReadOnlyList<GatorResult> results)
    {
        List<GatorResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                static x => x.Upper is null && x.Lower is null);

        return resultsList.ToSortedList();
    }

    /// <summary>
    /// Removes the recommended warmup periods from the Gator Oscillator results.
    /// </summary>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<GatorResult> RemoveWarmupPeriods(
        this IReadOnlyList<GatorResult> results) => results.Remove(150);
}
