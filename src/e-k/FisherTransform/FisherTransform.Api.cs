namespace Skender.Stock.Indicators;

// FISHER TRANSFORM (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<FisherTransformResult> GetFisherTransform<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 10)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.HL2)
            .CalcFisherTransform(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<FisherTransformResult> GetFisherTransform(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToResultTuple()
            .CalcFisherTransform(lookbackPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<FisherTransformResult> GetFisherTransform(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcFisherTransform(lookbackPeriods);
}
