namespace Skender.Stock.Indicators;

// RELATIVE STRENGTH INDEX (SERIES)

public static partial class Rsi
{
    public static IReadOnlyList<RsiResult> ToRsi<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods = 14)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        double avgGain = double.NaN;
        double avgLoss = double.NaN;

        List<RsiResult> results = new(length);
        double[] gain = new double[length]; // gain
        double[] loss = new double[length]; // loss

        if (length == 0)
        {
            return results;
        }

        double prevValue = source[0].Value;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            if (double.IsNaN(s.Value) || double.IsNaN(prevValue))
            {
                gain[i] = loss[i] = double.NaN;
            }
            else
            {
                gain[i] = s.Value > prevValue ? s.Value - prevValue : 0;
                loss[i] = s.Value < prevValue ? prevValue - s.Value : 0;
            }

            double? rsi = null;
            prevValue = s.Value;

            // re/initialize average gain
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

                rsi = !double.IsNaN(avgGain / avgLoss)
                      ? avgLoss > 0 ? 100 - 100 / (1 + avgGain / avgLoss) : 100
                      : null;
            }

            // calculate RSI normally
            else if (i > lookbackPeriods)
            {
                avgGain = (avgGain * (lookbackPeriods - 1) + gain[i]) / lookbackPeriods;
                avgLoss = (avgLoss * (lookbackPeriods - 1) + loss[i]) / lookbackPeriods;

                if (avgLoss > 0)
                {
                    double rs = avgGain / avgLoss;
                    rsi = 100 - 100 / (1 + rs);
                }
                else
                {
                    rsi = 100;
                }
            }

            RsiResult r = new(
                Timestamp: s.Timestamp,
                Rsi: rsi);

            results.Add(r);
        }

        return results;
    }
}
