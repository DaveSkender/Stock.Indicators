namespace Skender.Stock.Indicators;

// ICHIMOKU CLOUD (API)
public static partial class Indicator
{
    /// <include file='./info.xml' path='info/type[@name="Standard"]/*' />
    ///
    public static IEnumerable<IchimokuResult> GetIchimoku<TQuote>(
        this IEnumerable<TQuote> quotes,
        int tenkanPeriods = 9,
        int kijunPeriods = 26,
        int senkouBPeriods = 52)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcIchimoku(
                tenkanPeriods,
                kijunPeriods,
                senkouBPeriods,
                kijunPeriods,
                kijunPeriods);

    /// <include file='./info.xml' path='info/type[@name="Extended"]/*' />
    ///
    public static IEnumerable<IchimokuResult> GetIchimoku<TQuote>(
        this IEnumerable<TQuote> quotes,
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int offsetPeriods)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcIchimoku(
                tenkanPeriods,
                kijunPeriods,
                senkouBPeriods,
                offsetPeriods,
                offsetPeriods);

    /// <include file='./info.xml' path='info/type[@name="Full"]/*' />
    ///
    public static IEnumerable<IchimokuResult> GetIchimoku<TQuote>(
        this IEnumerable<TQuote> quotes,
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int senkouOffset,
        int chikouOffset)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcIchimoku(
                tenkanPeriods,
                kijunPeriods,
                senkouBPeriods,
                senkouOffset,
                chikouOffset);
}
