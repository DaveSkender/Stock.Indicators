namespace Skender.Stock.Indicators;

// TRIPLE EMA OSCILLATOR - TRIX (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<TrixResult> GetTrix<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusableResult
        => results
            .ToSortedList()
            .CalcTrix(lookbackPeriods);
}
