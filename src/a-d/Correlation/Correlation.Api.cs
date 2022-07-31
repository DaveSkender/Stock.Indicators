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
        List<(DateTime, double)> tpListA
            = quotesA.ToBasicTuple(CandlePart.Close);

        List<(DateTime, double)> tpListB
            = quotesB.ToBasicTuple(CandlePart.Close);

        return CalcCorrelation(tpListA, tpListB, lookbackPeriods);
    }

    // SERIES, from TUPLE
    public static IEnumerable<CorrResult> GetCorrelation(
        this IEnumerable<(DateTime, double)> tuplesA,
        IEnumerable<(DateTime, double)> tuplesB,
        int lookbackPeriods)
    {
        List<(DateTime, double)> tpListA = tuplesA.ToSortedList();
        List<(DateTime, double)> tpListB = tuplesB.ToSortedList();

        return CalcCorrelation(tpListA, tpListB, lookbackPeriods);
    }
}
