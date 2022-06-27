namespace Skender.Stock.Indicators;

// RELATIVE STRENGTH INDEX (SERIES)
public static partial class Indicator
{
    internal static List<RsiResult> CalcRsi(
        this List<(DateTime Date, double Value)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateRsi(lookbackPeriods);

        // initialize
        int length = tpList.Count;
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
            lastValue = tpList[0].Value;
        }

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            RsiResult r = new(date);
            results.Add(r);

            gain[i] = (value > lastValue) ? value - lastValue : 0;
            loss[i] = (value < lastValue) ? lastValue - value : 0;
            lastValue = value;

            // calculate RSI
            if (i > lookbackPeriods)
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
            else if (i == lookbackPeriods)
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
