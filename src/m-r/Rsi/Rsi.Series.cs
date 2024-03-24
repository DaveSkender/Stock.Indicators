namespace Skender.Stock.Indicators;

// RELATIVE STRENGTH INDEX (SERIES)

public static partial class Indicator
{
    internal static List<RsiResult> CalcRsi(
        this List<(DateTime Timestamp, double Value)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Rsi.Validate(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        double avgGain = double.NaN;
        double avgLoss = double.NaN;

        List<RsiResult> results = new(length);
        double[] gain = new double[length]; // gain
        double[] loss = new double[length]; // loss
        double prevValue;

        if (length == 0)
        {
            return results;
        }

        prevValue = tpList[0].Value;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            RsiResult r = new() { Timestamp = date };
            results.Add(r);

            if (double.IsNaN(value) || double.IsNaN(prevValue))
            {
                gain[i] = loss[i] = double.NaN;
            }
            else
            {
                gain[i] = (value > prevValue) ? value - prevValue : 0;
                loss[i] = (value < prevValue) ? prevValue - value : 0;
            }

            prevValue = value;

            // initialize average gain
            if (i >= lookbackPeriods && (double.IsNaN(avgGain) || double.IsNaN(avgLoss)))
            {
                double sumGain = 0;
                double sumLoss = 0;

                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    sumGain += gain[p];
                    sumLoss += loss[p];
                }

                avgGain = sumGain / lookbackPeriods;
                avgLoss = sumLoss / lookbackPeriods;

                r.Rsi = !double.IsNaN(avgGain / avgLoss)
                      ? (avgLoss > 0) ? 100 - (100 / (1 + (avgGain / avgLoss))) : 100
                      : null;
            }

            // calculate RSI normally
            else if (i > lookbackPeriods)
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
        }

        return results;
    }
}
