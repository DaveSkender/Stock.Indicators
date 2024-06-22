namespace Skender.Stock.Indicators;

// FISHER TRANSFORM (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<FisherTransformResult> GetFisherTransform<T>(
        this IEnumerable<T> results,
        int lookbackPeriods = 10)
        where T: IReusableResult
        => results
            .ToTupleResult()
            .CalcFisherTransform(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<FisherTransformResult> GetFisherTransform(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcFisherTransform(lookbackPeriods);
}
