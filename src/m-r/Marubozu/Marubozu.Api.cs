namespace Skender.Stock.Indicators;

// MARUBOZU (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<CandleResult> GetMarubozu<TQuote>(
        this IEnumerable<TQuote> quotes,
        double minBodyPercent = 95)
        where TQuote : IQuote => quotes
            .CalcMarubozu(minBodyPercent);
}
