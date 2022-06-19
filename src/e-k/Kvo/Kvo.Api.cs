namespace Skender.Stock.Indicators;

// KLINGER VOLUME OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<KvoResult> GetKvo<TQuote>(
        this IEnumerable<TQuote> quotes,
        int fastPeriods = 34,
        int slowPeriods = 55,
        int signalPeriods = 13)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcKvo(fastPeriods, slowPeriods, signalPeriods);
}
