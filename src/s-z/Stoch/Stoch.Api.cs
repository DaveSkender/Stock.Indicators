namespace Skender.Stock.Indicators;

// STOCHASTIC OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from TQuote (standard)
    /// <include file='./info.xml' path='info/type[@name="Main"]/*' />
    ///
    public static IEnumerable<StochResult> GetStoch<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 3)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcStoch(
                lookbackPeriods,
                signalPeriods,
                smoothPeriods, 3, 2, MaType.SMA);

    // SERIES, from TQuote (extended)
    /// <include file='./info.xml' path='info/type[@name="Extended"]/*' />
    ///
    public static IEnumerable<StochResult> GetStoch<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods,
        double kFactor,
        double dFactor,
        MaType movingAverageType)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcStoch(
                lookbackPeriods,
                signalPeriods,
                smoothPeriods,
                kFactor,
                dFactor,
                movingAverageType);
}
