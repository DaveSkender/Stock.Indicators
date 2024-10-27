namespace Skender.Stock.Indicators;

// MARUBOZU (SERIES)

public static partial class Marubozu
{
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
