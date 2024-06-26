namespace Skender.Stock.Indicators;

// TRIPLE EXPONENTIAL MOVING AVERAGE - TEMA (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<TemaResult> GetTema<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusableResult
        => results
            .ToSortedList()
            .CalcTema(lookbackPeriods);
}
