namespace Skender.Stock.Indicators;

// GATOR OSCILLATOR (UTILITIES)

public static partial class Gator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<GatorResult> Condense(
        this IReadOnlyList<GatorResult> results)
    {
        List<GatorResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.Upper is null && x.Lower is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<GatorResult> RemoveWarmupPeriods(
        this IReadOnlyList<GatorResult> results) => results.Remove(150);
}
