namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // TRIPLE EXPONENTIAL MOVING AVERAGE (TEMA)
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<TemaResult> GetTema<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicData> bdList = quotes.ToBasicClass(CandlePart.Close);

        // check parameter arguments
        ValidateTema(lookbackPeriods);

        // initialize
        int length = bdList.Count;
        List<TemaResult> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double? lastEma1 = 0;
        double? lastEma2;
        double? lastEma3;
        int initPeriods = Math.Min(lookbackPeriods, length);

        for (int i = 0; i < initPeriods; i++)
        {
            lastEma1 += bdList[i].Value;
        }

        lastEma1 /= lookbackPeriods;
        lastEma2 = lastEma3 = lastEma1;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            BasicData q = bdList[i];

            TemaResult result = new()
            {
                Date = q.Date
            };

            if (i > lookbackPeriods - 1)
            {
                double? ema1 = lastEma1 + (k * (q.Value - lastEma1));
                double? ema2 = lastEma2 + (k * (ema1 - lastEma2));
                double? ema3 = lastEma3 + (k * (ema2 - lastEma3));

                result.Tema = (decimal?)((3 * ema1) - (3 * ema2) + ema3);

                lastEma1 = ema1;
                lastEma2 = ema2;
                lastEma3 = ema3;
            }
            else if (i == lookbackPeriods - 1)
            {
                result.Tema = (decimal?)((3 * lastEma1) - (3 * lastEma2) + lastEma3);
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateTema(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for TEMA.");
        }
    }
}
