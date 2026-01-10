namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for Williams Fractal calculations.
/// </summary>
public static partial class Fractal
{
    /// <summary>
    /// Converts Fractal results to a chainable list using the specified side.
    /// </summary>
    /// <param name="results">The list of Fractal results.</param>
    /// <param name="side">The fractal side to use for chaining.</param>
    /// <returns>A list of chainable values.</returns>
    public static IReadOnlyList<QuotePart> Use(
        this IReadOnlyList<FractalResult> results,
        FractalSide side = FractalSide.Bear)
    {
        ArgumentNullException.ThrowIfNull(results);
        int length = results.Count;
        List<QuotePart> list = new(length);

        for (int i = 0; i < length; i++)
        {
            FractalResult r = results[i];

            double value = side switch {
                FractalSide.Bear => (double?)r.FractalBear ?? double.NaN,
                FractalSide.Bull => (double?)r.FractalBull ?? double.NaN,
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, "Invalid side provided.")
            };

            list.Add(new QuotePart(r.Timestamp, value));
        }

        return list;
    }

    /// <summary>
    /// Removes empty (null) periods from the Fractal results.
    /// </summary>
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<FractalResult> Condense(
        this IReadOnlyList<FractalResult> results)
    {
        List<FractalResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                static x => x.FractalBull is null && x.FractalBear is null);

        return resultsList.ToSortedList();
    }

    /// <summary>
    /// Validates the window span for Fractal calculations.
    /// </summary>
    /// <param name="windowSpan">The number of periods to look back and forward for the calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the window span is less than 2.
    /// </exception>
    internal static void Validate(
        int windowSpan)
    {
        // check parameter arguments
        if (windowSpan < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(windowSpan), windowSpan,
                "Window span must be at least 2 for Fractal.");
        }
    }
}
