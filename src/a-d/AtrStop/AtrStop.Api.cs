namespace Skender.Stock.Indicators;

// ATR TRAILING STOP (API)
public static partial class Indicator
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
            .ToQuoteD()
            .CalcAtrStop(lookbackPeriods, multiplier, endType);
}
