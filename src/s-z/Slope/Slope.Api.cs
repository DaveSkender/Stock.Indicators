namespace Skender.Stock.Indicators;

// SLOPE AND LINEAR REGRESSION (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<SlopeResult> ToSlope<T>(
        this IReadOnlyList<T> results,
        int lookbackPeriods)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcSlope(lookbackPeriods);
}
