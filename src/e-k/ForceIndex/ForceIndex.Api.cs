namespace Skender.Stock.Indicators;

// FORCE INDEX (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<ForceIndexResult> GetForceIndex<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 2)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcForceIndex(lookbackPeriods);
}
