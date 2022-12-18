using System.Collections.ObjectModel;

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
        Collection<(DateTime, double)> tpListBase = quotesBase
            .ToTuple(CandlePart.Close);
        Collection<(DateTime, double)> tpListEval = quotesEval
            .ToTuple(CandlePart.Close);

        return CalcPrs(tpListEval, tpListBase, lookbackPeriods, smaPeriods);
    }

    // SERIES, from TUPLE
    public static IEnumerable<PrsResult> GetPrs(
        this IEnumerable<(DateTime, double)> tupleEval,
        IEnumerable<(DateTime, double)> tupleBase,
        int? lookbackPeriods = null,
        int? smaPeriods = null)
    {
        Collection<(DateTime, double)> tpListBase = tupleBase.ToSortedCollection();
        Collection<(DateTime, double)> tpListEval = tupleEval.ToSortedCollection();

        return CalcPrs(tpListEval, tpListBase, lookbackPeriods, smaPeriods);
    }
}
