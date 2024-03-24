namespace Skender.Stock.Indicators;

// TRIPLE EMA OSCILLATOR - TRIX (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<TrixResult> GetTrix<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes
            .ToTuple(CandlePart.Close)
            .CalcTrix(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<TrixResult> GetTrix(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToTupleResult()
            .CalcTrix(lookbackPeriods);
    // SERIES, from TUPLE
    public static IEnumerable<TrixResult> GetTrix(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcTrix(lookbackPeriods);
}
