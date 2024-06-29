namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (API)

public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<SmaResult> GetSma<T>(
        this IEnumerable<T> source,
        int lookbackPeriods)
        where T : IReusable
        => source
            .ToSortedList()
            .CalcSma(lookbackPeriods);

    // OBSERVER, from Chain Provider
    public static Sma<TIn> ToSma<TIn>(
        this IChainProvider<TIn> chainProvider,
        int lookbackPeriods)
        where TIn : struct, IReusable
        => new(chainProvider, lookbackPeriods);
}
