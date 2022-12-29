namespace Skender.Stock.Indicators;

// BETA COEFFICIENT (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<BetaResult> GetBeta<TQuote>(
        this IEnumerable<TQuote> quotesEval,
        IEnumerable<TQuote> quotesMarket,
        int lookbackPeriods,
        BetaType type = BetaType.Standard)
        where TQuote : IQuote
    {
        List<(DateTime, double)> tpListEval
            = quotesEval.ToTuple(CandlePart.Close);

        List<(DateTime, double)> tpListMrkt
            = quotesMarket.ToTuple(CandlePart.Close);

        // to enable typical 'this' extension
        return CalcBeta(tpListEval, tpListMrkt, lookbackPeriods, type);
    }

    // SERIES, from CHAINS (both inputs reusable)
    public static IEnumerable<BetaResult> GetBeta(
        this IEnumerable<IReusableResult> evalResults,
        IEnumerable<IReusableResult> mrktResults,
        int lookbackPeriods,
        BetaType type = BetaType.Standard)
    {
        List<(DateTime Date, double Value)> tpListEval
            = evalResults.ToTuple();

        List<(DateTime Date, double Value)> tpListMrkt
            = mrktResults.ToTuple();

        return CalcBeta(tpListEval, tpListMrkt, lookbackPeriods, type)
            .SyncIndex(evalResults, SyncType.Prepend);
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
