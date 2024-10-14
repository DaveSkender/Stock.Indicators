namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (API)

public static partial class Sma
{
    // OBSERVER, from Chain Provider
    public static SmaHub<TIn> ToSma<TIn>(
        this IChainProvider<TIn> chainProvider,
        int lookbackPeriods)
        where TIn : IReusable
        => new(chainProvider, lookbackPeriods);
}
