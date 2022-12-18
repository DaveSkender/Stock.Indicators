namespace Skender.Stock.Indicators;

// PIVOT POINTS (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<PivotPointsResult> GetPivotPoints<TQuote>(
        this IEnumerable<TQuote> quotes,
        PeriodSize windowSize,
        PivotPointType pointType = PivotPointType.Standard)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcPivotPoints(windowSize, pointType);
}
