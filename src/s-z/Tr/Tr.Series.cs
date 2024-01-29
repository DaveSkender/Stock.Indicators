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

            TrResult r = new() { TickDate = q.TickDate };
            results.Add(r);

            if (i == 0)
            {
                prevClose = q.Close;
                continue;
            }

            r.Tr = Tr.Increment(prevClose, q.High, q.Low).NaN2Null();

            prevClose = q.Close;
        }

        return results;
    }
}
