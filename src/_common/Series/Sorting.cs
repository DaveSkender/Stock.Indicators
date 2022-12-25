using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// SORTED of SERIES
public static class Sorting
{
    public static Collection<TSeries> ToSortedCollection<TSeries>(
        this IEnumerable<TSeries> results)
        where TSeries : ISeries
        => results
            .ToSortedList()
            .ToCollection();

    internal static List<TSeries> ToSortedList<TSeries>(
        this IEnumerable<TSeries> results)
        where TSeries : ISeries
        => results
            .OrderBy(x => x.Date)
            .ToList();
}
