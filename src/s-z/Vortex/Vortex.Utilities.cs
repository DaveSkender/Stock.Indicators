namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<VortexResult> Condense(
        this IReadOnlyList<VortexResult> results)
    {
        List<VortexResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.Pvi is null && x.Nvi is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<VortexResult> RemoveWarmupPeriods(
        this IReadOnlyList<VortexResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Pvi != null || x.Nvi != null);

        return results.Remove(removePeriods);
    }
}
