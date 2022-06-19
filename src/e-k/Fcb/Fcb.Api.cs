namespace Skender.Stock.Indicators;

// FRACTAL CHAOS BANDS (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<FcbResult> GetFcb<TQuote>(
        this IEnumerable<TQuote> quotes,
        int windowSpan = 2)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcFcb(windowSpan);
}
