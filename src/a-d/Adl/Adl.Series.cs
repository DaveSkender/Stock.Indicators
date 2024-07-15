namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (SERIES)

public static partial class Adl
{
    internal static List<AdlResult> CalcAdl<TQuote>(
        this List<TQuote> source)
        where TQuote : IQuote
    {
        // initialize
        List<AdlResult> results = new(source.Count);
        double prevAdl = 0;

        // roll through source values
        for (int i = 0; i < source.Count; i++)
        {
            IQuote q = source[i];

            AdlResult r = Increment(
                q.Timestamp, prevAdl,
                q.High, q.Low, q.Close, q.Volume);

            results.Add(r);

            prevAdl = r.Adl;
        }

        return results;
    }
}
