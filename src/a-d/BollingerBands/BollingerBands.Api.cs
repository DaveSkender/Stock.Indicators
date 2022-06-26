namespace Skender.Stock.Indicators;

// BOLLINGER BANDS (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<BollingerBandsResult> GetBollingerBands<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcBollingerBands(lookbackPeriods, standardDeviations);

    // SERIES, from CHAIN
    public static IEnumerable<BollingerBandsResult> GetBollingerBands(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods = 20,
        double standardDeviations = 2) => results
            .ToResultTuple()
            .CalcBollingerBands(lookbackPeriods, standardDeviations)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<BollingerBandsResult> GetBollingerBands(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods = 20,
        double standardDeviations = 2) => priceTuples
            .ToSortedList()
            .CalcBollingerBands(lookbackPeriods, standardDeviations);
}
