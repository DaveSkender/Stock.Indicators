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
            .ToBasicTuple(CandlePart.Close)
            .CalcEma(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<EmaResult> GetEma(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToResultTuple()
            .CalcEma(lookbackPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<EmaResult> GetEma(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcEma(lookbackPeriods);

    // STREAM INITIALIZATION, from TQuote
    /// <include file='./info.xml' path='info/type[@name="stream"]/*' />
    ///
    internal static EmaBase InitEma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<(DateTime, double)> tpList
            = quotes.ToBasicTuple(CandlePart.Close);

        return new EmaBase(tpList, lookbackPeriods);
    }

    // STREAM INITIALIZATION, from CHAIN
    internal static EmaBase InitEma(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods)
    {
        // convert results
        List<(DateTime, double)> tpList
            = results.ToResultTuple();

        return new EmaBase(tpList, lookbackPeriods);
    }
}
