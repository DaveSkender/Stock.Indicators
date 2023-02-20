namespace Skender.Stock.Indicators;

// NewHighsNewLows (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<NewHighsNewLowsResult> GetNewHighsNewLows<TQuote>(
        this IEnumerable<TQuote> quotes,
        int tradingDays = 252)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .GetNewHighsNewLows(tradingDays);


    // SERIES, from IEnumerable<IEnumerable<NewHighsNewLowsResult>>
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<NewHighsNewLowsResult> GetNewHighsNewLows(
        this IEnumerable<IEnumerable<NewHighsNewLowsResult>> newHighsNewLowsResults) => newHighsNewLowsResults
            .GetNewHighsNewLows();
}
