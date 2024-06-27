namespace Skender.Stock.Indicators;

// KAUFMAN's ADAPTIVE MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<KamaResult> GetKama<T>(
        this IEnumerable<T> source,
        int erPeriods = 10,
        int fastPeriods = 2,
        int slowPeriods = 30)
        where T : IReusable
        => source
            .ToSortedList()
            .CalcKama(erPeriods, fastPeriods, slowPeriods);
}
