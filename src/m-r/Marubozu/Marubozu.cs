namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // MARUBOZU
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<CandleResult> GetMarubozu<TQuote>(
        this IEnumerable<TQuote> quotes,
        double minBodyPercent = 0.95)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidateMarubozu(minBodyPercent);

        // initialize
        List<CandleResult> results = quotes.ToCandleResults();
        int length = results.Count;

        // roll through candles
        for (int i = 0; i < length; i++)
        {
            CandleResult r = results[i];

            // check for current signal
            if (r.Candle.BodyPct >= minBodyPercent)
            {
                r.Price = r.Candle.Close;
                r.Signal = r.Candle.IsBullish ? Signal.BullSignal : Signal.BearSignal;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateMarubozu(
        double minBodyPercent)
    {
        // check parameter arguments
        if (minBodyPercent > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(minBodyPercent), minBodyPercent,
                "Minimum Body Percent must be less than 1 for Marubozu (<=100%).");
        }

        if (minBodyPercent < 0.8)
        {
            throw new ArgumentOutOfRangeException(nameof(minBodyPercent), minBodyPercent,
                "Minimum Body Percent must at least 0.8 (80%) for Marubozu and is usually greater than 0.9 (90%).");
        }
    }
}
