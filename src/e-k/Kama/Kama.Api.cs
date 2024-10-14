namespace Skender.Stock.Indicators;

// KAUFMAN's ADAPTIVE MOVING AVERAGE (API)

public static partial class Kama
{
    // SERIES, from CHAIN
    public static IReadOnlyList<KamaResult> ToKama<T>(
        this IEnumerable<T> source,
        int erPeriods = 10,
        int fastPeriods = 2,
        int slowPeriods = 30)
        where T : IReusable
        => source
            .ToSortedList()
            .CalcKama(erPeriods, fastPeriods, slowPeriods);
}
