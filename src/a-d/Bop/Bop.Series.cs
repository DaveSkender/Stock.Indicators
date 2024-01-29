namespace Skender.Stock.Indicators;

// BALANCE OF POWER (SERIES)

public static partial class Indicator
{
    internal static List<BopResult> CalcBop(
        this List<QuoteD> qdList,
        int smoothPeriods)
    {
        // check parameter arguments
        Bop.Validate(smoothPeriods);

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
            BopResult r = new() { TickDate = qdList[i].TickDate };
            results.Add(r);

            if (i >= smoothPeriods - 1)
            {
                double sum = 0;
                for (int p = i - smoothPeriods + 1; p <= i; p++)
                {
                    sum += raw[p];
                }

                r.Bop = (sum / smoothPeriods).NaN2Null();
            }
        }

        return results;
    }
}
