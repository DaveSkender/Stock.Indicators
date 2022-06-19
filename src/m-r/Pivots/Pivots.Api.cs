namespace Skender.Stock.Indicators;

// PIVOTS (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<PivotsResult> GetPivots<TQuote>(
        this IEnumerable<TQuote> quotes,
        int leftSpan = 2,
        int rightSpan = 2,
        int maxTrendPeriods = 20,
        EndType endType = EndType.HighLow)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcPivots(leftSpan, rightSpan, maxTrendPeriods, endType);
}
