namespace Skender.Stock.Indicators;

// SEEK & FIND in SERIES

public static class Seeking
{
    // FIND SERIES by DATE
    /// <summary> Finds time series values on a specific date.
    /// <para>
    /// See <see href="https://dotnet.StockIndicators.dev/utilities/#find-indicator-result-by-date?utm_source=library&amp;utm_medium=inline-help&amp;utm_campaign=embedded">documentation</see> for more information.
    /// </para>
    /// </summary>
    /// <typeparam name="TSeries">Any series type.</typeparam>
    /// <param name="series">Time series to evaluate.</param>
    /// <param name="lookupDate">Exact date to lookup.</param>
    /// <returns>First record in the series on the date specified.</returns>
    /// <exception cref="InvalidOperationException">
    /// Sequence contains no matching element
    /// </exception>
    public static TSeries Find<TSeries>(
        this IEnumerable<TSeries> series,
        DateTime lookupDate)
        where TSeries : ISeries
            => series.First(x => x.Timestamp == lookupDate);

    // TODO: add TryFind(), like TryParse() since struct won't allow null return types.
    // May just use this (above) with a try/catch and `bool` primary return type.

    // FIND INDEX by DATE
    public static int FindIndex<TSeries>(
        this List<TSeries> series,
        DateTime lookupDate)
        where TSeries : ISeries
            => series?.FindIndex(x => x.Timestamp == lookupDate) ?? -1;
}
