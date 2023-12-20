namespace Skender.Stock.Indicators;

// RELATIVE STRENGTH INDEX (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<RsiResult> GetRsi<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote => quotes
            .ToTuple(CandlePart.Close)
            .CalcRsi(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<RsiResult> GetRsi(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods = 14) => results
            .ToTuple()
            .CalcRsi(lookbackPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<RsiResult> GetRsi(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods = 14) => priceTuples
            .ToSortedList()
            .CalcRsi(lookbackPeriods);
}
