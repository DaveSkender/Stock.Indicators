namespace Skender.Stock.Indicators;

// CHANDE MOMENTUM OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<CmoResult> GetCmo<T>(
        this IEnumerable<T> source,
        int lookbackPeriods)
        where T : IReusable
        => source
            .ToSortedList()
            .CalcCmo(lookbackPeriods);
}
