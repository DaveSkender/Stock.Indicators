using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// SORTED of SERIES

public static class Sorting
{
    public static Collection<TSeries> ToSortedCollection<TSeries>(
        this IEnumerable<TSeries> series)
        where TSeries : ISeries
        => series
            .OrderBy(x => x.Date)
            .ToCollection();

    internal static List<TSeries> ToSortedList<TSeries>(
        this IEnumerable<TSeries> series)
        where TSeries : ISeries
        => series
            .OrderBy(x => x.Date)
            .ToList();
}
