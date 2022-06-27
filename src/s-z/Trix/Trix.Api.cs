namespace Skender.Stock.Indicators;

// TRIPLE EMA OSCILLATOR - TRIX (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<TrixResult> GetTrix<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int? signalPeriods = null)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcTrix(lookbackPeriods, signalPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<TrixResult> GetTrix(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods,
        int? signalPeriods = null) => results
            .ToResultTuple()
            .CalcTrix(lookbackPeriods, signalPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<TrixResult> GetTrix(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods,
        int? signalPeriods = null) => priceTuples
            .ToSortedList()
            .CalcTrix(lookbackPeriods, signalPeriods);
}
