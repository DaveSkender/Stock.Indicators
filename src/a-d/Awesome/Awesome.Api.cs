namespace Skender.Stock.Indicators;

// AWESOME OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<AwesomeResult> GetAwesome<T>(
        this IEnumerable<T> results,
        int fastPeriods = 5,
        int slowPeriods = 34)
        where T : IReusableResult
        => results
            .ToSortedList()
            .CalcAwesome(fastPeriods, slowPeriods);
}
