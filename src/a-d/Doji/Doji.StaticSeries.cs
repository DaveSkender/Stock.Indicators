namespace Skender.Stock.Indicators;

// DOJI (SERIES)

public static partial class Doji
{
    /// <summary>
    /// Doji is a single candlestick pattern where open and close price
    /// are virtually identical, representing market indecision.
    /// </summary>
    /// <typeparam name = "TQuote" > Configurable Quote type.
    /// See Guide for more information.</typeparam>
    /// <param name = "quotes" > Historical price quotes.</param>
    /// <param name = "maxPriceChangePercent" >
    /// Optional.Maximum absolute percent difference in open and close price.
    /// </param>
    /// <returns>Time series of Doji values.</returns>
    /// <exception cref = "ArgumentOutOfRangeException" >
    /// Invalid parameter value provided.
    /// </exception>
    public static IReadOnlyList<CandleResult> ToDoji<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        double maxPriceChangePercent = 0.1)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcDoji(maxPriceChangePercent);

    private static List<CandleResult> CalcDoji<TQuote>(
        this List<TQuote> quotesList,
        double maxPriceChangePercent)
        where TQuote : IQuote
    {
        // check parameter arguments
        Validate(maxPriceChangePercent);

        // initialize
        int length = quotesList.Count;
        List<CandleResult> results = new(length);

        maxPriceChangePercent /= 100;

        // roll through candles
        for (int i = 0; i < length; i++)
        {
            TQuote q = quotesList[i];
            decimal? matchPrice = null;
            Match matchType = Match.None;

            // check for current signal
            if (q.Open != 0
                && Math.Abs((double)(q.Close / q.Open) - 1d) <= maxPriceChangePercent)
            {
                matchPrice = q.Close;
                matchType = Match.Neutral;
            }

            results.Add(new CandleResult(
                timestamp: q.Timestamp,
                quote: q,
                match: matchType,
                price: matchPrice));
        }

        return results;
    }
}
