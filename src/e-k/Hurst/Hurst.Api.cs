namespace Skender.Stock.Indicators;

// HURST EXPONENT (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<HurstResult> GetHurst<T>(
        this IEnumerable<T> results,
        int lookbackPeriods = 100)
        where T: IReusableResult
        => results
            .ToTupleResult()
            .CalcHurst(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<HurstResult> GetHurst(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcHurst(lookbackPeriods);
}
