namespace Skender.Stock.Indicators;

// AWESOME OSCILLATOR (API)
public static partial class Awesome
{
    // SERIES, from CHAIN
    public static IReadOnlyList<AwesomeResult> ToAwesome<T>(
        this IEnumerable<T> source,
        int fastPeriods = 5,
        int slowPeriods = 34)
        where T : IReusable
        => source
            .ToSortedList(CandlePart.HL2)
            .CalcAwesome(fastPeriods, slowPeriods);
}
