namespace Skender.Stock.Indicators;

// STOCHASTIC MOMENTUM INDEX (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/type[@name="Main"]/*' />
    ///
    public static IReadOnlyList<SmiResult> ToSmi<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 13,
        int firstSmoothPeriods = 25,
        int secondSmoothPeriods = 2,
        int signalPeriods = 3)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcSmi(
                lookbackPeriods,
                firstSmoothPeriods,
                secondSmoothPeriods,
                signalPeriods);
}
