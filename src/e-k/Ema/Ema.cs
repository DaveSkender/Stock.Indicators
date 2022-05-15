namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // EXPONENTIAL MOVING AVERAGE
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<EmaResult> GetEma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        CandlePart candlePart = CandlePart.Close)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicData> bdList = quotes.ToBasicClass(candlePart);

        // calculate
        return bdList.CalcEma(lookbackPeriods);
    }

    // standard calculation
    private static List<EmaResult> CalcEma(
        this List<BasicData> bdList, int lookbackPeriods)
    {
        // check parameter arguments
        ValidateEma(lookbackPeriods);

        // initialize
        int length = bdList.Count;
        List<EmaResult> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double lastEma = 0;
        int initPeriods = Math.Min(lookbackPeriods, length);

        for (int i = 0; i < initPeriods; i++)
        {
            lastEma += bdList[i].Value;
        }

        lastEma /= lookbackPeriods;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            BasicData h = bdList[i];

            EmaResult result = new()
            {
                Date = h.Date
            };

            if (i + 1 > lookbackPeriods)
            {
                double ema = lastEma + (k * (h.Value - lastEma));
                result.Ema = (decimal?)ema;
                lastEma = ema;
            }
            else if (i + 1 == lookbackPeriods)
            {
                result.Ema = (decimal?)lastEma;
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateEma(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for EMA.");
        }
    }
}
