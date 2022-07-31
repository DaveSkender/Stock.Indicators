namespace Skender.Stock.Indicators;

// ULTIMATE OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<UltimateResult> GetUltimate<TQuote>(
        this IEnumerable<TQuote> quotes,
        int shortPeriods = 7,
        int middlePeriods = 14,
        int longPeriods = 28)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcUltimate(shortPeriods, middlePeriods, longPeriods);
}
