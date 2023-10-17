namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (SERIES)

public static partial class Indicator
{
    internal static List<AdlResult> CalcAdl(
        this List<QuoteD> qdList)
    {
        // initialize
        List<AdlResult> results = new(qdList.Count);
        double lastAdl = 0;

        // roll through quotes
        for (int i = 0; i < qdList.Count; i++)
        {
            QuoteD q = qdList[i];

            (double mfm, double mfv, double adl) = Adl.Increment(
                lastAdl,
                q.High,
                q.Low,
                q.Close,
                q.Volume);

            AdlResult r = new(q.Date)
            {
                MoneyFlowMultiplier = mfm,
                MoneyFlowVolume = mfv,
                Adl = adl
            };
            results.Add(r);

            lastAdl = adl;
        }

        return results;
    }
}
