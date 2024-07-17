namespace Skender.Stock.Indicators;

// CONNORS RSI (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<ConnorsRsiResult> GetConnorsRsi<T>(
        this IEnumerable<T> results,
        int rsiPeriods = 3,
        int streakPeriods = 2,
        int rankPeriods = 100)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);
}
