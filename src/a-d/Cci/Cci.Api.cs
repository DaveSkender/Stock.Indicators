namespace Skender.Stock.Indicators;

// COMMODITY CHANNEL INDEX (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IReadOnlyList<CciResult> ToCci<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 20)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcCci(lookbackPeriods);
}
