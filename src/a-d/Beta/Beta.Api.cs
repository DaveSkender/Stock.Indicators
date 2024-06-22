namespace Skender.Stock.Indicators;

// BETA COEFFICIENT (API)
public static partial class Indicator
{
    // SERIES, from CHAINS (both inputs reusable)
    public static IEnumerable<BetaResult> GetBeta<T>(
        this IEnumerable<T> evalResults,
        IEnumerable<T> mrktResults,
        int lookbackPeriods,
        BetaType type = BetaType.Standard)
        where T : IReusableResult
    {
        List<(DateTime Timestamp, double Value)> tpListEval
            = evalResults.ToTupleResult();

        List<(DateTime Timestamp, double Value)> tpListMrkt
            = mrktResults.ToTupleResult();

        return CalcBeta(tpListEval, tpListMrkt, lookbackPeriods, type);
    }

    // SERIES, from TUPLE
    public static IEnumerable<BetaResult> GetBeta(
        this IEnumerable<(DateTime, double)> evalTuple,
        IEnumerable<(DateTime, double)> mrktTuple,
        int lookbackPeriods,
        BetaType type = BetaType.Standard)
    {
        List<(DateTime, double)> tpListEval
            = evalTuple.ToSortedList();

        List<(DateTime, double)> tpListMrkt
            = mrktTuple.ToSortedList();

        return CalcBeta(tpListEval, tpListMrkt, lookbackPeriods, type);
    }
}
