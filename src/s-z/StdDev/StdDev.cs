namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // STANDARD DEVIATION
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<StdDevResult> GetStdDev<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int? smaPeriods = null)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicData> bdList = quotes.ToBasicClass(CandlePart.Close);

        // calculate
        return CalcStdDev(bdList, lookbackPeriods, smaPeriods);
    }

    // internals
    private static List<StdDevResult> CalcStdDev(
        List<BasicData> bdList, int lookbackPeriods, int? smaPeriods = null)
    {
        // check parameter arguments
        ValidateStdDev(lookbackPeriods, smaPeriods);

        // initialize
        List<StdDevResult> results = new(bdList.Count);

        // roll through quotes
        for (int i = 0; i < bdList.Count; i++)
        {
            BasicData bd = bdList[i];

            StdDevResult result = new()
            {
                Date = bd.Date,
            };

            if (i + 1 >= lookbackPeriods)
            {
                double[] periodValues = new double[lookbackPeriods];
                double sum = 0;
                int n = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    BasicData d = bdList[p];
                    periodValues[n] = d.Value;
                    sum += d.Value;
                    n++;
                }

                double periodAvg = sum / lookbackPeriods;

                result.StdDev = Functions.StdDev(periodValues);
                result.Mean = periodAvg;

                result.ZScore = (result.StdDev == 0) ? null
                    : (bd.Value - periodAvg) / result.StdDev;
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
