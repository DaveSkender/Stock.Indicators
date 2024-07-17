namespace Skender.Stock.Indicators;

// BETA COEFFICIENT (API)
public static partial class Indicator
{
    // SERIES, from CHAINS (both inputs reusable)
    public static IReadOnlyList<BetaResult> GetBeta<T>(
        this IEnumerable<T> evalSource,
        IEnumerable<T> mrktSource,
        int lookbackPeriods,
        BetaType type = BetaType.Standard)
        where T : IReusable
    {
        List<T> listEval
            = evalSource.ToSortedList();

        List<T> listMrkt
            = mrktSource.ToSortedList();

        return CalcBeta(listEval, listMrkt, lookbackPeriods, type);
    }
}
