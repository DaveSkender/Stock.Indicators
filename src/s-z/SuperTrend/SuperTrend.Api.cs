namespace Skender.Stock.Indicators;

// SUPERTREND (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IReadOnlyList<SuperTrendResult> ToSuperTrend<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 10,
        double multiplier = 3)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcSuperTrend(lookbackPeriods, multiplier);
}
