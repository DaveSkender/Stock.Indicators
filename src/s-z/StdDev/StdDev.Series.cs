namespace Skender.Stock.Indicators;

// STANDARD DEVIATION (SERIES)
public static partial class Indicator
{
    internal static List<StdDevResult> CalcStdDev(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        int? smaPeriods)
    {
        // check parameter arguments
        ValidateStdDev(lookbackPeriods, smaPeriods);

        // initialize
        int length = tpList.Count;
        List<StdDevResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            StdDevResult result = new()
            {
                Date = date,
            };

            if (i + 1 >= lookbackPeriods)
            {
                double[] periodValues = new double[lookbackPeriods];
                double sum = 0;
                int n = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    (DateTime _, double dValue) = tpList[p];
                    periodValues[n] = dValue;
                    sum += dValue;
                    n++;
                }

                double periodAvg = sum / lookbackPeriods;

                result.StdDev = periodValues.StdDev();
                result.Mean = periodAvg;

                result.ZScore = (result.StdDev == 0) ? double.NaN
                    : (value - periodAvg) / result.StdDev;
            }

            results.Add(result);

            // optional SMA
            if (smaPeriods != null && i >= lookbackPeriods + smaPeriods - 2)
            {
                double? sumSma = 0;
                for (int p = i + 1 - (int)smaPeriods; p <= i; p++)
                {
                    sumSma += results[p].StdDev;
                }

                result.StdDevSma = sumSma / smaPeriods;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateStdDev(
        int lookbackPeriods,
        int? smaPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Standard Deviation.");
        }

        if (smaPeriods is not null and <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "SMA periods must be greater than 0 for Standard Deviation.");
        }
    }
}
