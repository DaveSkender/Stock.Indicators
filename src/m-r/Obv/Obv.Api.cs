namespace Skender.Stock.Indicators;

// ON-BALANCE VOLUME (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IReadOnlyList<ObvResult> ToObv<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcObv();
}
