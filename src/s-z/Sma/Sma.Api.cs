namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (API)

public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/type[@name="standard"]/*' />
    ///
    public static IEnumerable<SmaResult> GetSma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes
            .ToTuple(CandlePart.Close)
            .CalcSma(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<SmaResult> GetSma(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToTuple()
            .CalcSma(lookbackPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<SmaResult> GetSma(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcSma(lookbackPeriods);

    // OBSERVER, from Quote Provider
    /// <include file='./info.xml' path='info/type[@name="observer"]/*' />
    ///
    public static Sma GetSma<TQuote>(
        this QuoteProvider<TQuote> provider,
        int lookbackPeriods)
        where TQuote:IQuote,new()
    {
        Use<TQuote> useObserver = provider
            .Use<TQuote>(CandlePart.Close);

        return new(useObserver, lookbackPeriods);
    }

    // OBSERVER, from Chain Provider
    /// <include file='./info.xml' path='info/type[@name="chainee"]/*' />
    ///
    public static Sma GetSma(
        this TupleProvider tupleProvider,
        int lookbackPeriods)
        => new(tupleProvider, lookbackPeriods);

    /// <include file='./info.xml' path='info/type[@name="Analysis"]/*' />
    ///
    // ANALYSIS, from TQuote
    public static IEnumerable<SmaAnalysis> GetSmaAnalysis<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes
            .ToTuple(CandlePart.Close)
            .CalcSmaAnalysis(lookbackPeriods);

    // ANALYSIS, from CHAIN
    public static IEnumerable<SmaAnalysis> GetSmaAnalysis(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToTuple()
            .CalcSmaAnalysis(lookbackPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // ANALYSIS, from TUPLE
    public static IEnumerable<SmaAnalysis> GetSmaAnalysis(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcSmaAnalysis(lookbackPeriods);
}
