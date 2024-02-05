namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (SERIES)

public static partial class Indicator
{
    internal static List<AdlResult> CalcAdl(
        this List<QuoteD> qdList)
    {
        // initialize
        List<AdlResult> results = new(qdList.Count);
        double prevAdl = 0;

        // roll through quotes
        for (int i = 0; i < qdList.Count; i++)
        {
            QuoteD q = qdList[i];

            AdlResult r = Adl.Increment(prevAdl, q.High, q.Low, q.Close, q.Volume);
            r.Timestamp = q.Timestamp;
            results.Add(r);

            prevAdl = r.Adl;
        }

        return results;
    }
}