namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (API)

public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<SmaResult> GetSma<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusableResult
        => results
            .ToTupleResult()
            .CalcSma(lookbackPeriods);

    // ANALYSIS, from CHAIN
    public static IEnumerable<SmaAnalysis> GetSmaAnalysis<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusableResult
        => results
            .ToTupleResult()
            .CalcSmaAnalysis(lookbackPeriods);

    // OBSERVER, from Chain Provider
    public static Sma<TIn> ToSma<TIn>(
        this IChainProvider<TIn> chainProvider,
        int lookbackPeriods)
        where TIn : struct, IReusableResult
        => new(chainProvider, lookbackPeriods);
}
