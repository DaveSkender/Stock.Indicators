namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // MARUBOZU
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<MarubozuResult> GetMarubozu<TQuote>(
        this IEnumerable<TQuote> quotes,
        double minBodyPercent = 0.95)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidateMarubozu(quotes, minBodyPercent);

        // convert quotes
        List<Candle> candles = quotes.ConvertToCandles();

        // initialize
        int size = candles.Count;
        List<MarubozuResult> results = new(size);

        // roll through candles
        for (int i = 0; i < size; i++)
        {
            Candle c = candles[i];

            MarubozuResult result = new()
            {
                Date = c.Date,
                IsBullish = c.IsBullish
            };
            results.Add(result);

            if (c.BodyPct >= (decimal)minBodyPercent)
            {
                result.Marubozu = c.Close;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateMarubozu<TQuote>(
        IEnumerable<TQuote> quotes,
        double minBodyPercent)
        where TQuote : IQuote
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

        // check quotes
        int qtyHistory = quotes.Count();
        int minHistory = 1;
        if (qtyHistory < minHistory)
        {
            string message = "Insufficient quotes provided for Marubozu.  " +
                string.Format(
                    EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

            throw new BadQuotesException(nameof(quotes), message);
        }
    }
}
