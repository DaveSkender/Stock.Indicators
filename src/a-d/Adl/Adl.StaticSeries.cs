namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the
/// Accumulation/Distribution Line (ADL) from a series of quotes.
/// </summary>
public static partial class Adl
{
    /// <summary>
    /// Converts a list of quotes to an Accumulation/Distribution Line (ADL) series.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="source">The source list of quotes.</param>
    /// <returns>A read-only list of ADL results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source is null.</exception>
    public static IReadOnlyList<AdlResult> ToAdl<TQuote>(this IReadOnlyList<TQuote> source)
        where TQuote : IQuote
    {
        ArgumentNullException.ThrowIfNull(source);

        // initialize
        int length = source.Count;
        List<AdlResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            AdlResult r = Increment(
                source[i].Timestamp,
                source[i].High,
                source[i].Low,
                source[i].Close,
                source[i].Volume,
                i > 0 ? results[i - 1].Adl : 0);

            results.Add(r);
        }

        return results;
    }
}
