namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (SERIES)

public static partial class Indicator
{
    internal static List<AdlResult> CalcAdl(
        this List<QuoteD> qdList,
        int? smaPeriods)
    {
        // check parameter arguments
        Adl.Validate(smaPeriods);

        // initialize
        List<AdlResult> results = new(qdList.Count);
        double lastAdl = 0;

        // roll through quotes
        for (int i = 0; i < qdList.Count; i++)
        {
            QuoteD q = qdList[i];

            (double mfm, double mfv, double adl) = Adl.Increment(lastAdl, q);

            AdlResult r = new(q.Date)
            {
                MoneyFlowMultiplier = mfm,
                MoneyFlowVolume = mfv,
                Adl = adl
            };
            results.Add(r);

            lastAdl = adl;

            // optional SMA
            if (smaPeriods != null && i + 1 >= smaPeriods)
            {
                double? sumSma = 0;
                for (int p = i + 1 - (int)smaPeriods; p <= i; p++)
                {
                    sumSma += results[p].Adl;
                }

                r.AdlSma = sumSma / smaPeriods;
            }
        }

        return results;
    }
}
