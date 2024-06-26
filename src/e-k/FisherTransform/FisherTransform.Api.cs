namespace Skender.Stock.Indicators;

// FISHER TRANSFORM (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<FisherTransformResult> GetFisherTransform<T>(
        this IEnumerable<T> results,
        int lookbackPeriods = 10)
        where T : IReusableResult
        => results
            .ToSortedList()
            .CalcFisherTransform(lookbackPeriods);
}
