namespace Skender.Stock.Indicators;

// MARUBOZU (SERIES)

public static partial class Indicator
{
    internal static List<CandleResult> CalcMarubozu<TQuote>(
        this List<TQuote> quotesList,
        double minBodyPercent)
        where TQuote : IQuote
    {
        // check parameter arguments
        Marubozu.Validate(minBodyPercent);

        // initialize
        int length = quotesList.Count;
        List<CandleResult> results = new(length);

        minBodyPercent /= 100;

        // roll through candles
        for (int i = 0; i < length; i++)
        {
            TQuote q = quotesList[i];
            decimal? matchPrice = null;
            Match matchType = Match.None;
            CandleProperties candle = q.ToCandle();

            // check for current signal
            if (candle.BodyPct >= minBodyPercent)
            {
                matchPrice = q.Close;
                matchType = candle.IsBullish ? Match.BullSignal : Match.BearSignal;
            }

            results.Add(new CandleResult(
                timestamp: q.Timestamp,
                candle: candle,
                match: matchType,
                price: matchPrice));
        }

        return results;
    }
}
