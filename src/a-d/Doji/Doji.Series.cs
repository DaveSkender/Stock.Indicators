namespace Skender.Stock.Indicators;

// DOJI (SERIES)

public static partial class Indicator
{
    private static List<CandleResult> CalcDoji<TQuote>(
        this List<TQuote> quotesList,
        double maxPriceChangePercent)
        where TQuote : IQuote
    {
        // check parameter arguments
        Doji.Validate(maxPriceChangePercent);

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
