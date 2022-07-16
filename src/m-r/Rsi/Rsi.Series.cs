namespace Skender.Stock.Indicators;

// RELATIVE STRENGTH INDEX (SERIES)
public static partial class Indicator
{
    internal static IEnumerable<RsiResult> CalcRsi(
        this IEnumerable<(DateTime Date, double Value)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateRsi(lookbackPeriods);

        // initialize
        double avgGain = 0;
        double avgLoss = 0;

        double lastValue = 0;
        bool isFirst = true;
        double alpha = 1.0d / lookbackPeriods;

        // roll through quotes
        int i = 0;

        foreach ((DateTime date, double value) in tpList)
        {
            RsiResult r = new(date);

            if (isFirst)
            {
                lastValue = value;
                isFirst = false;
                yield return r;
                continue;
            }

            double gain = (value > lastValue) ? (value - lastValue) : 0;
            double loss = (value < lastValue) ? (lastValue - value) : 0;
            
            i++;
            if (i <= lookbackPeriods)
            {
                avgGain += gain / lookbackPeriods;
                avgLoss += loss / lookbackPeriods;
            }
            else if (i > lookbackPeriods)
            {
                avgGain = (alpha * gain) + ((1 - alpha) * avgGain);
                avgLoss = (alpha * loss) + ((1 - alpha) * avgLoss);
            }

            if (i >= lookbackPeriods)
            {
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

            lastValue = value;

            yield return r;
        }
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