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

    // STREAM INITIALIZATION, from TQuote
    /// <include file='./info.xml' path='info/type[@name="stream"]/*' />
    ///
    public static EmaObs ObsEma(
        this QuoteProvider provider,
        int lookbackPeriods) => new(provider, lookbackPeriods);

#pragma warning disable SA1005 // Single line comments should begin with single space

    // // STREAM INITIALIZATION, from CHAIN
    // public static EmaObs InitEma(
    //     this IEnumerable<IReusableResult> results,
    //     int lookbackPeriods)
    // {
    //     // convert results
    //     List<(DateTime, double)> tpList
    //         = results.ToTuple();
    //
    //
    //     // new base instance
    //     EmaObs baseEma = new(lookbackPeriods);

    //     // prime the results
    //     for (int i = 0; i < tpList.Count; i++)
    //     {
    //         baseEma.Add(tpList[i]);
    //     }

    //     return baseEma;
    // }

    // // STREAM INITIALIZATION, from Tuple
    // public static EmaObs InitEma(
    //     this IEnumerable<(DateTime, double)> priceTuples,
    //     int lookbackPeriods)
    // {
    //     // convert quotes
    //     List<(DateTime, double)> tpList
    //         = priceTuples.ToSortedList();

    //     // new base instance
    //     EmaObs baseEma = new(lookbackPeriods);

    //     // prime the results
    //     for (int i = 0; i < tpList.Count; i++)
    //     {
    //         baseEma.Add(tpList[i]);
    //     }

    //     return baseEma;
    // }
#pragma warning restore SA1005 // Single line comments should begin with single space
}
