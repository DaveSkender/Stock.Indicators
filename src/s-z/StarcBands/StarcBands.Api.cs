namespace Skender.Stock.Indicators;

// STARC BANDS (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<StarcBandsResult> GetStarcBands<TQuote>(
        this IEnumerable<TQuote> quotes,
        int smaPeriods = 20,
        double multiplier = 2,
        int atrPeriods = 10)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcStarcBands(smaPeriods, multiplier, atrPeriods);
}
