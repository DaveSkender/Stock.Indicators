namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // RATE OF CHANGE (ROC)
    /// <include file='./info.xml' path='indicator/type[@name="Main"]/*' />
    ///
    public static IEnumerable<RocResult> GetRoc<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int? smaPeriods = null)
        where TQuote : IQuote
    {
        // convert quotes
        List<BaseQuote> bdList = quotes.ToBaseQuote(CandlePart.Close);

        // check parameter arguments
        ValidateRoc(lookbackPeriods, smaPeriods);

        // initialize
        List<RocResult> results = new(bdList.Count);

        // roll through quotes
        for (int i = 0; i < bdList.Count; i++)
        {
            BaseQuote q = bdList[i];

            RocResult result = new()
            {
                Date = q.Date
            };

            if (i + 1 > lookbackPeriods)
            {
                BaseQuote back = bdList[i - lookbackPeriods];

                result.Roc = (back.Value == 0) ? null
                    : 100d * (q.Value - back.Value) / back.Value;
            }

            results.Add(result);

            // optional SMA
            if (smaPeriods != null && i + 1 >= lookbackPeriods + smaPeriods)
            {
                double? sumSma = 0;
                for (int p = i + 1 - (int)smaPeriods; p <= i; p++)
                {
                    sumSma += results[p].Roc;
                }

                result.RocSma = sumSma / smaPeriods;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateRoc(
        int lookbackPeriods,
        int? smaPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for ROC.");
        }

        if (smaPeriods is not null and <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "SMA periods must be greater than 0 for ROC.");
        }
    }
}
