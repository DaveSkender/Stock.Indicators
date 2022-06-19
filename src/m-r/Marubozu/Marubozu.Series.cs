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
        ValidateMarubozu(minBodyPercent);

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

    // parameter validation
    private static void ValidateMarubozu(
        double minBodyPercent)
    {
        // check parameter arguments
        if (minBodyPercent > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(minBodyPercent), minBodyPercent,
                "Minimum Body Percent must be less than 100 for Marubozu (<=100%).");
        }

        if (minBodyPercent < 80)
        {
            throw new ArgumentOutOfRangeException(nameof(minBodyPercent), minBodyPercent,
                "Minimum Body Percent must at least 80 (80%) for Marubozu and is usually greater than 90 (90%).");
        }
    }
}
