namespace Skender.Stock.Indicators;

// PRICE RELATIVE STRENGTH (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<PrsResult> GetPrs<TQuote>(
        this IEnumerable<TQuote> quotesEval,
        IEnumerable<TQuote> quotesBase,
        int? lookbackPeriods = null)
        where TQuote : IQuote
    {
        List<(DateTime, double)> tpListBase = quotesBase
            .ToTuple(CandlePart.Close);
        List<(DateTime, double)> tpListEval = quotesEval
            .ToTuple(CandlePart.Close);

        return CalcPrs(tpListEval, tpListBase, lookbackPeriods);
    }

    // SERIES, from CHAINS (both inputs reusable)
    public static IEnumerable<PrsResult> GetPrs(
        this IEnumerable<IReusableResult> quotesEval,
        IEnumerable<IReusableResult> quotesBase,
        int? lookbackPeriods = null)
    {
        List<(DateTime TickDate, double Value)> tpListEval
            = quotesEval.ToTupleResult();

        List<(DateTime TickDate, double Value)> tpListBase
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
