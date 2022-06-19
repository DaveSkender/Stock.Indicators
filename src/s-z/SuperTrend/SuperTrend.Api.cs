namespace Skender.Stock.Indicators;

// SUPERTREND (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<SuperTrendResult> GetSuperTrend<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 10,
        double multiplier = 3)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcSuperTrend(lookbackPeriods, multiplier);
}
