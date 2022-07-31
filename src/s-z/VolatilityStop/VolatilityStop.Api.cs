namespace Skender.Stock.Indicators;

// VOLATILITY SYSTEM/STOP (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<VolatilityStopResult> GetVolatilityStop<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 7,
        double multiplier = 3)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcVolatilityStop(lookbackPeriods, multiplier);
}
