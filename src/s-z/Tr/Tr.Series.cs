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

            double hmpc = Math.Abs(q.High - prevClose);
            double lmpc = Math.Abs(q.Low - prevClose);

            r.Tr = Math.Max(q.High - q.Low, Math.Max(hmpc, lmpc));

            prevClose = q.Close;
        }

        return results;
    }
}
