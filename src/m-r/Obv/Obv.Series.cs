using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// ON-BALANCE VOLUME (SERIES)
public static partial class Indicator
{
    internal static Collection<ObvResult> CalcObv(
        this Collection<QuoteD> qdList,
        int? smaPeriods)
    {
        // check parameter arguments
        ValidateObv(smaPeriods);

        // initialize
        Collection<ObvResult> results = new();

        double prevClose = double.NaN;
        double obv = 0;

        // roll through quotes
        for (int i = 0; i < qdList.Count; i++)
        {
            QuoteD q = qdList[i];

            if (double.IsNaN(prevClose) || q.Close == prevClose)
            {
                // no change to OBV
            }
            else if (q.Close > prevClose)
            {
                obv += q.Volume;
            }
            else if (q.Close < prevClose)
            {
                obv -= q.Volume;
            }

            ObvResult r = new(q.Date)
            {
                Obv = obv
            };
            results.Add(r);

            prevClose = q.Close;

            // optional SMA
            if (smaPeriods != null && i + 1 > smaPeriods)
            {
                double? sumSma = 0;
                for (int p = i + 1 - (int)smaPeriods; p <= i; p++)
                {
                    sumSma += results[p].Obv;
                }

                r.ObvSma = sumSma / smaPeriods;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateObv(
        int? smaPeriods)
    {
        // check parameter arguments
        if (smaPeriods is not null and <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "SMA periods must be greater than 0 for OBV.");
        }
    }
}
