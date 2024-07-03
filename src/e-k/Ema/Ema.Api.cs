namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (API)

public static partial class Ema
{
    // SERIES, from CHAIN
    public static IEnumerable<EmaResult> GetEma<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcEma(lookbackPeriods);

    // OBSERVER, from Chain Provider
    public static EmaHub<TIn> ToEma<TIn>(
        this ChainProvider<TIn> chainProvider,
        int lookbackPeriods)
        where TIn : struct, IReusable
        => new(chainProvider, lookbackPeriods);
}
