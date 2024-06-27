using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// SORTED of SERIES

public static class Sorting
{
    public static Collection<TSeries> ToSortedCollection<TSeries>(
        this IEnumerable<TSeries> series)
        where TSeries : ISeries
        => series
            .OrderBy(x => x.Timestamp)
            .ToCollection();

    // TODO: need it?  is it a useful public utility?
    [Obsolete("new, never implemented")]
    public static IReadOnlyList<TSeries> ToSortedReadOnlyList<TSeries>(
    this IEnumerable<TSeries> series)
    where TSeries : ISeries
        => series
            .OrderBy(x => x.Timestamp)
            .ToList();

    internal static List<TSeries> ToSortedList<TSeries>(
        this IEnumerable<TSeries> series)
        where TSeries : ISeries
        => series
            .OrderBy(x => x.Timestamp)
            .ToList();
}
