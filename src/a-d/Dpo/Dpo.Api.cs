namespace Skender.Stock.Indicators;

// DETRENDED PRICE OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<DpoResult> GetDpo<T>(
        this IReadOnlyList<T> results,
        int lookbackPeriods)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcDpo(lookbackPeriods);
}
