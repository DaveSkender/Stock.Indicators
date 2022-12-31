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
            = quotesA.ToTuple(CandlePart.Close);

        List<(DateTime, double)> tpListB
            = quotesB.ToTuple(CandlePart.Close);

        return CalcCorrelation(tpListA, tpListB, lookbackPeriods);
    }

    // SERIES, from CHAINS (both inputs reusable)
    public static IEnumerable<CorrResult> GetCorrelation(
        this IEnumerable<IReusableResult> quotesA,
        IEnumerable<IReusableResult> quotesB,
        int lookbackPeriods)
    {
        List<(DateTime Date, double Value)> tpListA
            = quotesA.ToTuple();

        List<(DateTime Date, double Value)> tpListB
            = quotesB.ToTuple();

        return CalcCorrelation(tpListA, tpListB, lookbackPeriods)
            .SyncIndex(quotesA, SyncType.Prepend);
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
