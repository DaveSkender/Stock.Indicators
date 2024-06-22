namespace Skender.Stock.Indicators;

// CHANDE MOMENTUM OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<CmoResult> GetCmo<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusableResult
        => results
            .ToTupleResult()
            .CalcCmo(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<CmoResult> GetCmo(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcCmo(lookbackPeriods);
}
