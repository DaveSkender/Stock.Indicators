using System.Collections.ObjectModel;

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
        Collection<(DateTime, double)> tpListEval
            = quotesEval.ToTuple(CandlePart.Close);

        Collection<(DateTime, double)> tpListMrkt
            = quotesMarket.ToTuple(CandlePart.Close);

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
        Collection<(DateTime, double)> tpListEval = evalTuple.ToSortedCollection();
        Collection<(DateTime, double)> tpListMrkt = mrktTuple.ToSortedCollection();

        return CalcBeta(tpListEval, tpListMrkt, lookbackPeriods, type);
    }
}
