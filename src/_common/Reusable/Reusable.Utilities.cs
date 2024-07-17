namespace Skender.Stock.Indicators;

// REUSABLE TYPE UTILITIES

public static class ReusableUtility
{
    // convert IQuote type list to IReusable list
    public static IReadOnlyList<IReusable> ToReusableList<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote

        => quotes
            .OrderBy(x => x.Timestamp)
            .Select(x => x.ToReusable(candlePart))
            .ToList();

    /// <summary>
    /// Removes non-essential records containing null or NaN values.
    /// <para> See<see
    /// href="https://dotnet.StockIndicators.dev/utilities/#condense?utm_source=library&amp;utm_medium=inline-help&amp;utm_campaign=embedded">
    /// documentation</see> for more information.
    /// </para>
    /// </summary>
    /// <typeparam name="T">Any reusable result type.</typeparam>
    /// <param name="results">Indicator results to evaluate.</param>
    /// <returns>Time series of indicator results, condensed.</returns>
    public static IReadOnlyList<T> Condense<T>(
        this IEnumerable<T> results)
        where T : IReusable
    {
        List<T> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => double.IsNaN(x.Value));

        return resultsList;
    }

    /// <summary>
    /// Removes the recommended quantity of results from the beginning
    /// of the results list using a reverse-engineering approach.
    /// <para> See
    /// <see href="https://dotnet.StockIndicators.dev/utilities/#remove-warmup-periods?utm_source=library&amp;utm_medium=inline-help&amp;utm_campaign=embedded">
    /// documentation</see> for more information.
    /// </para>
    /// </summary>
    /// <typeparam name="T">Any reusable result type.</typeparam>
    /// <param name="results">Indicator results to evaluate.</param>
    /// <returns>Time series of results, pruned.</returns>
    internal static IReadOnlyList<T> RemoveWarmupPeriods<T>(
        this IEnumerable<T> results)
        where T : IReusable
    {
        // this is the default implementation, it will
        // be overridden in the specific indicator class

        int removePeriods = results
            .ToList()
            .FindIndex(x => !double.IsNaN(x.Value));

        return results.Remove(removePeriods);
    }

    // convert TQuote element to a basic chainable class
    internal static IReusable ToReusable(this IQuote q, CandlePart candlePart)
        => q.ToQuotePart(candlePart);
}
