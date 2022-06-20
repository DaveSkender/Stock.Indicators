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
        int? lookbackPeriods = null,
        int? smaPeriods = null)
        where TQuote : IQuote
    {
        List<(DateTime, double)> tpListBase = quotesBase
            .ToBasicTuple(CandlePart.Close);
        List<(DateTime, double)> tpListEval = quotesEval
            .ToBasicTuple(CandlePart.Close);

        return CalcPrs(tpListEval, tpListBase, lookbackPeriods, smaPeriods);
    }

    // SERIES, from TUPLE
    public static IEnumerable<PrsResult> GetPrs(
        this IEnumerable<(DateTime, double)> tupleEval,
        IEnumerable<(DateTime, double)> tupleBase,
        int? lookbackPeriods = null,
        int? smaPeriods = null)
    {
        List<(DateTime, double)> tpListBase = tupleBase.ToSortedList();
        List<(DateTime, double)> tpListEval = tupleEval.ToSortedList();

        return CalcPrs(tpListEval, tpListBase, lookbackPeriods, smaPeriods);
    }
}
