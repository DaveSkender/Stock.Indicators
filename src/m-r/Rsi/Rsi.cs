namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // RELATIVE STRENGTH INDEX
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<RsiResult> GetRsi<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote
    {
        // convert quotes
        List<SimplePrice> bdList = quotes.ToPrice(CandlePart.Close);

        // calculate
        return CalcRsi(bdList, lookbackPeriods);
    }

    // internals
    private static List<RsiResult> CalcRsi(List<SimplePrice> bdList, int lookbackPeriods)
    {
        // check parameter arguments
        ValidateRsi(lookbackPeriods);

        // initialize
        int length = bdList.Count;
        double avgGain = 0;
        double avgLoss = 0;

        List<RsiResult> results = new(length);
        double[] gain = new double[length]; // gain
        double[] loss = new double[length]; // loss
        double lastValue;

        if (length == 0)
        {
            return results;
        }
        else
        {
            lastValue = bdList[0].Value;
        }

        // roll through quotes
        for (int i = 0; i < bdList.Count; i++)
        {
            SimplePrice h = bdList[i];
            int index = i + 1;

            RsiResult r = new()
            {
                Date = h.Date
            };
            results.Add(r);

            gain[i] = (h.Value > lastValue) ? h.Value - lastValue : 0;
            loss[i] = (h.Value < lastValue) ? lastValue - h.Value : 0;
            lastValue = h.Value;

            // calculate RSI
            if (index > lookbackPeriods + 1)
            {
                avgGain = ((avgGain * (lookbackPeriods - 1)) + gain[i]) / lookbackPeriods;
                avgLoss = ((avgLoss * (lookbackPeriods - 1)) + loss[i]) / lookbackPeriods;

                if (avgLoss > 0)
                {
                    double rs = avgGain / avgLoss;
                    r.Rsi = 100 - (100 / (1 + rs));
                }
                else
                {
                    r.Rsi = 100;
                }
            }

            // initialize average gain
            else if (index == lookbackPeriods + 1)
            {
                double sumGain = 0;
                double sumLoss = 0;

                for (int p = 1; p <= lookbackPeriods; p++)
                {
                    sumGain += gain[p];
                    sumLoss += loss[p];
                }

                avgGain = sumGain / lookbackPeriods;
                avgLoss = sumLoss / lookbackPeriods;

                r.Rsi = (avgLoss > 0) ? 100 - (100 / (1 + (avgGain / avgLoss))) : 100;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateRsi(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for RSI.");
        }
    }
}
