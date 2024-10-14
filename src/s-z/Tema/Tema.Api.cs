namespace Skender.Stock.Indicators;

// TRIPLE EXPONENTIAL MOVING AVERAGE - TEMA (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<TemaResult> GetTema<T>(
        this IReadOnlyList<T> results,
        int lookbackPeriods)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcTema(lookbackPeriods);
}
