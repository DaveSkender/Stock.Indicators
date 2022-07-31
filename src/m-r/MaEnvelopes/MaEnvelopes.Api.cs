namespace Skender.Stock.Indicators;

// MOVING AVERAGE ENVELOPES (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<MaEnvelopeResult> GetMaEnvelopes<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        double percentOffset = 2.5,
        MaType movingAverageType = MaType.SMA)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcMaEnvelopes(lookbackPeriods, percentOffset, movingAverageType);

    // SERIES, from CHAIN
    public static IEnumerable<MaEnvelopeResult> GetMaEnvelopes(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods,
        double percentOffset = 2.5,
        MaType movingAverageType = MaType.SMA) => results
            .ToResultTuple()
            .CalcMaEnvelopes(lookbackPeriods, percentOffset, movingAverageType)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<MaEnvelopeResult> GetMaEnvelopes(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods,
        double percentOffset = 2.5,
        MaType movingAverageType = MaType.SMA) => priceTuples
            .ToSortedList()
            .CalcMaEnvelopes(lookbackPeriods, percentOffset, movingAverageType);
}
