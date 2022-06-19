namespace Skender.Stock.Indicators;

// ELDER-RAY (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<ElderRayResult> GetElderRay<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 13)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcElderRay(lookbackPeriods);
}
