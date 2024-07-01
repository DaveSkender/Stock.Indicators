namespace Skender.Stock.Indicators;

// AVERAGE TRUE RANGE (API)
public static partial class Atr
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<AtrResult> GetAtr<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcAtr(lookbackPeriods);
}
