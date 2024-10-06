namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (API)

public static partial class Sma
{
    // SERIES, from CHAIN
    public static IReadOnlyList<SmaResult> GetSma<T>(
        this IEnumerable<T> source,
        int lookbackPeriods)
        where T : IReusable
        => source
            .ToSortedList()
            .CalcSma(lookbackPeriods);

    // OBSERVER, from Chain Provider
    public static SmaHub<TIn> ToSma<TIn>(
        this IChainProvider<TIn> chainProvider,
        int lookbackPeriods)
        where TIn : IReusable
        => new(chainProvider, lookbackPeriods);
}
