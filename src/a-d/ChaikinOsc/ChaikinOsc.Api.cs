namespace Skender.Stock.Indicators;

// CHAIKIN OSCILLATOR
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<ChaikinOscResult> GetChaikinOsc<TQuote>(
        this IEnumerable<TQuote> quotes,
        int fastPeriods = 3,
        int slowPeriods = 10)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcChaikinOsc(fastPeriods, slowPeriods);
}
