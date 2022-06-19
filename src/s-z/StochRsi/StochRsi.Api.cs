namespace Skender.Stock.Indicators;

// STOCHASTIC RSI (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<StochRsiResult> GetStochRsi<TQuote>(
        this IEnumerable<TQuote> quotes,
        int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods = 1)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcStochRsi(
                rsiPeriods,
                stochPeriods,
                signalPeriods,
                smoothPeriods);
}
