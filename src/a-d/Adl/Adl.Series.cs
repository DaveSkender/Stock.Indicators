namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (SERIES)

public static partial class Adl
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

            AdlResult r = Increment(
                q.Timestamp, prevAdl,
                q.High, q.Low, q.Close, q.Volume);

            results.Add(r);

            prevAdl = r.Adl;
        }

        return results;
    }
}
