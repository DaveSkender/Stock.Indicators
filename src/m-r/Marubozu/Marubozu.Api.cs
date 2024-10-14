namespace Skender.Stock.Indicators;

// MARUBOZU (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IReadOnlyList<CandleResult> ToMarubozu<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        double minBodyPercent = 95)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcMarubozu(minBodyPercent);
}
