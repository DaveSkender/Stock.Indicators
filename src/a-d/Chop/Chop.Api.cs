namespace Skender.Stock.Indicators;

// CHOPPINESS INDEX (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<ChopResult> GetChop<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcChop(lookbackPeriods);
}
