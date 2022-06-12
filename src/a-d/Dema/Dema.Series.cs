namespace Skender.Stock.Indicators;

// DOUBLE EXPONENTIAL MOVING AVERAGE - DEMA (SERIES)
public static partial class Indicator
{
    // series calculation
    internal static List<DemaResult> CalcDema(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateDema(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        List<DemaResult> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double? lastEma1 = 0;
        double? lastEma2;
        int initPeriods = Math.Min(lookbackPeriods, length);

        for (int i = 0; i < initPeriods; i++)
        {
            (DateTime date, double value) = tpList[i];
            lastEma1 += value;
        }

        lastEma1 /= lookbackPeriods;
        lastEma2 = lastEma1;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            DemaResult result = new()
            {
                Date = date
            };

            if (i > lookbackPeriods - 1)
            {
                double? ema1 = lastEma1 + (k * (value - lastEma1));
                double? ema2 = lastEma2 + (k * (ema1 - lastEma2));

                result.Dema = (2d * ema1) - ema2;

                lastEma1 = ema1;
                lastEma2 = ema2;
            }
            else if (i == lookbackPeriods - 1)
            {
                result.Dema = (2d * lastEma1) - lastEma2;
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
