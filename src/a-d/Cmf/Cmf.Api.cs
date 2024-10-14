namespace Skender.Stock.Indicators;

// CHAIKIN MONEY FLOW (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IReadOnlyList<CmfResult> ToCmf<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 20)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcCmf(lookbackPeriods);
}
