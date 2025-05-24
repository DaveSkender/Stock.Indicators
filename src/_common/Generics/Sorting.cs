namespace Skender.Stock.Indicators;

/// <summary>
/// Provides extension methods for sorting a series of elements.
/// </summary>
public static class Sorting
{
    /// <summary>
    /// Sorts the series by their timestamps in ascending order.
    /// </summary>
    /// <typeparam name="TSeries">The type of the elements in the series, which must implement <see cref="ISeries"/>.</typeparam>
    /// <param name="series">The series of elements to sort.</param>
    /// <returns>A read-only list of the sorted elements.</returns>
    public static IReadOnlyList<TSeries> ToSortedList<TSeries>(
        this IEnumerable<TSeries> series)
        where TSeries : ISeries
        => series
            .OrderBy(x => x.Timestamp)
            .ToList();
}
