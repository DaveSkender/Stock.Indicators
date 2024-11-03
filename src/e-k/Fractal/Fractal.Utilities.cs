namespace Skender.Stock.Indicators;

// WILLIAMS FRACTAL (UTILITIES)

public static partial class Fractal
{
    // remove empty (null) periods
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<FractalResult> Condense(
        this IReadOnlyList<FractalResult> results)
    {
        List<FractalResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.FractalBull is null && x.FractalBear is null);

        return resultsList.ToSortedList();
    }

    // parameter validation
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
