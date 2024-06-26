namespace Skender.Stock.Indicators;

// ENDPOINT MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<EpmaResult> GetEpma<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusableResult
        => results
            .ToSortedList()
            .CalcEpma(lookbackPeriods);
}
