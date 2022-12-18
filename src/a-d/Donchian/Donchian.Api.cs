namespace Skender.Stock.Indicators;

// DONCHIAN CHANNEL (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<DonchianResult> GetDonchian<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 20)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcDonchian(lookbackPeriods);
}
