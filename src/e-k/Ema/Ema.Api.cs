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
            .ToTupleResult()
            .CalcEma(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<EmaResult> GetEma(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcEma(lookbackPeriods);

    // OBSERVER, from Quote Provider
    /// <include file='./info.xml' path='info/type[@name="observer"]/*' />
    ///
    public static Ema<BasicData> GetEma<TQuote>(
        this QuoteProvider<TQuote> provider,
        int lookbackPeriods)
        where TQuote : IQuote, new()
    {
        Use<TQuote> useObserver = provider
            .Use(CandlePart.Close);

        return new(useObserver, lookbackPeriods);
    }

    // OBSERVER, from Chain Provider
    /// <include file='./info.xml' path='info/type[@name="chainee"]/*' />
    ///
    public static Ema<TResult> GetEma<TResult>(
        this ChainProvider<TResult> chainProvider,
        int lookbackPeriods)
        where TResult : IReusableResult, new()
        => new(chainProvider, lookbackPeriods);
}
