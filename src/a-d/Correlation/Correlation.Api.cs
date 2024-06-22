namespace Skender.Stock.Indicators;

// CORRELATION COEFFICIENT (API)
public static partial class Indicator
{
    // SERIES, from CHAINS (both inputs reusable)
    public static IEnumerable<CorrResult> GetCorrelation<T>(
        this IEnumerable<T> quotesA,
        IEnumerable<T> quotesB,
        int lookbackPeriods)
        where T : IReusableResult
    {
        List<(DateTime Timestamp, double Value)> tpListA
            = quotesA.ToTupleResult();

        List<(DateTime Timestamp, double Value)> tpListB
            = quotesB.ToTupleResult();

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
