namespace Skender.Stock.Indicators;

// TILLSON T3 MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<T3Result> GetT3<T>(
        this IEnumerable<T> results,
        int lookbackPeriods = 5,
        double volumeFactor = 0.7)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcT3(lookbackPeriods, volumeFactor);
}
