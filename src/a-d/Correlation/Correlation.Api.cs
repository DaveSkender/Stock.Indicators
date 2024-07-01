namespace Skender.Stock.Indicators;

// CORRELATION COEFFICIENT (API)
public static partial class Indicator
{
    // SERIES, from CHAINS (both inputs reusable)
    public static IEnumerable<CorrResult> GetCorrelation<T>(
        this IEnumerable<T> sourceA,
        IEnumerable<T> sourceB,
        int lookbackPeriods)
        where T : IReusable
        => CalcCorrelation(
            sourceA.ToSortedList(),
            sourceB.ToSortedList(),
            lookbackPeriods);
}
