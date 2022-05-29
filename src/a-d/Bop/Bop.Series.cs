namespace Skender.Stock.Indicators;

// BALANCE OF POWER (SERIES)
public static partial class Indicator
{
    // series calculation
    internal static IEnumerable<BopResult> CalcBop(
        this List<QuoteD> qdList,
        int smoothPeriods)
    {
        // check parameter arguments
        ValidateBop(smoothPeriods);

        // initialize
        int length = qdList.Count;
        List<BopResult> results = new(length);

        double[] raw = qdList
            .Select(x => (x.High != x.Low) ?
                ((x.Close - x.Open) / (x.High - x.Low)) : double.NaN)
            .ToArray();

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            BopResult r = new()
            {
                Date = qdList[i].Date
            };

            if (i >= smoothPeriods - 1)
            {
                double sum = 0;
                for (int p = i - smoothPeriods + 1; p <= i; p++)
                {
                    sum += raw[p];
                }

                r.Bop = sum / smoothPeriods;
            }

            results.Add(r);
        }

        return results;
    }

    // parameter validation
    private static void ValidateBop(
        int smoothPeriods)
    {
        // check parameter arguments
        if (smoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                "Smoothing periods must be greater than 0 for BOP.");
        }
    }
}
