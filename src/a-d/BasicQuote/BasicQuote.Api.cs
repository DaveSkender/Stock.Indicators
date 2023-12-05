namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // BASE QUOTE (specified candle part)
    /// <include file='./info.xml' path='info/type[@name="standard"]/*' />
    ///
    public static IEnumerable<BasicResult> GetBaseQuote<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote => quotes
            .Select(q => q.ToBasicData(candlePart))
            .OrderBy(x => x.Date);

    // BASE QUOTE (default to Close)
    /// <include file='./info.xml' path='info/type[@name="default-close"]/*' />
    ///
    public static IEnumerable<BasicResult> GetBaseQuote<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => quotes
            .Select(q => q.ToBasicData(CandlePart.Close))
            .OrderBy(x => x.Date);
}
