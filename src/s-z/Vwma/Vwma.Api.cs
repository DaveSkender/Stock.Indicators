namespace Skender.Stock.Indicators;

// VOLUME WEIGHTED MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<VwmaResult> GetVwma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcVwma(lookbackPeriods);
}
