namespace Skender.Stock.Indicators;

// FRACTAL CHAOS BANDS (UTILITIES)

public static partial class Fcb
{
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<FcbResult> Condense(
        this IReadOnlyList<FcbResult> results)
    {
        List<FcbResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.UpperBand is null && x.LowerBand is null);

        return resultsList.ToSortedList();
    }

    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<FcbResult> RemoveWarmupPeriods(
        this IReadOnlyList<FcbResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.UpperBand != null || x.LowerBand != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    internal static void Validate(
        int windowSpan)
    {
        // check parameter arguments
        if (windowSpan < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(windowSpan), windowSpan,
                "Window span must be at least 2 for FCB.");
        }
    }
}
