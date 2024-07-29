namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (SERIES)

public static partial class Adl
{
    internal static List<AdlResult> CalcAdl<TQuote>(
        this List<TQuote> source)
        where TQuote : IQuote
    {
        // initialize
        int length = source.Count;
        List<AdlResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            AdlResult r = Increment(
                source[i].Timestamp,
                source[i].High,
                source[i].Low,
                source[i].Close,
                source[i].Volume,
                i > 0 ? results[i - 1].Adl : 0);

            results.Add(r);
        }

        return results;
    }
}
