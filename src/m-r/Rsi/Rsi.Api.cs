namespace Skender.Stock.Indicators;

// RELATIVE STRENGTH INDEX (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<RsiResult> GetRsi<T>(
        this IEnumerable<T> results,
        int lookbackPeriods = 14)
        where T : IReusableResult
        => results
            .ToTupleResult()
            .CalcRsi(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<RsiResult> GetRsi(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods = 14) => priceTuples
            .ToSortedList()
            .CalcRsi(lookbackPeriods);
}
