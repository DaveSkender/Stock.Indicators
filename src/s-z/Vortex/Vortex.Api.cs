namespace Skender.Stock.Indicators;

// VORTEX INDICATOR (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<VortexResult> GetVortex<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcVortex(lookbackPeriods);
}
