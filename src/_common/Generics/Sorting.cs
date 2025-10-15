namespace Skender.Stock.Indicators;

/// <summary>
/// Provides extension methods for sorting a series of elements.
/// </summary>
public static class Sorting
{
    /// <summary>
    /// Sorts the series by their timestamps in ascending order.
    /// </summary>
    /// <param name="series">The series of elements to sort.</param>
    /// <returns>A read-only list of the sorted elements.</returns>
    public static IReadOnlyList<T> ToSortedList<T>(
        this IEnumerable<T> series)
        where T : ISeries
        => series
            .OrderBy(x => x.Timestamp)
            .ToList();
}
