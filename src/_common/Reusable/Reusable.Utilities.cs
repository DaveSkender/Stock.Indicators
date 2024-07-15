using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Skender.Stock.Indicators;

// REUSABLE TYPE UTILITIES

public static class ReusableUtility
{
    // convert IQuote type list to Reusable list
    public static IReadOnlyList<Reusable> ToReusableList<TQuote>(
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
    public static IEnumerable<T> Condense<T>(
        this IEnumerable<T> results)
        where T : IReusable
    {
        List<T> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => double.IsNaN(x.Value));

        return resultsList.ToSortedList();
    }

    ///<summary>
    ///Removes the recommended quantity of results from the beginning of the results list using a reverse-engineering approach. <para> See<see href="https://dotnet.StockIndicators.dev/utilities/#remove-warmup-periods?utm_source=library&amp;utm_medium=inline-help&amp;utm_campaign=embedded"> documentation</see> for more information. </para>
    ///</summary>
    ///<param name="results">Indicator results to evaluate.</param>
    ///<returns>Time series of results, pruned.</returns>
    internal static IEnumerable<T> RemoveWarmupPeriods<T>(
        this IEnumerable<T> results)
        where T : IReusable
    {
        // this is the default implementation;
        // it will be overridden in the specific indicator class

        int removePeriods = results
            .ToList()
            .FindIndex(x => !double.IsNaN(x.Value));

        return results.Remove(removePeriods);
    }

    // convert QuoteD type list to Reusable list
    internal static List<Reusable> ToReusableList(
        this List<QuoteD> qdList,
        CandlePart candlePart)

          => qdList
            .OrderBy(x => x.Timestamp)
            .Select(x => x.ToReusable(candlePart))
            .ToList();

    // convert TQuote element to a basic chainable class
    internal static Reusable ToReusable(this IQuote q, CandlePart candlePart)
        => new(q.Timestamp, q.ToQuotePartValue(candlePart));

    // convert quoteD element to reusable type
    internal static Reusable ToReusable(this QuoteD q, CandlePart candlePart)

        => candlePart switch {

            CandlePart.Open => new(q.Timestamp, q.Open),
            CandlePart.High => new(q.Timestamp, q.High),
            CandlePart.Low => new(q.Timestamp, q.Low),
            CandlePart.Close => new(q.Timestamp, q.Close),
            CandlePart.Volume => new(q.Timestamp, q.Volume),
            CandlePart.HL2 => new(q.Timestamp, (q.High + q.Low) / 2),
            CandlePart.HLC3 => new(q.Timestamp, (q.High + q.Low + q.Close) / 3),
            CandlePart.OC2 => new(q.Timestamp, (q.Open + q.Close) / 2),
            CandlePart.OHL3 => new(q.Timestamp, (q.Open + q.High + q.Low) / 3),
            CandlePart.OHLC4 => new(q.Timestamp, (q.Open + q.High + q.Low + q.Close) / 4),

            _ => throw new ArgumentOutOfRangeException(
                nameof(candlePart), candlePart, "Invalid candlePart provided.")
        };
}
