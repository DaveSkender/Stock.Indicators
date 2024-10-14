namespace Skender.Stock.Indicators;

// HURST EXPONENT (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<HurstResult> GetHurst<T>(
        this IReadOnlyList<T> results,
        int lookbackPeriods = 100)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcHurst(lookbackPeriods);
}
