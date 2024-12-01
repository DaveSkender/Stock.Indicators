namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for reusable types.
/// </summary>
public static partial class Reusable
{
    /// <summary>
    /// Converts a list of quotes to a list of reusable types.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The list of quotes.</param>
    /// <param name="candlePart">The part of the candle to use.</param>
    /// <returns>A list of reusable types.</returns>
    public static IReadOnlyList<IReusable> ToReusable<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote

        => quotes
            .OrderBy(x => x.Timestamp)
            .Select(x => x.ToReusable(candlePart))
            .ToList();

    /// <summary>
    /// Removes non-essential records containing null or NaN values.
    /// </summary>
    /// <typeparam name="T">Any reusable result type.</typeparam>
    /// <param name="results">Indicator results to evaluate.</param>
    /// <returns>Time series of indicator results, condensed.</returns>
    public static IReadOnlyList<T> Condense<T>(
        this IReadOnlyList<T> results)
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
    /// </summary>
    /// <typeparam name="T">Any reusable result type.</typeparam>
    /// <param name="results">Indicator results to evaluate.</param>
    /// <returns>Time series of results, pruned.</returns>
    internal static IReadOnlyList<T> RemoveWarmupPeriods<T>(
        this IReadOnlyList<T> results)
        where T : IReusable
    {
        // this is the default implementation, it will
        // be overridden in the specific indicator class

        int removePeriods = results
            .ToList()
            .FindIndex(x => !double.IsNaN(x.Value));

        return results.Remove(removePeriods);

        // TODO: remove specific indicator 'RemoveWarmupPeriods()' methods
        // that are now redundant to this generic method (not all are).
        // Note: Some or all of these may already be removed.
    }

    /// <summary>
    /// Converts a quote to a basic chainable class.
    /// </summary>
    /// <param name="q">The quote to convert.</param>
    /// <param name="candlePart">The part of the candle to use.</param>
    /// <returns>A reusable type.</returns>
    internal static IReusable ToReusable(this IQuote q, CandlePart candlePart)
        => q.ToQuotePart(candlePart);
}
