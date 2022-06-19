namespace Skender.Stock.Indicators;

// PRICE VOLUME OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<PvoResult> GetPvo<TQuote>(
        this IEnumerable<TQuote> quotes,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Volume)
            .CalcPvo(fastPeriods, slowPeriods, signalPeriods);

    // given that this is volume-based, other chaining is moot
}
