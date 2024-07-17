namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (API)

public static partial class Ema
{
    // SERIES, from CHAIN
    public static IReadOnlyList<EmaResult> GetEma<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcEma(lookbackPeriods);

    // HUB, from Chain Provider
    public static EmaHub<T> ToEma<T>(
        this IChainProvider<T> chainProvider,
        int lookbackPeriods)
        where T : IReusable
        => new(chainProvider, lookbackPeriods);
}
