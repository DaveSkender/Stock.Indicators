namespace Skender.Stock.Indicators;

// PRICE VOLUME OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IReadOnlyList<PvoResult> ToPvo<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
        where TQuote : IQuote => quotes
            .Use(CandlePart.Volume)
            .ToList()
            .CalcPvo(fastPeriods, slowPeriods, signalPeriods);

    // given that this is volume-based, other chaining is moot
}
