namespace Skender.Stock.Indicators;

// ELDER-RAY (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IReadOnlyList<ElderRayResult> ToElderRay<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 13)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcElderRay(lookbackPeriods);
}
