namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (SERIES)
public static partial class Indicator
{
    internal static List<SmaResult> CalcSma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Sma.Validate(lookbackPeriods);

        // initialize
        List<SmaResult> results = new(tpList.Count);

        // roll through quotes
        for (int i = 0; i < tpList.Count; i++)
        {
            (DateTime date, double _) = tpList[i];

            SmaResult result = new(date);
            results.Add(result);

            result.Sma = Sma
                .Increment(tpList, i, lookbackPeriods)
                .NaN2Null();
        }

        return results;
    }
}
