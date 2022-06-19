namespace Skender.Stock.Indicators;

// DOJI (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<CandleResult> GetDoji<TQuote>(
        this IEnumerable<TQuote> quotes,
        double maxPriceChangePercent = 0.1)
        where TQuote : IQuote => quotes
            .CalcDoji(maxPriceChangePercent);
}
