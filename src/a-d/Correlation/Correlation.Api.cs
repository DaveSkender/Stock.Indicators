using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// CORRELATION COEFFICIENT (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<CorrResult> GetCorrelation<TQuote>(
        this IEnumerable<TQuote> quotesA,
        IEnumerable<TQuote> quotesB,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        Collection<(DateTime, double)> tpListA
            = quotesA.ToTuple(CandlePart.Close);

        Collection<(DateTime, double)> tpListB
            = quotesB.ToTuple(CandlePart.Close);

        return CalcCorrelation(tpListA, tpListB, lookbackPeriods);
    }

    // SERIES, from TUPLE
    public static IEnumerable<CorrResult> GetCorrelation(
        this IEnumerable<(DateTime, double)> tuplesA,
        IEnumerable<(DateTime, double)> tuplesB,
        int lookbackPeriods)
    {
        Collection<(DateTime, double)> tpListA = tuplesA.ToSortedCollection();
        Collection<(DateTime, double)> tpListB = tuplesB.ToSortedCollection();

        return CalcCorrelation(tpListA, tpListB, lookbackPeriods);
    }
}
