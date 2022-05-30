namespace Skender.Stock.Indicators;

// BETA COEFFICIENT (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<BetaResult> GetBeta<TQuote>(
        IEnumerable<TQuote> quotesMarket,
        IEnumerable<TQuote> quotesEval,
        int lookbackPeriods,
        BetaType type = BetaType.Standard)
        where TQuote : IQuote
    {
        List<(DateTime, double)> tpListEval
            = quotesEval.ToBasicTuple(CandlePart.Close);

        List<(DateTime, double)> tpListMrkt
            = quotesMarket.ToBasicTuple(CandlePart.Close);

        // TODO: reverse API order (above), in next version,
        // to enable typical 'this' extension
        return CalcBeta(tpListEval, tpListMrkt, lookbackPeriods, type);
    }

    // SERIES, from TUPLE
    public static IEnumerable<BetaResult> GetBeta(
        this IEnumerable<(DateTime, double)> evalTuple,
        IEnumerable<(DateTime, double)> mrktTuple,
        int lookbackPeriods,
        BetaType type = BetaType.Standard)
    {
        List<(DateTime, double)> tpListEval = evalTuple.ToSortedList();
        List<(DateTime, double)> tpListMrkt = mrktTuple.ToSortedList();

        return CalcBeta(tpListEval, tpListMrkt, lookbackPeriods, type);
    }
}
