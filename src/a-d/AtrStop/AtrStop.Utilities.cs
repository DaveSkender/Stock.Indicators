namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="ReusableUtility.Condense{T}(IEnumerable{T})"/>
    public static IEnumerable<AtrStopResult> Condense(
        this IEnumerable<AtrStopResult> results)
    {
        List<AtrStopResult> resultsList = results
            .ToList();

        resultsList.RemoveAll(match: x => x.AtrStop is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<AtrStopResult> RemoveWarmupPeriods(
        this IEnumerable<AtrStopResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.AtrStop != null);

        return results.Remove(removePeriods);
    }
}
