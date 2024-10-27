namespace Skender.Stock.Indicators;

// SORTED of SERIES

public static partial class Utility
{
    public static IReadOnlyList<TSeries> ToSortedList<TSeries>(
        this IEnumerable<TSeries> series)
        where TSeries : ISeries
        => series
            .OrderBy(x => x.Timestamp)
            .ToList();
}
