namespace Skender.Stock.Indicators;

// ULCER INDEX (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<UlcerIndexResult> GetUlcerIndex<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcUlcerIndex(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<UlcerIndexResult> GetUlcerIndex(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToResultTuple()
            .CalcUlcerIndex(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<UlcerIndexResult> GetUlcerIndex(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcUlcerIndex(lookbackPeriods);
}
