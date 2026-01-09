namespace Skender.Stock.Indicators;

/// <summary>
/// Provides extension methods for sorting a series of elements.
/// </summary>
public static class Sorting
{
    /// <summary>
    /// Sorts the series by their timestamps in ascending order.
    /// </summary>
    /// <typeparam name="T">Type of record</typeparam>
    /// <param name="series">The series of elements to sort.</param>
    /// <returns>A read-only list of the sorted elements.</returns>
    public static IReadOnlyList<T> ToSortedList<T>(
        this IEnumerable<T> series)
        where T : ISeries
        => series
            .OrderBy(static x => x.Timestamp)
            .ToList();

    /// <summary>
    /// Sorts quotes by their timestamps in ascending order and returns as IReusable list.
    /// </summary>
    /// <remarks>
    /// This method is needed for backward compatibility with obsolete methods
    /// that need to convert IQuote collections to IReusable for refactored indicators.
    /// </remarks>
    /// <typeparam name="TQuote">The type of the quote elements, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>A read-only list of IReusable elements.</returns>
    internal static IReadOnlyList<IReusable> ToSortedReusableList<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
        => quotes
            .OrderBy(static x => x.Timestamp)
            .Cast<IReusable>()
            .ToList();
}
