namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (API)

public static partial class Ema
{
    // HUB, from Chain Provider
    public static EmaHub<T> ToEma<T>(
        this IChainProvider<T> chainProvider,
        int lookbackPeriods)
        where T : IReusable
        => new(chainProvider, lookbackPeriods);
}

public interface IEma
{
    int LookbackPeriods { get; }
    double K { get; }
}
