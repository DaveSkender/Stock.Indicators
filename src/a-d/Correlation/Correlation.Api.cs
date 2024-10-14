namespace Skender.Stock.Indicators;

// CORRELATION COEFFICIENT (API)
public static partial class Correlation
{
    // SERIES, from CHAINS (both inputs reusable)
    public static IReadOnlyList<CorrResult> ToCorrelation<T>(
        this IEnumerable<T> sourceA,
        IEnumerable<T> sourceB,
        int lookbackPeriods)
        where T : IReusable
        => CalcCorrelation(
            sourceA.ToSortedList(),
            sourceB.ToSortedList(),
            lookbackPeriods);
}
