namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (API)
// with extended analysis

public static partial class Indicator
{
    // ANALYSIS, from CHAIN
    public static IReadOnlyList<SmaAnalysis> ToSmaAnalysis<T>(
        this IEnumerable<T> source,
        int lookbackPeriods)
        where T : IReusable
        => source
            .ToSortedList()
            .CalcSmaAnalysis(lookbackPeriods);
}
