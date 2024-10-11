namespace Skender.Stock.Indicators;

// TRUE RANGE (SERIES)

public static partial class Tr
{
    // calculate series
    private static List<TrResult> CalcTr(
        this List<QuoteD> source)
    {
        // initialize
        int length = source.Count;
        TrResult[] results = new TrResult[length];

        // skip first period
        if (length > 0)
        {
            results[0] = new TrResult(source[0].Timestamp, null);
        }

        // roll through source values
        for (int i = 1; i < length; i++)
        {
            QuoteD q = source[i];

            results[i] = new TrResult(
                Timestamp: q.Timestamp,
                Tr: Increment(q.High, q.Low, source[i - 1].Close));
        }

        return new List<TrResult>(results);
    }
}
