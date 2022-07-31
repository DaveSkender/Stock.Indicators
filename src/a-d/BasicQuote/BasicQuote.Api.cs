namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // BASE QUOTE (simplified quote)
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<BasicData> GetBaseQuote<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart = CandlePart.Close)
        where TQuote : IQuote => quotes
            .ToBasicData(candlePart);
}
