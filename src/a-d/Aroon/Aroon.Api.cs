namespace Skender.Stock.Indicators;

// AROON OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IReadOnlyList<AroonResult> GetAroon<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 25)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcAroon(lookbackPeriods);
}
