namespace Skender.Stock.Indicators;

// PRICE RELATIVE STRENGTH (API)
public static partial class Indicator
{
    // SERIES, from CHAINS (both inputs reusable)
    public static IEnumerable<PrsResult> GetPrs<T>(
        this IEnumerable<T> quotesEval,
        IEnumerable<T> quotesBase,
        int? lookbackPeriods = null)
        where T : IReusableResult
    {
        List<(DateTime Timestamp, double Value)> tpListEval
            = quotesEval.ToTupleResult();

        List<(DateTime Timestamp, double Value)> tpListBase
            = quotesBase.ToTupleResult();

        return CalcPrs(tpListEval, tpListBase, lookbackPeriods);
    }

    // SERIES, from TUPLE
    public static IEnumerable<PrsResult> GetPrs(
        this IEnumerable<(DateTime, double)> tupleEval,
        IEnumerable<(DateTime, double)> tupleBase,
        int? lookbackPeriods = null)
    {
        List<(DateTime, double)> tpListBase = tupleBase.ToSortedList();
        List<(DateTime, double)> tpListEval = tupleEval.ToSortedList();

        return CalcPrs(tpListEval, tpListBase, lookbackPeriods);
    }
}
