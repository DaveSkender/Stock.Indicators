namespace Skender.Stock.Indicators;

// KELTNER CHANNELS (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<KeltnerResult> GetKeltner<TQuote>(
        this IEnumerable<TQuote> quotes,
        int emaPeriods = 20,
        double multiplier = 2,
        int atrPeriods = 10)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcKeltner(emaPeriods, multiplier, atrPeriods);
}
