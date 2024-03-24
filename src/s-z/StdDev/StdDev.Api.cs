namespace Skender.Stock.Indicators;

// STANDARD DEVIATION (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<StdDevResult> GetStdDev<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes
            .ToTuple(CandlePart.Close)
            .CalcStdDev(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<StdDevResult> GetStdDev(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToTupleResult()
            .CalcStdDev(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<StdDevResult> GetStdDev(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcStdDev(lookbackPeriods);
}
