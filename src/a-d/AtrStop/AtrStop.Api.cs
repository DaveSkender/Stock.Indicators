namespace Skender.Stock.Indicators;

// ATR TRAILING STOP (API)

public static partial class AtrStop
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<AtrStopResult> GetAtrStop<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 21,
        double multiplier = 3,
        EndType endType = EndType.Close)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcAtrStop(lookbackPeriods, multiplier, endType);

    // OBSERVER, from Quote Provider
    public static AtrStopHub<TIn> ToAtrStop<TIn>(
        this IQuoteProvider<TIn> quoteProvider,
        int lookbackPeriods = 21,
        double multiplier = 3,
        EndType endType = EndType.Close)
        where TIn : IQuote
        => new(quoteProvider, lookbackPeriods, multiplier, endType);
}
