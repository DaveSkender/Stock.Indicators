namespace Skender.Stock.Indicators;

// TRUE RANGE (SERIES)

public static partial class Indicator
{
    // calculate series
    private static List<TrResult> CalcTr(
        this List<QuoteD> qdList)
    {
        // initialize
        int length = qdList.Count;
        List<TrResult> results = new(length);

        // skip first period
        if (length > 0)
        {
            results.Add(
                new(qdList[0].Timestamp, null));
        }

        // roll through source values
        for (int i = 1; i < length; i++)
        {
            QuoteD q = qdList[i];

            results.Add(new(
                Timestamp: q.Timestamp,
                Tr: Tr.Increment(qdList[i - 1].Close, q.High, q.Low)
                      .NaN2Null()));
        }

        return results;
    }
}
