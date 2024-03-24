namespace Skender.Stock.Indicators;

// MARUBOZU (SERIES)

public static partial class Indicator
{
    /// <include file='./info.xml' path='info/*' />
    ///
    internal static List<CandleResult> CalcMarubozu<TQuote>(
        this IEnumerable<TQuote> quotes,
        double minBodyPercent)
        where TQuote : IQuote
    {
        // check parameter arguments
        Marubozu.Validate(minBodyPercent);

        // initialize
        List<CandleResult> results = quotes.ToCandleResults();
        minBodyPercent /= 100;
        int length = results.Count;

        // roll through candles
        for (int i = 0; i < length; i++)
        {
            CandleResult r = results[i];

            // check for current signal
            if (r.Candle.BodyPct >= minBodyPercent)
            {
                r.Price = r.Candle.Close;
                r.Match = r.Candle.IsBullish ? Match.BullSignal : Match.BearSignal;
            }
        }

        return results;
    }
}
