namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (API)

public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/type[@name="standard"]/*' />
    ///
    public static IEnumerable<EmaResult> GetEma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes
            .ToTuple(CandlePart.Close)
            .CalcEma(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<EmaResult> GetEma(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToTuple()
            .CalcEma(lookbackPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<EmaResult> GetEma(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcEma(lookbackPeriods);

    // OBSERVER, from Quote Provider
    /// <include file='./info.xml' path='info/type[@name="observer"]/*' />
    ///
    public static Ema GetEma<TQuote>(
        this QuoteProvider<TQuote> provider,
        int lookbackPeriods)
        where TQuote : IQuote, new()
    {
        var useObserver = provider
            .Use(CandlePart.Close);

        return new(useObserver, lookbackPeriods);
    }

    // OBSERVER, from Chain Provider
    /// <include file='./info.xml' path='info/type[@name="chainee"]/*' />
    ///
    public static Ema GetEma(
        this TupleProvider tupleProvider,
        int lookbackPeriods)
        => new(tupleProvider, lookbackPeriods);
}
