namespace Skender.Stock.Indicators;

// TRUE RANGE (SERIES)

public static partial class Indicator
{
    // calculate series
    private static List<TrResult> CalcTr(
        this List<QuoteD> source)
    {
        // initialize
        int length = source.Count;
        List<TrResult> results = new(length);

        // skip first period
        if (length > 0)
        {
            results.Add(
                new(source[0].Timestamp, null));
        }

        // roll through source values
        for (int i = 1; i < length; i++)
        {
            QuoteD q = source[i];

            results.Add(new(
                Timestamp: q.Timestamp,
                Tr: Tr.Increment(source[i - 1].Close, q.High, q.Low)
                      .NaN2Null()));
        }

        return results;
    }
}
