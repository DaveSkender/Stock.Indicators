namespace Skender.Stock.Indicators;

// ENDPOINT MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<EpmaResult> GetEpma<TQuote>(
    this IEnumerable<TQuote> quotes,
    int lookbackPeriods)
    where TQuote : IQuote => quotes
        .ToBasicTuple()
        .CalcEpma(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<EpmaResult> GetEpma(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToResultTuple()
            .CalcEpma(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<EpmaResult> GetEpma(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToTupleList()
            .CalcEpma(lookbackPeriods);
}