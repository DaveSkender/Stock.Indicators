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
            .ToTupleResult()
            .CalcAwesome(fastPeriods, slowPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<AwesomeResult> GetAwesome(
        this IEnumerable<(DateTime, double)> priceTuples,
        int fastPeriods = 5,
        int slowPeriods = 34) => priceTuples
            .ToSortedList()
            .CalcAwesome(fastPeriods, slowPeriods);
}
