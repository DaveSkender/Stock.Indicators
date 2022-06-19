namespace Skender.Stock.Indicators;

// DOJI (SERIES)
public static partial class Indicator
{
    /// <include file='./info.xml' path='info/*' />
    ///
    internal static List<CandleResult> CalcDoji<TQuote>(
        this IEnumerable<TQuote> quotes,
        double maxPriceChangePercent)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidateDoji(maxPriceChangePercent);

        // initialize
        List<CandleResult> results = quotes.ToCandleResults();
        maxPriceChangePercent /= 100;
        int length = results.Count;

        // roll through candles
        for (int i = 0; i < length; i++)
        {
            CandleResult r = results[i];

            // check for current signal
            if (r.Candle.Open != 0)
            {
                if (Math.Abs((double)(r.Candle.Close / r.Candle.Open) - 1d) <= maxPriceChangePercent)
                {
                    r.Price = r.Candle.Close;
                    r.Match = Match.Neutral;
                }
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateDoji(
        double maxPriceChangePercent)
    {
        // check parameter arguments
        if (maxPriceChangePercent is < 0 or > 0.5)
        {
            throw new ArgumentOutOfRangeException(nameof(maxPriceChangePercent), maxPriceChangePercent,
                "Maximum Percent Change must be between 0 and 0.5 for Doji (0% to 0.5%).");
        }
    }
}
