namespace Skender.Stock.Indicators;

// STOCHASTIC MOMENTUM INDEX (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/type[@name="Main"]/*' />
    ///
    public static IEnumerable<SmiResult> GetSmi<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int firstSmoothPeriods,
        int secondSmoothPeriods,
        int signalPeriods = 3)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcSmi(
                lookbackPeriods,
                firstSmoothPeriods,
                secondSmoothPeriods,
                signalPeriods);
}
