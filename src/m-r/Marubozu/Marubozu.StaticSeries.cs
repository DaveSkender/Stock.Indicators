/// <summary>
/// Provides methods for identifying Marubozu candlestick patterns in a series of quotes.
/// </summary>
public static partial class Marubozu
{
    /// <summary>
    /// Converts a list of quotes to Marubozu candlestick pattern results.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quotes, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="quotes">The list of quotes to analyze.</param>
    /// <param name="minBodyPercent">The minimum body percentage to qualify as a Marubozu. Default is 95.</param>
    /// <returns>A list of <see cref="CandleResult"/> indicating the presence of Marubozu patterns.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the minimum body percentage is out of range.</exception>
    public static IReadOnlyList<CandleResult> ToMarubozu<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        double minBodyPercent = 95)
        where TQuote : IQuote
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(quotes);
        Validate(minBodyPercent);

        // initialize
        int length = quotes.Count;
        List<CandleResult> results = new(length);

        minBodyPercent /= 100;

        // roll through candles
        for (int i = 0; i < length; i++)
        {
            TQuote q = quotes[i];
            decimal? matchPrice = null;
            Match matchType = Match.None;
            CandleProperties candle = q.ToCandle();

            // check for current signal
            if (candle.BodyPct >= minBodyPercent)
            {
                matchPrice = q.Close;
                matchType = candle.IsBullish ? Match.BullSignal : Match.BearSignal;
            }

            results.Add(new(
                timestamp: q.Timestamp,
                candle: candle,
                match: matchType,
                price: matchPrice));
        }

        return results;
    }
}
