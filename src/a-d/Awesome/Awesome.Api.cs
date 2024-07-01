namespace Skender.Stock.Indicators;

// AWESOME OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<AwesomeResult> GetAwesome<T>(
        this IEnumerable<T> source,
        int fastPeriods = 5,
        int slowPeriods = 34)
        where T : IReusable
        => source
            .ToSortedList()
            .CalcAwesome(fastPeriods, slowPeriods);
}
