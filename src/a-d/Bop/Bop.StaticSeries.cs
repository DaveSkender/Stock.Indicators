namespace Skender.Stock.Indicators;

// BALANCE OF POWER (SERIES)

public static partial class Bop
{
    public static IReadOnlyList<BopResult> ToBop<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int smoothPeriods = 14)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcBop(smoothPeriods);

    private static List<BopResult> CalcBop(
        this List<QuoteD> source,
        int smoothPeriods)
    {
        // check parameter arguments
        Bop.Validate(smoothPeriods);

        // initialize
        int length = source.Count;
        List<BopResult> results = new(length);

        double[] raw = source
            .Select(x => x.High - x.Low != 0 ?
                (x.Close - x.Open) / (x.High - x.Low) : double.NaN)
            .ToArray();

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            double? bop = null;

            if (i >= smoothPeriods - 1)
            {
                double sum = 0;
                for (int p = i - smoothPeriods + 1; p <= i; p++)
                {
                    sum += raw[p];
                }

                bop = (sum / smoothPeriods).NaN2Null();
            }

            results.Add(new(
                Timestamp: source[i].Timestamp,
                Bop: bop));
        }

        return results;
    }
}
