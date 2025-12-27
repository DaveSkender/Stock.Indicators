namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for reusable types.
/// </summary>
public static class Reusable
{
    /// <summary>
    /// Converts a list of quotes to a list of reusable types.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="candlePart">The <see cref="CandlePart" /> element.</param>
    /// <returns>A list of reusable types.</returns>
    public static IReadOnlyList<IReusable> ToReusable(
        this IReadOnlyList<IQuote> quotes,
        CandlePart candlePart)

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
                static x => double.IsNaN(x.Value));

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
            .FindIndex(static x => !double.IsNaN(x.Value));

        return results.Remove(removePeriods);

        // TODO: remove specific indicator 'RemoveWarmupPeriods()' methods
        // that are now redundant to this generic method (not all are).
        // Note: Some or all of these may already be removed.
    }

    /// <summary>
    /// Converts a quote to a basic chainable class.
    /// </summary>
    /// <param name="q">The quote to convert.</param>
    /// <param name="candlePart">The <see cref="CandlePart" /> element.</param>
    /// <returns>A reusable type.</returns>
    internal static IReusable ToReusable(this IQuote q, CandlePart candlePart)
        => q.ToQuotePart(candlePart);

    /// <summary>
    /// Creates a new array containing the double values from each element in the specified list.
    /// </summary>
    /// <param name="source">
    /// The list of <see cref="IReusable"/> objects to extract values from. Cannot be null.
    /// </param>
    /// <returns>
    /// An array of double values corresponding to the <c>Value</c> property of each element in <paramref
    /// name="source"/>. The array will be empty if <paramref name="source"/> contains no elements.
    /// </returns>
    internal static double[] ToValuesArray(
        this IReadOnlyList<IReusable> source)
    {
        int length = source.Count;
        double[] values = new double[length];

        for (int i = 0; i < length; i++)
        {
            values[i] = source[i].Value;
        }

        return values;
    }
}
