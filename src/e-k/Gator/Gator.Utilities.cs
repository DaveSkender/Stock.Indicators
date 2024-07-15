namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="ReusableUtility.Condense{T}(IEnumerable{T})"/>
    public static IEnumerable<GatorResult> Condense(
        this IEnumerable<GatorResult> results)
    {
        List<GatorResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.Upper is null && x.Lower is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<GatorResult> RemoveWarmupPeriods(
        this IEnumerable<GatorResult> results) => results.Remove(150);
}
