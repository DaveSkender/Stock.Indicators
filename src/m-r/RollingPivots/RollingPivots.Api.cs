namespace Skender.Stock.Indicators;

// ROLLING PIVOT POINTS (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IReadOnlyList<RollingPivotsResult> ToRollingPivots<TQuote>(
        this IEnumerable<TQuote> quotes,
        int windowPeriods,
        int offsetPeriods,
        PivotPointType pointType = PivotPointType.Standard)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcRollingPivots(windowPeriods, offsetPeriods, pointType);
}
