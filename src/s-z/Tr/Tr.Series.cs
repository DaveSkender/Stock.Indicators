namespace Skender.Stock.Indicators;

// TRUE RANGE (SERIES)

public static partial class Indicator
{
    // calculate series
    internal static List<TrResult> CalcTr(
        this List<QuoteD> qdList)
    {
        // initialize
        List<TrResult> results = new(qdList.Count);
        double prevClose = 0;

        // roll through quotes
        for (int i = 0; i < qdList.Count; i++)
        {
            QuoteD q = qdList[i];

            TrResult r = new(q.Date);
            results.Add(r);

            if (i is 0)
            {
                prevClose = q.Close;
                continue;
            }

            r.Tr = Tr.Increment(prevClose, q.High, q.Low);

            prevClose = q.Close;
        }

        return results;
    }
}
