namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // DOUBLE EXPONENTIAL MOVING AVERAGE (DEMA)
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<DemaResult> GetDema<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<BaseQuote> bdList = quotes.ToBaseQuote(CandlePart.Close);

        // check parameter arguments
        ValidateDema(lookbackPeriods);

        // initialize
        int length = bdList.Count;
        List<DemaResult> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double? lastEma1 = 0;
        double? lastEma2;
        int initPeriods = Math.Min(lookbackPeriods, length);

        for (int i = 0; i < initPeriods; i++)
        {
            lastEma1 += bdList[i].Value;
        }

        lastEma1 /= lookbackPeriods;
        lastEma2 = lastEma1;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            BaseQuote q = bdList[i];

            DemaResult result = new()
            {
                Date = q.Date
            };

            if (i > lookbackPeriods - 1)
            {
                double? ema1 = lastEma1 + (k * (q.Value - lastEma1));
                double? ema2 = lastEma2 + (k * (ema1 - lastEma2));

                result.Dema = (decimal?)((2d * ema1) - ema2);

                lastEma1 = ema1;
                lastEma2 = ema2;
            }
            else if (i == lookbackPeriods - 1)
            {
                result.Dema = (decimal?)((2d * lastEma1) - lastEma2);
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateDema(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for DEMA.");
        }
    }
}
