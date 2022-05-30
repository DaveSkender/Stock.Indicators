namespace Skender.Stock.Indicators;

// AVERAGE TRUE RANGE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<AtrResult> GetAtr<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcAtr(lookbackPeriods);
}
