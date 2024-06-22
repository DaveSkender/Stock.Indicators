namespace Skender.Stock.Indicators;

// TRUE STRENGTH INDEX (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<TsiResult> GetTsi<T>(
        this IEnumerable<T> results,
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7)
        where T : IReusableResult
        => results
            .ToTupleResult()
            .CalcTsi(lookbackPeriods, smoothPeriods, signalPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<TsiResult> GetTsi(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7) => priceTuples
            .ToSortedList()
            .CalcTsi(lookbackPeriods, smoothPeriods, signalPeriods);
}
