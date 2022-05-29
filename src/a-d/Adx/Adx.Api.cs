namespace Skender.Stock.Indicators;

// AVERAGE DIRECTIONAL INDEX (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<AdxResult> GetAdx<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote => quotes
            .SortToList()
            .CalcAdx(lookbackPeriods);
}
