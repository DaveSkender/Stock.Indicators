namespace Skender.Stock.Indicators;

// TRIPLE EMA OSCILLATOR - TRIX (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<TrixResult> ToTrix<T>(
        this IReadOnlyList<T> results,
        int lookbackPeriods)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcTrix(lookbackPeriods);
}
